using System;
using System.Collections.Generic;
using System.Linq;
using AppLayer.NetworkGroups;
using AppLayer.Voice;
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;
using Whoo.Daemons;
using Whoo.Data;

namespace Whoo
{
    /// <summary>
    /// A Room represents the business-logic side of Whoo Rooms.
    /// Its only job is handling the room group and table groups.
    /// </summary>
    public class WhooRoom : NotifyPropertyChanged, IDisposable
    {
        public readonly StrapiRoom       StrapiRoom;
        public readonly INetworkGroup    RoomGroup;
        public readonly List<WhooTable>  Tables;
        public          List<DaemonBase> Daemons;

        public WhooTable this[int i]
        {
            get => i >= 0 && i < Tables.Count ? Tables[i] : null;
            set
            {
                if (i >= 0 && i < Tables.Count)
                {
                    Tables[i] = value;
                    OnPropertyChanged(nameof(Tables));
                }
            }
        }

        public INetworkGroup CurrentSitting { get; private set; }

        #region Setup

        public WhooRoom(StrapiRoom room, INetworkGroup group)
        {
            StrapiRoom = room;
            RoomGroup  = group;
            Tables     = new List<WhooTable>();
            Daemons    = new List<DaemonBase>();

            SeatLocalUserAtTableAsync(null).Forget();

            LoadAllTables();

            LoadNecessaryDaemons().Forget();

            room.OnRoomRefreshed -= OnRoomRefreshed;
            room.OnRoomRefreshed += OnRoomRefreshed;
        }

        private async UniTaskVoid LoadNecessaryDaemons()
        {
            OccupantManagerDaemon occupantManager = new OccupantManagerDaemon(this);
            if (await occupantManager.CanAttach())
            {
                occupantManager.Run();
                Daemons.Add(occupantManager);
            }

            NetworkMessageParserDaemon msgParser = new NetworkMessageParserDaemon(this);
            if (await msgParser.CanAttach())
            {
            }
        }

        private void OnRoomRefreshed(RoomModel rm)
        {
            OnPropertyChanged(nameof(StrapiRoom));
            LoadAllTables();
        }

        private void LoadAllTables()
        {
            foreach (var zone in StrapiRoom.RoomModel.zone_instances)
            {
                var table = Tables.Find(t => t.ZoneInstance.id == zone.id);
                if (table != null)
                {
                    table.ZoneInstance = zone;
                    continue;
                }

                table = new WhooTable(this, zone);
                Tables.Add(table);
            }

            OnPropertyChanged(nameof(Tables));
        }

        #endregion

        #region Misc

        private bool _loading;

        public void UpdateDaemons()
        {
            Daemons.ForEach(d => d.Update());
        }

        public async UniTask SeatLocalUserAtTableAsync(WhooTable table)
        {
            /*
             * We simply set the property "SittingAtTable" on a user in the room with the id of the zone they're in.
             * If this property doesn't exist or is an empty string, the user is in the room itself.
             * All we need this property for is to know where to display the user.
             * The reason we can't directly use the OnUserJoined callback for discord lobbies is to accomodate eavesdroppers.
             */

            if (table?.Group != null && table.Group == CurrentSitting) return;

            if (_loading) return;
            _loading = true;

            try
            {
                await WrappedMethod();
            }
            finally
            {
                _loading = false;
            }

            async UniTask WrappedMethod()
            {
                var roomUser = RoomGroup?.LocalUser;
                LeaveCurrentGroup();
                /*if (table == null)
                {
                    //RoomGroup.SetOrDeleteCustomProperty(GroupProps.TableUserIsSittingAt(roomUser.UniqueId), null);
                    RoomGroup?.SetOrDeleteCustomProperty(GroupProps.TableUserIsSittingAt(String.Empty), null);
                    if (RoomGroup is IVoiceChannel roomVChannel) roomVChannel.ConnectVoice(null, null);
                    return;
                }*/

                var tableGroup = await Utils.JoinGroup(table?.ZoneInstance.credentials);
                var zone       = table?.ZoneInstance.zone;
                //todo -- keep history of tables last joined.
                if (tableGroup == null)
                {
                    var capacity = zone.proximity
                        ? table.WhooRoom.StrapiRoom.RoomModel.layout.capacity
                        : zone.seats.Count * 2;

                    var failableTable = await new ExponentialBackoff<INetworkGroup>().Try(async () =>
                    {
                        var g = await Utils.CreateGroup((uint) capacity);
                        return new Failable<INetworkGroup>()
                        {
                            Value   = g,
                            HasValue = g != null
                        };
                    });

                    if (!failableTable.HasValue)
                    {
                        Debug.LogError($"{nameof(SeatLocalUserAtTableAsync)}: Unable to create new group.");
                        return;
                    }

                    tableGroup = failableTable.Value;

                    var (platform_id, pass) = tableGroup.IdAndPassword;

                    await StrapiRoom.RoomModel.PutAsync(new RoomModel()
                    {
                        zone_instances = StrapiRoom.RoomModel.zone_instances.Select(zi =>
                                                    {
                                                        if (zi.id == table.ZoneInstance.id)
                                                            return new ZoneInstance()
                                                            {
                                                                id = zi.id,
                                                                credentials = new PlatformCredentials()
                                                                {
                                                                    platform_id     = platform_id,
                                                                    platform_secret = pass
                                                                }
                                                            };
                                                        else
                                                        {
                                                            return new ZoneInstance {id = zi.id};
                                                        }
                                                    }).
                                                    ToArray()
                    }, string.Empty, Utils.StrapiModelSerializationDefaults());
                }

                CurrentSitting = tableGroup;
                //RoomGroup.SetOrDeleteCustomProperty(GroupProps.TableUserIsSittingAt(roomUser.UniqueId),
                RoomGroup.SetOrDeleteCustomProperty(GroupProps.TableUserIsSittingAt(String.Empty),
                    zone.id);
                if (tableGroup is IVoiceChannel tableChannel) tableChannel.ConnectVoice(null, null);

                void LeaveCurrentGroup()
                {
                    if (CurrentSitting == null || CurrentSitting == RoomGroup) return;
                    CurrentSitting.SafeLeave(null);
                    CurrentSitting = null;
                }
            }
        }

        #endregion

        #region IDisposable

        private bool _disposed = false;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed                  =  true;
            StrapiRoom.OnRoomRefreshed -= OnRoomRefreshed;
            Daemons.ForEach(d => d.Dispose());
            
            Tables.ForEach(t => t.Dispose());
            Tables.Clear();
            
            RoomGroup.LeaveOrDestroy(null);
            
            OnPropertyChanged(nameof(Tables));
            OnPropertyChanged(nameof(StrapiRoom));
        }

        #endregion
    }
}