namespace AppLayer.Callbacks
{
    /// <summary>
    /// Callbacks for group activity.
    /// </summary>
    /// NOTE: This coding style wasn't adopted to switch services at runtime or adopt multiple services in the same
    /// build, but to limit the impact of having to change the service layer.
    /// Thus, the parameters of the callbacks below are based on Discord's lobbies, and will have to be changed
    /// if the service layer is ever changed.
    public interface IGroupCallbacks : ICallbacks
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