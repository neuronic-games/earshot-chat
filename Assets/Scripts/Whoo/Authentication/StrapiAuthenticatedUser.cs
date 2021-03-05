using Cysharp.Threading.Tasks;
using Whoo.Data;

namespace Whoo
{
    public class StrapiAuthenticatedUser : IAuthenticatedContext
    {
        public StrapiUser StrapiUser { get; private set; }
        public string     Jwt        { get; private set; }

        public StrapiAuthenticatedUser(StrapiUser strapiUser, string jwt)
        {
            StrapiUser = strapiUser;
            Jwt        = jwt;
        }

        public async UniTask<bool> IsValid()
        {
            return await UniTask.FromResult(!string.IsNullOrEmpty(Jwt)); //todo - mock api request
        }

        public AuthTypes AuthType { get; set; }

        public async UniTask<bool> Detach()
        {
            StrapiUser = null;
            Jwt        = null;
            return await UniTask.FromResult(true);
        }

        public async UniTask<string> GetUserId()
        {
            return await UniTask.FromResult(StrapiUser.platform_id);
        }
    }
}