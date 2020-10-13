using System;
using System.Collections.Generic;
using AppLayer.Commands;

namespace AppLayer.NetworkGroups
{
    public interface IUser : IExecutor, IEquatable<IUser>
    {
        INetworkGroup Group { get; }
        bool IsReady { get; }
        string Name { get; }
        IReadOnlyDictionary<string, string> CustomProperties { get; set; }
        void SetCustomProperties(IReadOnlyDictionary<string, string> value, Action onImplemented);
        void DeleteCustomProperties(IReadOnlyList<string> properties, Action onImplemented);
    }
}