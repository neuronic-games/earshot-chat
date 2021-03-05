using System;
using System.Collections.Generic;

namespace AppLayer.NetworkGroups
{
    public interface INetworkGroup : IEquatable<INetworkGroup>
    {
        #region Properties

        (string GroupId, string JoinPassword) IdAndPassword { get; }
        bool IsAlive { get; }
        IReadOnlyList<IUser> Members { get; }
        IUser LocalUser { get; }
        void Broadcast(byte[] message);
        void Broadcast(string message);
        IReadOnlyDictionary<string, string> CustomProperties { get; set; }
        bool IsOwner { get; }
        void SetCustomProperties(IReadOnlyDictionary<string, string> properties, Action onImplemented);
        void DeleteCustomProperties(IReadOnlyList<string> properties, Action onImplemented);

        void SetOrDeleteCustomProperty(string key, string value);
        void LeaveOrDestroy(Action<bool>      onLeft);
        void SafeLeave(Action<bool>           onLeft);

        #endregion

        #region Events

        event Action    OnDestroyed;
        event Action    OnUsersUpdated;
        event Action    OnGroupPropertiesUpdated;
        event Broadcast OnBroadcastReceived;

        #endregion
    }

    public delegate void Broadcast(long sender, byte[] data);
}