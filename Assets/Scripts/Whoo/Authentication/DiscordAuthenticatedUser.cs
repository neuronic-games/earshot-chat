using Cysharp.Threading.Tasks;
using DiscordAppLayer;

namespace Whoo
{
    public class DiscordAuthenticatedUser : IAuthenticatedContext
    {
        public readonly DiscordApp DiscordApp;

        public DiscordAuthenticatedUser(DiscordApp app)
        {
            DiscordApp = app;
        }

        public async UniTask<bool> IsValid()
        {
            return await UniTask.FromResult(DiscordApp != null && DiscordApp.Initialized);
        }

        public AuthTypes AuthType => AuthTypes.Discord;

        public async UniTask<bool> Detach()
        {
            if (DiscordApp == null || !DiscordApp.Initialized) return await UniTask.FromResult(false);
            DiscordApp.DestroyApp();
            return await UniTask.FromResult(true);
        }

        public async UniTask<string> GetUserId()
        {
            return await UniTask.FromResult(DiscordApp.LocalUser.Id.ToString());
        }
    }
}