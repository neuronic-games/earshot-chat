using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AppLayer.NetworkGroups;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UI.Dialogs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;
using Whoo.Data;
using Object = UnityEngine.Object;

namespace Whoo
{
    public static class Utils
    {
        #region Time

        /// <summary>
        /// The time now from unix epoch represented as a TimeSpan struct.
        /// </summary>
        public static TimeSpan EpochToNowSpan => (DateTime.UtcNow - new DateTime(1970, 1, 1));

        /// <summary>
        /// The time now from unix epoch in seconds.
        /// </summary>
        public static double EpochToNowSeconds => EpochToNowSpan.TotalSeconds;

        #endregion

        /// <summary>
        /// Destroys all children of a given transform.
        /// </summary>
        public static void ClearChildren(this Transform transform, bool destroyInactive = false)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                if (child.gameObject.activeSelf || destroyInactive)
                {
                    Object.Destroy(child.gameObject);
                }
            }
        }

        #region Json

        private static readonly JsonSerializerSettings s_StrapiModelSerializerSettings = new JsonSerializerSettings()
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling    = NullValueHandling.Ignore
        };

        public static JsonSerializerSettings StrapiModelSerializationDefaults() => s_StrapiModelSerializerSettings;

        /// <summary>
        /// Wraps a json string that represents a top-level array into an object for deserialization into c# object.
        /// </summary>
        public static string WrapJsonArrayWithObject(string json, string key) => $"{{ \"{key}\" : {json} }}";

        /// <summary>
        /// Tries to populate an object with the json response from a GET request made to the given url.
        /// </summary>
        /// <param name="url">The URL to make a GET request to.</param>
        /// <param name="wrap">True if the returned json should be wrapped into a top-level object.</param>
        /// <param name="key">Key of the top-level object, if json is to be wrapped.</param>
        /// <typeparam name="T">Type of object to create and populate with the json.</typeparam>
        /// <return>Instance of object, populated based on json response. Never null.</return>
        public static async UniTask<T> GetJsonObjectAsync<T>(string url, string authHeader = null, bool wrap = false,
            string                                                  key = null)
            where T : new()
        {
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                if (!string.IsNullOrEmpty(authHeader))
                {
                    request.SetRequestHeader("Authorization", authHeader);
                }

                string response = (await request.SendWebRequest()).downloadHandler.text;

                T result = new T();
                JsonConvert.PopulateObject(wrap ? WrapJsonArrayWithObject(response, key) : response, result);
                if (result is IDebugData debug) debug.Json = response;
                return result;
            }
        }

        public static async UniTask<JsonArray<T>> GetJsonArrayAsync<T>(string url, string authHeader = null)
            where T : new()
        {
            return await GetJsonObjectAsync<JsonArray<T>>(url, authHeader, true, nameof(JsonArray<T>.List));
        }

        public static async UniTask<TResponse> PostObjectAsync<TPost, TResponse>(string url, TPost obj,
            string                                                                      authHeader = null)
            where TResponse : new()
        {
            return await PostObjectAsync<TPost, TResponse>(url,
                JsonConvert.SerializeObject(obj, Utils.StrapiModelSerializationDefaults()), authHeader);
        }

        public static async UniTask<TResponse> PostObjectAsync<TPost, TResponse>(string url, string obj,
            string                                                                      authHeader = null)
            where TResponse : new()
        {
            using (var webRequest = UnityWebRequest.Put(url, obj))
            {
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.method = UnityWebRequest.kHttpVerbPOST;
                if (!string.IsNullOrEmpty(authHeader))
                {
                    webRequest.SetRequestHeader("Authorization", authHeader);
                }

                string responseString = (await webRequest.SendWebRequest()).downloadHandler.text;

                var response = new TResponse();
                JsonConvert.PopulateObject(responseString, response);
                return response;
            }
        }

        #endregion

        #region Strapi

        /// <summary>
        /// Loads an image from a uri that may possibly be a Strapi Resource uri (of form "/uploads/{id}").
        /// </summary>
        public static async UniTask<Texture> LoadPossibleWhooImage(string url, bool forceReloadCache = false,
            bool                                                          cancellable = false,
            CancellationToken                                             token       = default)
        {
            if (string.IsNullOrEmpty(url)) return null;
            return await LoadImage(url.StartsWith("/uploads/") ? Endpoint.Upload(url) : url, forceReloadCache,
                cancellable,                                                                 token);
        }

        /// <summary>
        /// Loads an image from a url and returns it as a texture. Has internal caching.
        /// </summary>
        public static async UniTask<Texture> LoadImage(string url, bool forceReloadCache = false,
            bool                                              cancellable = false,
            CancellationToken                                 token       = default)
        {
            if (!forceReloadCache && TryGetCachedImage(url, out Texture2D texture)) return texture;
            UnityWebRequestAsyncOperation op      = UnityWebRequestTexture.GetTexture(url).SendWebRequest();
            UnityWebRequest               request = null;

            if (cancellable)
            {
                var (cancelled, webr) = await op.WithCancellation(token).
                                                 SuppressCancellationThrow();
                if (cancelled) return null;
                request = webr;
            }
            else
            {
                request = await op;
            }

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.Log($"Network error fetching texture from URL: {url}");
                return null;
            }
            else
            {
                texture           =  ((DownloadHandlerTexture) request.downloadHandler).texture;
                texture.hideFlags |= HideFlags.DontSave;
                SubmitCachedImage(url, texture);
                return texture;
            }
        }

        #region Image Caching

        private static readonly Dictionary<string, Texture2D> s_ImageCache = new Dictionary<string, Texture2D>();

        private static void SubmitCachedImage(string url, Texture2D texture2D)
        {
            s_ImageCache[url] = texture2D;
        }

        private static bool TryGetCachedImage(string url, out Texture2D texture2D)
        {
            return s_ImageCache.TryGetValue(url, out texture2D);
        }

        #endregion

        #endregion

        #region UI

        /// <summary>
        /// Replaces the main texture of a raw image, rescaling the image UVs so that texture fits the image. 
        /// </summary>
        public static void ApplyTextureAndFit(this RawImage image, Texture texture)
        {
            if (texture == null)
            {
                Debug.Log($"{nameof(ApplyTextureAndFit)}: texture is null. Image: {image}");
                return;
            }

            if (image.texture != null && image.texture.hideFlags.HasFlag(HideFlags.DontSave))
            {
                Object.Destroy(image.texture);
                image.texture = null;
            }

            Rect  imgRect   = image.rectTransform.rect;
            float imgAspect = imgRect.height / imgRect.width;
            float texAspect = texture.height / (float) texture.width;

            Rect uv = new Rect(0, 0, 1, 1);
            if (imgAspect < texAspect)
            {
                uv.height = imgAspect       / texAspect;
                uv.y      = (1 - uv.height) * 0.5f;
            }
            else
            {
                uv.width = texAspect      / imgAspect;
                uv.x     = (1 - uv.width) * 0.5f;
            }

            image.texture = texture;
            image.uvRect  = uv;
        }

        #endregion

        #region Http

        /// <summary>
        /// Clears an http listener response to an empty 200 OK state.
        /// </summary>
        /// <param name="response"></param>
        public static void ClearTo200OK(this HttpListenerResponse response)
        {
            response.Headers.Clear();
            response.SendChunked = false;
            response.StatusCode  = 200;
            response.Headers.Add("Server", String.Empty);
            response.Headers.Add("Date",   String.Empty);
        }

        #endregion

        #region Whoo general

        public static async UniTask<INetworkGroup> CreateGroup(uint capacity, bool showDialogOnError = false)
        {
            var app = AppLayer.AppLayer.Get();
            if (app.CanCreateGroup)
            {
                UniTaskCompletionSource<INetworkGroup> groupSource = new UniTaskCompletionSource<INetworkGroup>();

                app.CreateNewGroup(capacity, false, g =>
                {
                    if (g == null)
                    {
                        if (showDialogOnError)
                        {
                            Dialog.Get().
                                   RequestInfo("Failed to Create", "Unknown error.",
                                       DialogStyle.Error,          null);
                        }
                    } //should be logged by logging service

                    groupSource.TrySetResult(g);
                });

                return await groupSource.Task;
            }
            else
            {
                Debug.Log($"Can't create any more groups.");
                return null;
            }
        }

        public static async UniTask<INetworkGroup> JoinGroup(long id, string pass, bool showDialogOnError = false)
        {
            var app = AppLayer.AppLayer.Get();
            if (app.CanJoinGroup)
            {
                UniTaskCompletionSource<INetworkGroup> groupSource = new UniTaskCompletionSource<INetworkGroup>();

                app.JoinGroup(id, pass, g =>
                {
                    if (g == null)
                    {
                        if (showDialogOnError)
                        {
                            Dialog.Get().
                                   RequestInfo("Failed to Join", "Either Room Id or Password is incorrect.",
                                       DialogStyle.Error,        null);
                        }
                    } //should be logged by logging service

                    groupSource.TrySetResult(g);
                });

                return await groupSource.Task;
            }
            else
            {
                Debug.Log($"Can't join any more groups.");
                return null;
            }
        }

        public static async UniTask<INetworkGroup> JoinGroup(RoomModel room)
        {
            if (string.IsNullOrEmpty(room?.owner?.id))
            {
                Debug.LogWarning($"Room has no profile attached.");
                return null;
            }

            return await Utils.JoinGroup(room.room_credentials);
        }

        public static async Task<INetworkGroup> JoinGroup(PlatformCredentials roomCreds)
        {
            if (roomCreds == null || string.IsNullOrEmpty(roomCreds.platform_id) ||
                string.IsNullOrEmpty(roomCreds.platform_secret))
            {
                return null;
            }

            if (!long.TryParse(roomCreds.platform_id, out long groupId))
            {
                Debug.Log($"'{roomCreds.platform_id}' is not parseable to type long.");
                return null;
            }

            return await Utils.JoinGroup(groupId, roomCreds.platform_secret);
        }

        public static string GetTableMetaDataKey(int i) => $"Table{i}";

        #endregion

        #region PointerEventData

        public static bool LeftClick(this   PointerEventData evt) => evt.button == PointerEventData.InputButton.Left;
        public static bool RightClick(this  PointerEventData evt) => evt.button == PointerEventData.InputButton.Right;
        public static bool MiddleClick(this PointerEventData evt) => evt.button == PointerEventData.InputButton.Middle;

        #endregion

        public static void SetClipboardText(string text)
        {
            GUIUtility.systemCopyBuffer = text;
        }

        #region KvpExtensions

        public static void Deconstruct<TKey, TValue>(
            this KeyValuePair<TKey, TValue> kvp,
            out  TKey                       key,
            out  TValue                     value)
        {
            key   = kvp.Key;
            value = kvp.Value;
        }

        #endregion
    }

    public class JsonArray<T> : IDebugData
    {
        public List<T> List;
        public string  Json { get; set; }
    }

    [Serializable]
    public struct EmptyResponse
    {
        
    }
}