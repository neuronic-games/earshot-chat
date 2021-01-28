using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AppLayer.NetworkGroups;
using Cysharp.Threading.Tasks;
using Dialogs;
using Newtonsoft.Json;
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

        public static TimeSpan EpochToNowSpan => (DateTime.UtcNow - new DateTime(1970, 1, 1));

        public static double EpochToNowSeconds => EpochToNowSpan.TotalSeconds;

        #endregion

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

        private static JsonSerializerSettings _strapiModelSerializerSettings = new JsonSerializerSettings()
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling    = NullValueHandling.Ignore
        };

        public static JsonSerializerSettings StrapiModelSerializationDefaults() => _strapiModelSerializerSettings;

        public static string WrapJsonArrayWithObject(string json, string key) => $"{{ \"{key}\" : {json} }}";

        public static async UniTask<T> GetJsonObjectAsync<T>(string url, bool wrap = false, string key = null)
            where T : new()
        {
            string response = (await UnityWebRequest.Get(url).SendWebRequest()).downloadHandler.text;
            T      result   = new T();
            JsonConvert.PopulateObject(wrap ? WrapJsonArrayWithObject(response, key) : response, result);
            if (result is IDebugData debug) debug.Json = response;
            return result;
        }

        #endregion

        #region Strapi

        public static async UniTask<Texture> LoadPossibleWhooImage(string url, bool forceReloadCache = false,
            bool                                                          cancellable = false,
            CancellationToken                                             token       = default)
        {
            if (string.IsNullOrEmpty(url)) return null;
            return await LoadImage(url.StartsWith("/uploads/") ? Endpoint.Upload(url) : url, forceReloadCache,
                cancellable,                                                                 token);
        }

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

        private static void SubmitCachedImage(string url, Texture2D texture2D)
        {
            _imageCache[url] = texture2D;
        }

        private static Dictionary<string, Texture2D> _imageCache = new Dictionary<string, Texture2D>();

        private static bool TryGetCachedImage(string url, out Texture2D texture2D)
        {
            return _imageCache.TryGetValue(url, out texture2D);
        }

        #endregion

        #endregion

        #region UI

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

        public class TimedLock
        {
            private readonly SemaphoreSlim toLock;

            public TimedLock()
            {
                toLock = new SemaphoreSlim(1, 1);
            }

            public async Task<LockReleaser> Lock(TimeSpan timeout)
            {
                if (await toLock.WaitAsync(timeout))
                {
                    return new LockReleaser(toLock);
                }

                throw new TimeoutException();
            }

            public struct LockReleaser : IDisposable
            {
                private readonly SemaphoreSlim toRelease;

                public LockReleaser(SemaphoreSlim toRelease)
                {
                    this.toRelease = toRelease;
                }

                public void Dispose()
                {
                    toRelease.Release();
                }
            }
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
}