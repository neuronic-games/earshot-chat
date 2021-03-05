using System;
using System.Collections.Generic;
using AppLayer.NetworkGroups;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Whoo.Data;

namespace Whoo.Daemons
{
    /// <summary>
    /// Tries to mirror the discord-side users of the room's group to strapi occupants model.
    /// It is entirely reactive, though you may call Update to do a manual push.
    /// </summary>
    public class OccupantManagerDaemon : DaemonBase
    {
        public OccupantManagerDaemon(WhooRoom room) : base(room)
        {
        }

        public override async UniTask<bool> CanAttach()
        {
            bool isOwnerOfGroup = Room.HasNetworkCapability() && Room.RoomGroup.IsOwner;
            return await UniTask.FromResult(isOwnerOfGroup);
        }

        public override void Run()
        {
            Room.RoomGroup.OnUsersUpdated += MirrorOccupants;
            MirrorOccupants();
        }

        public override void Dispose()
        {
            base.Dispose();
            if (Room.RoomGroup != null)
            {
                Room.RoomGroup.OnUsersUpdated -= MirrorOccupants;
            }
        }

        public void MirrorOccupants() => MirrorOccupantsAsync().Forget();

        private readonly List<IUser> _lastKnown  = new List<IUser>();
        private readonly List<IUser> _toKeep     = new List<IUser>();
        private readonly List<IUser> _newlyAdded = new List<IUser>();

        public async UniTaskVoid MirrorOccupantsAsync()
        {
            //sort all users (history + current) into three categories:-
            // 1. To keep without modification
            // 2. To remove from occupants (left in group)
            // 3. To add to occupants (newly joined)
            for (var i = 0; i < Room.RoomGroup.Members.Count; i++)
            {
                var member   = Room.RoomGroup.Members[i];
                var existing = _lastKnown.Find(user => user.UniqueId == member.UniqueId);
                if (existing != null)
                {
                    _lastKnown.Remove(existing);
                    _toKeep.Add(member);
                }
                else
                {
                    _newlyAdded.Add(member);
                }
            }

            //clear the remaining users in group, as they've left
            _lastKnown.Clear();

            _lastKnown.AddRange(_toKeep);
            _lastKnown.AddRange(_newlyAdded);

            _toKeep.Clear();
            _newlyAdded.Clear();

            //finally, get list of occupants and push to strapi
            await Room.StrapiRoom.RoomModel.PutAsync(new RoomModel()
            {
                occupants = await GetOccupantsFromSortedMembers()
            }, string.Empty, Utils.StrapiModelSerializationDefaults());
            Room.RequestRefreshOnOccupants();
        }

        private async UniTask<List<Occupant>> GetOccupantsFromSortedMembers()
        {
            List<Occupant> result        = new List<Occupant>();
            var            profilesQuery = Endpoint.Base().Collection(Collections.Profile);

            //prepare query
            for (int index = 0; index < _newlyAdded.Count; index++)
            {
                IUser user = _newlyAdded[index];
                profilesQuery.Equals((Profile p) => p.platform_unique_id, user.UniqueId);
            }

            //do query
            List<Profile> list = (await Utils.GetJsonArrayAsync<Profile>(profilesQuery)).List;

            //create occupant from query results
            for (int index = 0; index < _newlyAdded.Count; index++)
            {
                IUser    user       = _newlyAdded[index];
                Occupant toAdd      = new Occupant {moderator = user.Equals(user.Group.LocalUser)};
                Profile  addProfile = list.Find(u => u.platform_unique_id == user.UniqueId);
                if (addProfile != null)
                {
                    toAdd.profile = addProfile.id;
                    result.Add(toAdd);
                }
                else
                {
                    Debug.Log($"User {user} does not have strapi profile.");
                }
            }

            //finally, add the id's of the ones we want to keep
            foreach (IUser user in _toKeep)
            {
                var occupant =
                    Room.StrapiRoom.RoomModel.occupants?.Find(o => o.profile.platform_unique_id == user.UniqueId);
                if (occupant != null) result.Add(occupant);
            }

            return result;
        }
    }
}