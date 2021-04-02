using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Cysharp.Threading.Tasks;
using DiscordAppLayer;
using Newtonsoft.Json;
using TMPro;
using UI.Dialogs;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Whoo.Data;
using Screen = UI.Screens.Screen;

namespace Whoo.Screens
{
    public class AuthenticationScreen : Screen
    {
        [Header("General Config")]
        [SerializeField]
        private float refreshInterval;

        [SerializeField]
        private List<AuthTypes> allowedAuths;

        [SerializeField]
        private Button loginWithDiscord;

        [FormerlySerializedAs("username")]
        [SerializeField]
        private TMP_InputField usernameField;

        [FormerlySerializedAs("password")]
        [SerializeField]
        private TMP_InputField passwordField;

        [SerializeField]
        private TMP_InputField emailField;

        [SerializeField]
        private Button loginUsername;

        [SerializeField]
        private Button registerUsername;

        [SerializeField]
        private GameObject loadingBlocker;

        public string defaultErrorTitle = "Oops!";

        private float _lastRefresh;

        #region Screen Management

        private void Awake()
        {
            loginWithDiscord.onClick.AddListener(() => LoginWithDiscord());
            loginUsername.onClick.AddListener(() => LoginWithUsername());
            registerUsername.onClick.AddListener(() => RegisterWithUsername());
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

        #region Screen

        public override async UniTask Setup()
        {
            await Refresh();
        }

        public override async UniTask Refresh()
        {
            if (await Build.ContextIsValid())
            {
                Build.ToStartScreen().Forget();
            }
            else
            {
                bool hasDiscord = allowedAuths.Contains(AuthTypes.Discord) && DiscordApp.GetDiscordApp(out var app);
                loginWithDiscord.gameObject.SetActive(hasDiscord);
            }
        }

        #endregion

        #region Auth Callbacks

        public delegate UniTask<Failable<AuthResponse>> Authenticator();

        public async UniTask LoginWithDiscord()
        {
            if (DiscordApp.GetDiscordApp(out var app))
            {
                await TryAuthenticate(Authentication.LoginWithDiscord);
            }
        }

        public async UniTask LoginWithUsername()
        {
            string username = usernameField.text;
            string password = passwordField.text;
            string email    = emailField.text;

            bool hasUsername = !string.IsNullOrEmpty(username);

            await TryAuthenticate(() => Authentication.LoginWithIdentifier(hasUsername ? username : email, password));
        }

        public async UniTask RegisterWithUsername()
        {
            string username = usernameField.text;
            string password = passwordField.text;
            string email    = emailField.text;

            await TryAuthenticate(() => Authentication.RegisterWithUsernameEmail(username, email, password));
        }

        private async UniTask TryAuthenticate(Authenticator authenticator, string defaultErrorMessage = null)
        {
            loadingBlocker.SetActive(true);

            Failable<AuthResponse> response = await authenticator.Invoke();

            if (response.HasValue)
            {
                SetupAuthenticatedContext(response.Value);
            }
            else
            {
                Dialog.Get().RequestInfo(defaultErrorTitle, response.PrettyError ?? defaultErrorMessage);
            }

            loadingBlocker.SetActive(false);
        }

        private void SetupAuthenticatedContext(AuthResponse response)
        {
            //todo -- check validity
            if (string.IsNullOrEmpty(response.jwt) || response.user == null)
            {
                Debug.Log($"Failed to setup authenticated user context.");
                return;
            }

            Build.AuthContexts.Add(new StrapiAuthenticatedUser(response.user, response.jwt));
            Refresh().Forget();
        }

        #endregion
    }
}