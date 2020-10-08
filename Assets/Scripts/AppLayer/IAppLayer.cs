using System.Collections.Generic;
using AppLayer.Callbacks;
using AppLayer.NetworkGroups;
using ServiceLocator;

namespace AppLayer
{
    public interface IAppLayer : IService
    {
        #region Callback Registration

        void RegisterCallbacks(IGroupCallbacks   listener);
        void RegisterCallbacks(IVoiceCallbacks   listener);
        void RegisterCallbacks(IOverlayCallbacks listener);
        void RemoveCallbacks(IGroupCallbacks     listener);
        void RemoveCallbacks(IVoiceCallbacks     listener);
        void RemoveCallbacks(IOverlayCallbacks   listener);

        #endregion

        #region Relationships

        IUser                        LocalUser   { get; }
        IReadOnlyList<IUser>         KnownUsers  { get; }
        IReadOnlyList<INetworkGroup> KnownGroups { get; }

        #endregion
    }
}