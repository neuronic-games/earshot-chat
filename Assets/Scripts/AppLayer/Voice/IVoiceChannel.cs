using System;

namespace AppLayer.Voice
{
    /// <summary>
    /// Interface for voice channels.
    /// </summary>
    public interface IVoiceChannel
    {
        bool         IsConnected { get; }
        void         Connect(Action onConnect, Action onFail);
        void         Disconnect();
        event Action OnDisconnect;
    }
}