using Discord;

namespace DiscordCommon.Interfaces
{
    public interface IOverlayCallbacks : IListener
    {
        void OnToggle(bool manager);
    }
}