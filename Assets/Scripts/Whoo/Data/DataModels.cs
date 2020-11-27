using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Whoo.Data
{
    #region Commons

    /// <summary>
    /// Common properties for strapi objects.
    /// </summary>
    [Serializable]
    public abstract class StrapiCommon
    {
        //track changes
        /// <summary>
        /// Unique id assigned by strapi to the object.
        /// </summary>
        public string id;

        public string _id;

        /// <summary>
        /// Means different things depending on the object.
        /// </summary>
        public string name;

        /// <summary>
        /// Timestamp of last update on object.
        /// </summary>
        public string updatedAt;

        /// <summary>
        /// Timestamp of creation of object.
        /// </summary>
        public string createdAt;

        public abstract string EndPoint { get; }

        public virtual async UniTaskVoid GetAsync()
        {
            var endp = EndPoint;
            string response = (await UnityWebRequest.Get(endp).SendWebRequest()).
                              downloadHandler.text;
            //try-catch
            JsonUtility.FromJsonOverwrite(response, this);
        }

        public virtual async UniTaskVoid PutAsync(object updateObj)
        {
            var endp = EndPoint;
            var response = (await UnityWebRequest.Put(endp, JsonUtility.ToJson(updateObj)).SendWebRequest()).
                           downloadHandler.text;
            JsonUtility.FromJsonOverwrite(response, this);
        }
    }

    [Serializable]
    public class ImageCommon
    {
        public string hash;

        public string ext;

        public string mime;

        public float size;

        public int width;

        public int height;

        public string url;
    }

    [Serializable]
    public class UserCommon
    {
        public string _id;
        public string username;
        public string firstname;
        public string lastname;
        public string createdAt;
        public string updatedAt;
        public int    __v;
        public string id;
    }

    #endregion

    [Serializable]
    public class Layout : StrapiCommon
    {
        #region Definitions

        [Serializable]
        public class ImageData : ImageCommon
        {
            #region Definitions

            public class Format : ImageCommon
            {
                public string path;
            }

            public class Formats
            {
                public Format thumbnail;
                public Format medium;
                public Format small;
            }

            #endregion

            public string id;

            public string _id;

            public string alternativeText;

            public string updated_by;

            public string created_by;

            public string caption;

            public Formats formats;

            public string provider;

            public string[] related;
        }

        #endregion

        public ImageData image;

        //room properties
        public int        capacity;
        public UserCommon created_by;
        public UserCommon updated_by;

        #region Methods

        public override string EndPoint => StrapiEndpoints.LayoutEndpoint(id);

        #endregion
    }

    /// <summary>
    /// The platform-specific room used for realtime network/voice.
    /// </summary>
    [Serializable]
    public class Room : StrapiCommon
    {
        public const string Platform_Discord = "Discord";
        public const string Platform_Agora   = "Agora";

        public string platform;
        public string platform_id;
        public string platform_secret;

        public Layout layout;

        public override string EndPoint => StrapiEndpoints.RoomEndpoint(id);

        public async UniTask<List<Zone>> GetAllZonesAsync()
        {
            string   endpoint = StrapiEndpoints.AllZones(id);
            string   response = (await UnityWebRequest.Get(id).SendWebRequest()).downloadHandler.text;
            ZoneList list     = new ZoneList();
            JsonUtility.FromJsonOverwrite(response, list);
            return list.list;
        }

        [Serializable]
        private class ZoneList
        {
            public List<Zone> list;
        }
    }

    [Serializable]
    public class Zone : StrapiCommon
    {
        public bool proximity;

        public UserCommon created_by;
        public UserCommon updated_by;

        public Layout.ImageData image;

        public int x;
        public int y;
        public int width;
        public int height;

        public string seats;

        public Layout layout;

        #region Methods

        public override string EndPoint => StrapiEndpoints.ZoneEndpoint(id);

        #endregion
    }

    [Serializable]
    public class Profile : StrapiCommon
    {
        [Serializable]
        public class Avatar
        {
            public bool male;
            public int  hair;
            public int  faceA;
            public int  faceB;
            public int  torso;
        }

        public Avatar avatar;
    }
}