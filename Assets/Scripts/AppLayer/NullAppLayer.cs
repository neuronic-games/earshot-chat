using DiscordCommon.Interfaces;

namespace AppLayer
{
    public class NullAppLayer : IAppLayer
    {
        public void RegisterCallbacks(IGroupCallbacks listener)
        {
            
        }

        public void RegisterCallbacks(IVoiceCallbacks listener)
        {
            
        }

        public void RegisterCallbacks(IOverlayCallbacks listener)
        {
            
        }

        public void RemoveCallbacks(IGroupCallbacks listener)
        {
            
        }

        public void RemoveCallbacks(IVoiceCallbacks listener)
        {
            
        }

        public void RemoveCallbacks(IOverlayCallbacks listener)
        {
            
        }
    }
}