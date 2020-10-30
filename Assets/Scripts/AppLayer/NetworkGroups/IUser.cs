using System;
using System.Collections.Generic;
using AppLayer.Commands;

namespace AppLayer.NetworkGroups
{
    public interface IUser : IExecutor, IEquatable<IUser>
    {
        #region Properties

        INetworkGroup Group { get; }
        bool IsReady { get; }
        string UniqueId { get; }
        string Name { get; }
        IReadOnlyDictionary<string, string> CustomProperties { get; set; }
        void SetCustomProperties(IReadOnlyDictionary<string, string> value, Action onImplemented);
        void DeleteCustomProperties(IReadOnlyList<string> properties, Action onImplemented);

        void SetOrDeleteCustomProperty(string key, string value);

        #endregion

        #region Events

        event Action<bool> OnSpeaking;

        event Action OnCustomPropertiesUpdated;

        #endregion
    }
}