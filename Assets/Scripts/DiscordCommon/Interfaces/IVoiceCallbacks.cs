using Discord;

namespace DiscordCommon.Interfaces
{
    public interface IVoiceCallbacks : IListener
    {
        void OnSettingsUpdate();
    }
}