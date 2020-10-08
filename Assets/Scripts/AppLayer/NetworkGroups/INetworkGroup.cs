using System;
using System.Collections.Generic;

namespace AppLayer.NetworkGroups
{
    public interface INetworkGroup : IEquatable<INetworkGroup>
    {
        bool                                IsAlive { get; }
        IReadOnlyList<IUser>                Members { get; }
        void                                Broadcast(byte[] message);
        void                                Broadcast(string message);
        IReadOnlyDictionary<string, string> CustomProperties { get; set; }
        void                                SetCustomProperties(IReadOnlyDictionary<string, string> properties);
        void DeleteCustomProperties(IReadOnlyList<string> properties);
    }
}