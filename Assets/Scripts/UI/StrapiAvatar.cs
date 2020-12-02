using Cysharp.Threading.Tasks;
using Whoo;

namespace UI
{
    public class StrapiAvatar : AvatarBase
    {
        public string url;

        public async UniTask<bool> LoadAvatar(string _url)
        {
            url = _url;
            return await _LoadAvatar();
        }

        public override void LoadAvatar()
        {
            _LoadAvatar().Forget();
        }

        private async UniTask<bool> _LoadAvatar()
        {
            var tex = await Utils.LoadPossibleWhooImage(url);
            if (tex != null)
            {
                image.texture = tex;
                return true;
            }

            return false;
        }
    }
}