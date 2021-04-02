using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using Whoo.Data;

namespace Whoo
{
    public static class Authentication
    {
        #region Class Definitions

        [Serializable]
        public struct StrapiLoginRequest
        {
            public string identifier;
            public string password;
        }

        [Serializable]
        public struct StrapiForgotPasswordRequest
        {
            public string email;
        }

        [Serializable]
        public struct StrapiResetPasswordRequest
        {
            public string code;
            public string password;
            public string passwordConfirmation;
        }

        [Serializable]
        public struct StrapiRegistrationRequest
        {
            public string username;
            public string password;
            public string email;
        }

        [Serializable]
        public class DiscordExchangeResponse
        {
            public string access_token;
            public string token_type;
            public int    expires_in;
            public string refresh_token;
            public string scope;
            public double timestamp;
        }

        #endregion

        #region Public Methods

        public static async UniTask<Failable<AuthResponse>> LoginWithIdentifier(string usernameOrEmail, string password)
        {
            string api = Endpoint.Base() + "auth/local";

            try
            {
                AuthResponse response = await Utils.PostObjectAsync<StrapiLoginRequest, AuthResponse>(api,
                    new StrapiLoginRequest()
                    {
                        identifier = usernameOrEmail,
                        password   = password
                    });
                return new Failable<AuthResponse>(response);
            }
            catch (UnityWebRequestException webreq)
            {
                if (webreq.ResponseCode == 400)
                {
                    StrapiError error        = JsonConvert.DeserializeObject<StrapiError>(webreq.Text);
                    string      errorMessage = error.message.FirstOrDefault()?.messages.FirstOrDefault()?.message;
                    return new Failable<AuthResponse>(errorMessage);
                }
                else
                {
                    Debug.Log($"Unknown error occured: {webreq.Error}");
                }
            }

            return default;
        }

        public static async UniTask<Failable<AuthResponse>> RegisterWithUsernameEmail(string username, string email,
            string                                                                           password)
        {
            string api = Endpoint.Base() + "auth/local/register";

            try
            {
                AuthResponse response = await Utils.PostObjectAsync<StrapiRegistrationRequest, AuthResponse>(api,
                    new StrapiRegistrationRequest()
                    {
                        password = password,
                        email    = email,
                        username = username
                    });
                return new Failable<AuthResponse>(response);
            }
            catch (UnityWebRequestException webreq)
            {
                if (webreq.ResponseCode == 400)
                {
                    StrapiError error        = JsonConvert.DeserializeObject<StrapiError>(webreq.Text);
                    string      errorMessage = error.message.FirstOrDefault()?.messages.FirstOrDefault()?.message;
                    return new Failable<AuthResponse>(errorMessage);
                }
            }

            return default;
        }

        public static async UniTask<Failable<AuthResponse>> LoginWithDiscord()
        {
            var accessToken = await GetDiscordOAuth("identify", "email");
            if (accessToken == null) return default;

            var url = Endpoint.Base() + "auth/discord/callback?access_token=" + accessToken.access_token;
            try
            {
                var response = await Utils.GetJsonObjectAsync<AuthResponse>(url);
                return new Failable<AuthResponse>(response);
            }
            catch (UnityWebRequestException)
            {
                return default;
            }
        }

        public static async UniTask<bool> KickOffForgotPassword(string email)
        {
            CancelOngoing();
            var api = Endpoint.Base() + "auth/forgot-password";
            try
            {
                var response = await
                    Utils.PostObjectAsync<StrapiForgotPasswordRequest, EmptyResponse>(api,
                        new StrapiForgotPasswordRequest() {email = email});
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }

            return false;
        }

        public static async UniTask<bool> FinishForgotPassword(string code, string password,
            string                                                    passwordConfirmation)
        {
            if (string.Equals(password, passwordConfirmation)) return false;

            string api = Endpoint.Base() + "auth/reset-password";
            try
            {
                var response = Utils.PostObjectAsync<StrapiResetPasswordRequest, EmptyResponse>(api,
                    new StrapiResetPasswordRequest()
                    {
                        code = code, password = password, passwordConfirmation = passwordConfirmation
                    });
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }

            return false;
        }

        public static void CancelOngoing()
        {
            authCToken.Cancel();
            authCToken = new CancellationTokenSource();
        }

        #endregion

        #region Private Members

        private static CancellationTokenSource authCToken = new CancellationTokenSource();

        private static async UniTask<DiscordExchangeResponse> GetDiscordOAuth(params string[] scopes)
        {
            CancelOngoing();

            string savedOauthString = PlayerPrefs.GetString("discord_oauth");
            if (!string.IsNullOrEmpty(savedOauthString))
            {
                var savedOauth = new DiscordExchangeResponse();
                JsonConvert.PopulateObject(savedOauthString, savedOauth);
                if (Utils.EpochToNowSpan.TotalSeconds > savedOauth.expires_in + savedOauth.timestamp) return savedOauth;
                else PlayerPrefs.SetString("discord_oauth", string.Empty);
            }

            UnityWebRequest unityWebRequest = null;

            //send 
            using (var listener = new HttpListener())
            {
                string callbackUrl = "http://localhost:54991/auth/discord/callback/";
                listener.Prefixes.Add(callbackUrl);
                listener.Start();
                //Application.OpenURL(Endpoint.Base() + "connect/discord");
                Application.OpenURL(
                    "https://discord.com/api/oauth2/authorize?client_id=757347194410893432&redirect_uri=http%3A%2F%2Flocalhost%3A54991%2Fauth%2Fdiscord%2Fcallback&response_type=code&scope=identify%20email&prompt=none");

                var (cancelled, context) = await listener.GetContextAsync().
                                                          AsUniTask().
                                                          AttachExternalCancellation(authCToken.Token).
                                                          SuppressCancellationThrow();

                if (cancelled) return default;

                context.Response.ClearTo200OK();
                context.Response.Close();

                /*var formData = new MultipartFormDataSection(
                    $"client_id={757347194410893432}&client_secret=DcarCXTKGiCzv7KXgRe992FnzR&grant_type=authorization_code&code={context.Request.QueryString["code"]}&scope={string.Join(" ", scopes)}");*/

                unityWebRequest = UnityWebRequest.
                    Post("https://discord.com/api/oauth2/token", new Dictionary<string, string>()
                    {
                        {"client_id", "757347194410893432"},
                        {"client_secret", "DcarCXTKGiCzv7KXgRe992FnzR"},
                        {"redirect_uri", "http%3A%2F%2Flocalhost%3A1337%2Fconnect%2Fdiscord%2Fcallback"},
                        {"code", context.Request.QueryString["code"]},
                        {"scope", string.Join(" ", scopes)},
                        {"grant_type", "authorization_code"},
                    });
            }

            try
            {
                var exchangeResponseString = (await unityWebRequest.
                                                    SendWebRequest().
                                                    WithCancellation(authCToken.Token)).downloadHandler.text;
                var exchangeResponse = new DiscordExchangeResponse();
                JsonConvert.PopulateObject(exchangeResponseString, exchangeResponse);

                if (!string.IsNullOrEmpty(exchangeResponse.access_token))
                {
                    exchangeResponse.timestamp = Utils.EpochToNowSpan.TotalSeconds;
                    PlayerPrefs.SetString("discord_oauth", exchangeResponseString);
                }

                return exchangeResponse;
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }

            return default;
        }

        #endregion
    }
}