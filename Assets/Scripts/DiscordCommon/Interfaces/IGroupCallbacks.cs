using System;

namespace DiscordCommon.Interfaces
{
    public interface IGroupCallbacks : IListener
    {
        void OnLobbyUpdate(long             lobbyid);
        void OnNetworkMessage(long          lobbyid, long userid, byte channelid, byte[] data);
        void OnMemberConnect(long           lobbyid, long userid);
        void OnSpeaking(long                lobbyid, long userid, bool speaking);
        void OnMemberUpdate(long            lobbyid, long userid);
        void OnMemberDisconnect(long        lobbyid, long userid);
        void OnLobbyDelete(long             lobbyid, uint reason);
        void OnLobbyMessage(long lobbyid, long userid, byte[] data);
    }
}