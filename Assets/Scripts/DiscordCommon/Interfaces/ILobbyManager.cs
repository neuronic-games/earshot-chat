using System;

namespace DiscordCommon.Interfaces
{
    public interface ILobbyManager
    {
        void OnLobbyDelete();
        void OnLobbyMessage();
        void OnLobbyUpdate();

        void OnSpeaking();
        void OnMemberConnect();
        void OnMemberDisconnect();
        void OnMemberUpdate();
        void OnNetworkMessage();
    }
}