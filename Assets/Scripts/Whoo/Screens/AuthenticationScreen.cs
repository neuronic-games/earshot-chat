using Cysharp.Threading.Tasks;
using UnityEngine;
using Screen = UI.Screens.Screen;

namespace Whoo.Screens
{
    public class AuthenticationScreen : Screen
    {
        [SerializeField]
        private float refreshInterval;

        [SerializeField]
        private AuthTypes[] allowedAuths;

        private float _lastRefresh;

        #region Screen Management

        public override async UniTask Refresh()
        {
            if (await Build.AuthContext.ContextIsValid())
            {
                Build.ToStartScreen().Forget();
            }
        }

        public void Update()
        {
            if (Time.realtimeSinceStartup >= _lastRefresh + refreshInterval)
            {
                _lastRefresh = Time.realtimeSinceStartup;
                Refresh().Forget();
            }
        }

        #endregion

        public override async UniTask Setup()
        {
            
        }
    }
}