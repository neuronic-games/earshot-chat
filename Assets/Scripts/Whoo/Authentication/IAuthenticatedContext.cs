using Cysharp.Threading.Tasks;

namespace Whoo
{
    public enum AuthTypes
    {
        Discord,
        StrapiUsername
    }

    public interface IAuthenticatedContext
    {
        UniTask<bool>   IsValid();
        AuthTypes       AuthType { get; }
        UniTask<bool>   Detach();
        UniTask<string> GetUserId();
    }
}