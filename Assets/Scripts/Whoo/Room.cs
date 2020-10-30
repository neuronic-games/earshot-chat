using System;
using System.Collections.Generic;
using AppLayer.NetworkGroups;
using AppLayer.Voice;
using UnityEngine;

namespace Whoo
{
    /// <summary>
    /// A Room represents the business-logic side of Whoo Rooms.
    /// Its only job is handling the room group and table groups.
    /// </summary>
    public class Room : IDisposable
    {
        public readonly INetworkGroup RoomGroup;

        public readonly List<Table> Tables;
        public int ExpectedTables { get; private set; }

        public IUser CurrentSitting { get; private set; }

        public Room(INetworkGroup group)
        {
            RoomGroup = group;
            Tables    = new List<Table>();

            SeatLocalUserAtGroup(RoomGroup);
            
            LoadAllTablesFromMetadata();
        }

        public int LoadAllTablesFromMetadata()
        {
            if (RoomGroup.CustomProperties.TryGetValue(Constants.TableCount, out string countStr) &&
                int.TryParse(countStr, out int count))
            {
                Debug.Log($"Found {countStr} tables in group.");
                for (int i = 0; i < count; i++)
                {
                    int iLambda = i;
                    Utils.DelayedAction(() => LoadTable(Utils.GetTableMetaDataKey(iLambda),
                            b =>
                            {
                                Debug.Log((b ? "Successfully loaded" : "Failed to load") +
                                          $" table number {iLambda}.");
                            })
                        , (iLambda + 1) * Constants.GroupJoinRate);
                }

                return count;
            }

            return 0;
        }

        #region Table Methods

        /// <summary>
        /// Only use onFinish action for error-checking.
        /// </summary>
        public void AddTable(TableDisplayProperties props, Action<Table> onFinish)
        {
            if (!RoomGroup.IsAlive) return;
            AppLayer.AppLayer.Get().CreateNewGroup(Constants.DefaultCapacity, false, OnCreated);

            ExpectedTables++;
            
            void OnCreated(INetworkGroup group)
            {
                ExpectedTables--;
                Table table = null;
                if (group != null)
                {
                    table = new Table(this, group, props);
                    var groupIdAndPassword = table.Group.IdAndPassword;
                    RoomGroup.SetOrDeleteCustomProperty(Utils.GetTableMetaDataKey(Tables.Count),
                        $"{groupIdAndPassword.GroupId},{groupIdAndPassword.JoinPassword}");
                    Tables.Add(table);
                    RoomGroup.SetOrDeleteCustomProperty(Constants.TableCount, Tables.Count.ToString());
                }

                onFinish?.Invoke(table);
            }
        }

        public void RemoveTable(Table table)
        {
            RemoveTable(table.Group);
        }

        public void RemoveTable(INetworkGroup tableGroup)
        {
            if (tableGroup == null) return; //throw?
            int index = Tables.FindIndex(t => t.Group == tableGroup);
            if (index == -1) return;
            tableGroup.LeaveOrDestroy(OnLeft);

            void OnLeft(bool success)
            {
                if (success)
                {
                    Tables.RemoveAt(index);
                    RoomGroup.SetOrDeleteCustomProperty(Utils.GetTableMetaDataKey(index), null);
                }
            }
        }

        public void LoadTable(string metadataKey, Action<bool> success)
        {
            string joinDetails = null;
            ExpectedTables++;

            try
            {
                if (RoomGroup.CustomProperties.TryGetValue(metadataKey, out joinDetails))
                {
                    string[] details  = joinDetails.Split(',');
                    long     id       = long.Parse(details[0]); //discord detail
                    string   password = details[1];

                    if (Tables.Exists(t => t.Group.IdAndPassword.GroupId == details[0]))
                    {
                        success?.Invoke(true);
                        return;
                    }

                    AppLayer.AppLayer.Get().JoinGroup(id, password, OnJoin);

                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(
                    $"{GetType().Name} : {nameof(LoadTable)} Exception caught. Value of property {joinDetails}.\n{ex}");
            }

            success?.Invoke(false);

            void OnJoin(INetworkGroup networkGroup)
            {
                ExpectedTables--;
                if (networkGroup == null)
                {
                    success?.Invoke(false);
                }
                else
                {
                    Table table = new Table(this, networkGroup, null);
                    Tables.Add(table);
                    success?.Invoke(true);
                }
            }
        }

        #endregion

        #region Misc

        public void SeatLocalUserAtGroup(INetworkGroup group)
        {
            if (CurrentSitting?.Group == group) return;
            if (CurrentSitting?.Group is IVoiceChannel channel && channel.IsConnectedVoice) channel.DisconnectVoice();
            var otherUser = group.LocalUser;
            CurrentSitting?.SetSitting(false);
            CurrentSitting = otherUser;
            CurrentSitting.SetSitting(true);
            if (CurrentSitting.Group is IVoiceChannel otherChannel) otherChannel.ConnectVoice(null, null);
        }

        #endregion

        #region IDisposable

        private bool _disposed = false;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            //DetachListeners(RoomGroup);
            RoomGroup.LeaveOrDestroy(null);
            Tables.ForEach(t => t.Dispose());
            Tables.Clear();
        }

        #endregion
    }

    /*
    public abstract class NetworkGroupEventListener
    {
        public NetworkGroupEventListener()
        {
            
        }
        
        protected NetworkGroupEventListener(INetworkGroup @group)
        {
            AttachListeners(group);
        }

        protected bool DetachListeners(INetworkGroup group)
        {
            if (group == null) return false;
            group.OnDestroyed              -= OnGroupDestroyed;
            group.OnUsersUpdated           -= OnUsersUpdated;
            group.OnGroupPropertiesUpdated -= OnGroupPropertiesUpdated;
            return true;
        }
        
        protected bool AttachListeners(INetworkGroup group)
        {
            if (!DetachListeners(group)) return false;
            group.OnDestroyed              += OnGroupDestroyed;
            group.OnUsersUpdated           += OnUsersUpdated;
            group.OnGroupPropertiesUpdated += OnGroupPropertiesUpdated;
            return true;
        }

        protected abstract void OnGroupPropertiesUpdated();

        protected abstract void OnUsersUpdated();

        protected abstract void OnGroupDestroyed();
    }*/
}