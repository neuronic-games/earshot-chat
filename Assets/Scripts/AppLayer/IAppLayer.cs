using System;
using System.Collections.Generic;
using AppLayer.Callbacks;
using AppLayer.NetworkGroups;
using ServiceLocator;

namespace AppLayer
{
    public interface IAppLayer : IService
    {
        #region Disposing

        void DestroyApp();

        #endregion

        #region Callback Registration

        void RegisterCallbacks(IGroupCallbacks   listener);
        void RegisterCallbacks(IVoiceCallbacks   listener);
        void RegisterCallbacks(IOverlayCallbacks listener);
        void RegisterCallbacks(IUserCallbacks    listener);
        void RemoveCallbacks(IGroupCallbacks     listener);
        void RemoveCallbacks(IVoiceCallbacks     listener);
        void RemoveCallbacks(IOverlayCallbacks   listener);
        void RemoveCallbacks(IUserCallbacks      listener);

        #endregion

        #region Relationships

        IReadOnlyList<IUser>         KnownUsers  { get; }
        IReadOnlyList<INetworkGroup> KnownGroups { get; }

        #endregion

        #region Factory

        int  GroupCapacity  { get; }
        bool CanCreateGroup { get; }
        bool CanJoinGroup   { get; }
        void CreateNewGroup(uint       capacity, bool   locked, Action<INetworkGroup> onCreated);
        void JoinGroup(long            groupId,  string secret, Action<INetworkGroup> onJoined);
        void DeleteGroup(INetworkGroup group);

        #endregion
    }
}