using System;

namespace AppLayer.Voice
{
    /// <summary>
    /// Interface for voice channels.
    /// </summary>
    public interface IVoiceChannel
    {
        bool         IsConnectedVoice { get; }
        void         ConnectVoice(Action onConnect, Action onFail);
        void         DisconnectVoice();
        event Action OnDisconnectVoice;
    }
}