using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Whoo.Data
{
    #region Commons

    public abstract class ObjectOrId
    {
        [JsonIgnore]
        public Mode mode = Mode.FullObject;

        [DefaultValue("")]
        public string id = string.Empty;

        //when a model is returned as a nested json, e.g. in a relation, the user info is not in "full" but only the string id is provided
        public enum Mode
        {
            FullObject,
            IdOnly
        }

        protected static T Make<T>(string id, T value = default) where T : ObjectOrId, new()
        {
            return new T()
            {
                mode = Mode.IdOnly,
                id   = id
            };
        }
    }

    /// <summary>
    /// Common properties for strapi objects.
    /// </summary>
    [Serializable]
    public abstract class StrapiCommon : ObjectOrId
    {
        [DefaultValue("")]
        public string _id = string.Empty;

        /// <summary>
        /// Means different things depending on the object.
        /// </summary>
        [DefaultValue("")]
        public string name = string.Empty;

        /// <summary>
        /// Timestamp of last update on object.
        /// </summary>
        [DefaultValue("")]
        public string updatedAt = string.Empty;

        /// <summary>
        /// Timestamp of creation of object.
        /// </summary>
        [DefaultValue("")]
        public string createdAt = string.Empty;
    }

    [Serializable]
    public abstract class StrapiComponentCommon : StrapiCommon
    {
        public abstract string __component { get; }
    }

    [Serializable]
    public abstract class StrapiModelCommon : StrapiCommon, IDebugData
    {
        [JsonIgnore]
        public string Json { get; set; } = string.Empty;

        [JsonIgnore]
        public abstract string ResourceUrl { get; }

        public virtual async UniTask GetAsync()
        {
            string endp            = ResourceUrl;
            var    unityWebRequest = await UnityWebRequest.Get(endp).SendWebRequest();
            string response        = unityWebRequest.downloadHandler.text;
            unityWebRequest.Dispose();
            //try-catch
            JsonConvert.PopulateObject(response, this);
            Json = response;
        }

        public virtual async UniTask PutAsync(object updateObj, JsonSerializerSettings settings = null)
        {
            string endp       = ResourceUrl;
            string updateJson = JsonConvert.SerializeObject(updateObj, settings);
            var    webRequest = UnityWebRequest.Put(endp, updateJson);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            string response = (await webRequest.SendWebRequest()).
                              downloadHandler.text;
            webRequest.Dispose();
            JsonConvert.PopulateObject(response, this, settings);
            Json = response;
        }

        public virtual async UniTask PutAllAsync()
        {
            await PutAsync(this);
        }

        public virtual async UniTask PostAsync(object newObj, JsonSerializerSettings settings = null)
        {
            string endp       = ResourceUrl;
            var    newJson    = JsonConvert.SerializeObject(newObj, settings);
            var    webRequest = UnityWebRequest.Put(endp, newJson);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.method = UnityWebRequest.kHttpVerbPOST;
            string response = (await webRequest.SendWebRequest()).
                              downloadHandler.text;
            JsonConvert.PopulateObject(response, this, settings);
            Json = response;
        }
    }

    [Serializable]
    public class ImageCommon
    {
        [DefaultValue("")]
        public string hash = string.Empty;

        [DefaultValue("")]
        public string ext = string.Empty;

        [DefaultValue("")]
        public string mime = string.Empty;

        [DefaultValue(-1)]
        public float size = -1;

        [DefaultValue(-1)]
        public int width = -1;

        [DefaultValue(-1)]
        public int height = -1;

        [DefaultValue("")]
        public string url = string.Empty;
    }

    [Serializable]
    public class UserCommon : ObjectOrId
    {
        //full
        [DefaultValue("")]
        public string _id = string.Empty;

        [DefaultValue("")]
        public string username = string.Empty;

        [DefaultValue("")]
        public string firstname = string.Empty;

        [DefaultValue("")]
        public string lastname = string.Empty;

        [DefaultValue("")]
        public string createdAt = string.Empty;

        [DefaultValue("")]
        public string updatedAt = string.Empty;

        [DefaultValue("")]
        public int __v = 0;

        public static implicit operator UserCommon(string id) => Make<UserCommon>(id);
    }

    [Serializable]
    public class ImageData : ImageCommon
    {
        #region Definitions

        public class Format : ImageCommon
        {
            [DefaultValue("")]
            public string path = string.Empty;
        }

        public class Formats
        {
            public Format thumbnail = null;
            public Format medium    = null;
            public Format small     = null;
        }

        #endregion

        [DefaultValue("")]
        public string id = string.Empty;

        [DefaultValue("")]
        public string _id = string.Empty;

        [DefaultValue("")]
        public string alternativeText = string.Empty;

        [DefaultValue("")]
        public string updated_by = string.Empty;

        [DefaultValue("")]
        public string created_by = string.Empty;

        [DefaultValue("")]
        public string caption = string.Empty;

        public Formats formats = null;

        [DefaultValue("")]
        public string provider = string.Empty;

        public string[] related = null;
    }

    #endregion

    /// <summary>
    /// Room layouts define how read-only properties for various pre-built rooms.
    /// </summary>
    [Serializable]
    public class Layout : StrapiModelCommon
    {
        #region Definitions

        #endregion

        public ImageData[] image = null;

        //room properties
        [DefaultValue(-1)]
        public int capacity = -1;

        public UserCommon created_by = null;
        public UserCommon updated_by = null;

        #region Methods

        public static implicit operator Layout(string id) => Make<Layout>(id);

        public override string ResourceUrl => Endpoint.Base().CollectionEntry(Collections.Layout, id);

        public async UniTask<List<Zone>> GetAllZonesAsync()
        {
            string   endpoint = Endpoint.Base().Collection(Collections.Zone).Equals("layout.id", id);
            string   response = (await UnityWebRequest.Get(endpoint).SendWebRequest()).downloadHandler.text;
            ZoneList list     = new ZoneList();
            JsonConvert.PopulateObject(Utils.WrapJsonArrayWithObject(response, nameof(ZoneList.list)), list);
            return list.list;
        }

        [Serializable]
        private class ZoneList
        {
            public List<Zone> list;
        }

        #endregion
    }

    [Serializable]
    public class PlatformCredentials : StrapiComponentCommon
    {
        [JsonProperty("__component")]
        public override string __component => "platform.credentials";

        [DefaultValue("")]
        public string platform = string.Empty;

        [DefaultValue("")]
        public string platform_id = string.Empty;

        [DefaultValue("")]
        public string platform_secret = string.Empty;

        public const string Platform_Discord = "Discord";
        public const string Platform_Agora   = "Agora";

        public static implicit operator PlatformCredentials(string id) => Make<PlatformCredentials>(id);
    }

    [Serializable]
    public class ZoneInstance : StrapiComponentCommon
    {
        public override string              __component => "platform.zone_instance";
        public          PlatformCredentials credentials = null;
        public          Zone                zone        = null;

        public static implicit operator ZoneInstance(string id) => Make<ZoneInstance>(id);
    }

    /// <summary>
    /// The platform-specific room instance used for realtime network/voice. 
    /// </summary>
    [Serializable]
    public class RoomModel : StrapiModelCommon
    {
        public PlatformCredentials room_credentials = null;

        public ZoneInstance[] zone_instances = null;

        public Layout  layout = null;
        public Profile owner  = null;

        public override string ResourceUrl => Endpoint.Base().CollectionEntry(Collections.Room, id);

        public static implicit operator RoomModel(string id) => Make<RoomModel>(id);

        public async UniTask<List<Occupant>> GetAllOccupantsAsync()
        {
            string       endpoint = Endpoint.Base().Collection(Collections.Occupant).Equals("room.id", id);
            OccupantList list = await Utils.GetJsonObjectAsync<OccupantList>(endpoint, true, nameof(OccupantList.list));
            return list.list;
        }

        [Serializable]
        private class OccupantList : IDebugData
        {
            public string         Json { get; set; } = string.Empty;
            public List<Occupant> list = null;
        }

        public async UniTask EnsureHasZoneInstancedAsync()
        {
            var allZones = await layout.GetAllZonesAsync();
            if (zone_instances == null || zone_instances.Length < allZones.Count)
            {
                var updateObj = new
                {
                    zone_instances = allZones.Select(z => new {zone = z.id}).ToArray()
                };
                await this.PutAsync(updateObj);
            }

            return;
        }
    }

    /// <summary>
    /// A zone is, for example, a table in a room. This is a readonly value as well, completing the layout.
    /// </summary>
    [Serializable]
    public class Zone : StrapiModelCommon
    {
        [Serializable]
        public class Seat : StrapiComponentCommon
        {
            [DefaultValue(-1)]
            public int x = -1;

            [DefaultValue(-1)]
            public int y = -1;

            public override string __component => "layout.seat";

            public static implicit operator Seat(string id) => Make<Seat>(id);

            public static explicit operator Vector2(Seat s) => new Vector2(s.x, s.y);
        }

        public bool proximity;

        public UserCommon created_by = null;
        public UserCommon updated_by = null;

        public ImageData[] image = null;

        [DefaultValue(-1)]
        public int x = -1;

        [DefaultValue(-1)]
        public int y = -1;

        [DefaultValue(-1)]
        public int width = -1;

        [DefaultValue(-1)]
        public int height = -1;

        [JsonProperty("visible_seats")]
        public bool visibleSeats;

        [JsonProperty("seatz")]
        public List<Seat> seats = null;

        public Layout layout = null;

        #region Methods

        public static implicit operator Zone(string id)    => Make<Zone>(id);
        public override                 string ResourceUrl => Endpoint.Base().CollectionEntry(Collections.Zone, id);

        #endregion
    }

    /// <summary>
    /// User profiles have extra information about a user.
    /// </summary>
    [Serializable]
    public class Profile : StrapiModelCommon
    {
        [Serializable]
        public class AvatarComponent : StrapiComponentCommon
        {
            public bool male;
            public int  hair;
            public int  faceA;
            public int  faceB;
            public int  torso;

            public override string __component => "profile.avatar";
        }

        public                          ImageData[]     image         = Array.Empty<ImageData>();
        public                          AvatarComponent profileAvatar = null;
        public static implicit operator Profile(string id) => Make<Profile>(id);

        public override string ResourceUrl => Endpoint.Base().CollectionEntry(Collections.Profile, id);
    }

    [Serializable]
    public class User : StrapiModelCommon
    {
        public const string PLATFORM_DISCORD = "Discord";
        public const string PLATFORM_AGORA   = "Agora";
        public const string PLATFORM_DOLBY   = "Dolby";

        [DefaultValue(null)]
        public string platform = null;

        [DefaultValue(null)]
        public string platform_id = null;

        [DefaultValue(null)]
        public string username = null;

        [DefaultValue(null)]
        public string email = null;

        [DefaultValue(null)]
        public string password = null;

        public bool confirmed;
        public bool blocked;

        public static implicit operator User(string id)    => Make<User>(id);
        public override                 string ResourceUrl => Endpoint.Base().CollectionEntry(Collections.User, id);
    }

    [Serializable]
    public class Occupant : StrapiModelCommon
    {
        [DefaultValue("")]
        public string room = string.Empty;

        public Profile profile = null;

        public bool moderator = false;

        [DefaultValue("")]
        public string color = string.Empty;

        public static implicit operator Occupant(string id) => Make<Occupant>(id);
        public override                 string ResourceUrl => Endpoint.Base().CollectionEntry(Collections.Occupant, id);
    }
}