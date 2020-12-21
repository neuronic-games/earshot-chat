using Cysharp.Threading.Tasks;

namespace Whoo
{
    public enum AuthTypes
    {
        Discord,
        Username
    }

    public interface IAuthenticatedContext
    {
        UniTask<bool> IsValid();
        AuthTypes     AuthType { get; }
        UniTask<bool> Detach();
    }

    public static class AuthUtils
    {
        public static async UniTask<bool> ContextIsValid(this IAuthenticatedContext ctx)
        {
            return ctx != null && await ctx.IsValid();
        }
    }
}