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

    /// <summary>
    /// All strapi results are nested up-to a certain depth. After this depth, only the id and not the full data of a field is returned.
    /// This base class is required to mirror this property when deserilizing json.
    /// It will either contain the full object or just its id, and this can be read from an enum property.
    /// </summary>
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

        /// <summary>
        /// This method is used in child classes to easily declare an implicit string operator. 
        /// </summary>
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
    /// Common properties for everything strapi.
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

    /// <summary>
    /// Base model for all strapi COMPONENTS.
    /// </summary>
    [Serializable]
    public abstract class StrapiComponentCommon : StrapiCommon
    {
        public abstract string __component { get; }
    }

    /// <summary>
    /// Base model for all strapi MODEL objects, such as those in collections.
    /// </summary>
    [Serializable]
    public abstract class StrapiModelCommon : StrapiCommon, IDebugData
    {
        [JsonIgnore]
        public string Json { get; set; } = string.Empty;

        [JsonIgnore]
        public abstract string ResourceUrl { get; }

        public virtual async UniTask GetAsync(string jwt = null)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(ResourceUrl))
            {
                if (!string.IsNullOrEmpty(jwt))
                {
                    request.SetRequestHeader("Authorization", $"Bearer {jwt}");
                }

                var response = (await request.SendWebRequest()).downloadHandler.text;
                JsonConvert.PopulateObject(response, this);
                Json = response;
            }
        }

        public virtual async UniTask PutAsync(object updateObj, string jwt = null,
            JsonSerializerSettings settings = null)
        {
            string updateJson = JsonConvert.SerializeObject(updateObj, settings);
            using (UnityWebRequest request = UnityWebRequest.Put(ResourceUrl, updateJson))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                if (!string.IsNullOrEmpty(jwt))
                {
                    request.SetRequestHeader("Authorization", $"Bearer {jwt}");
                }

                string response = (await request.SendWebRequest()).
                                  downloadHandler.text;
                JsonConvert.PopulateObject(response, this, settings);
                Json = response;
            }
        }

        public virtual async UniTask PutAllAsync()
        {
            await PutAsync(this);
        }

        public virtual async UniTask PostAsync(object newObj, string jwt = null, JsonSerializerSettings settings = null)
        {
            var newJson = JsonConvert.SerializeObject(newObj, settings);

            using (UnityWebRequest request = UnityWebRequest.Put(ResourceUrl, newJson))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                request.method = UnityWebRequest.kHttpVerbPOST;
                if (!string.IsNullOrEmpty(jwt))
                {
                    request.SetRequestHeader("Authorization", $"Bearer {jwt}");
                }

                string response = (await request.SendWebRequest()).
                                  downloadHandler.text;
                JsonConvert.PopulateObject(response, this, settings);
                Json = response;
            }
        }
    }

    /// <summary>
    /// Flat, actual data of image.
    /// </summary>
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

    /// <summary>
    /// Common data for user last-modified/created-by entries in other components.
    /// </summary>
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

    /// <summary>
    /// Contains strapi object form of uploaded images, including automatic thumbnails.
    /// </summary>
    [Serializable]
    public class StrapiImage : ImageCommon
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

    #region Components

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

    [Serializable]
    public class Occupant : StrapiComponentCommon
    {
        public Profile profile = null;

        public bool moderator = false;

        public int seat_id = -1;

        public static implicit operator Occupant(string id) => Make<Occupant>(id);
        public override                 string __component  => "runtime.occupant";
    }

    #endregion

    #region Models

    /// <summary>
    /// Room layouts define how read-only properties for various pre-built rooms.
    /// </summary>
    [Serializable]
    public class Layout : StrapiModelCommon
    {
        #region Definitions

        #endregion

        public StrapiImage[] image = null;

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

    /// <summary>
    /// The platform-specific room INSTANCE used for realtime network/voice.
    /// This is the top-level data/control of each room.
    /// </summary>
    [Serializable]
    public class RoomModel : StrapiModelCommon
    {
        public PlatformCredentials room_credentials = null;

        public ZoneInstance[] zone_instances = null;

        public List<Occupant> occupants = null;

        public Layout  layout = null;
        public Profile owner  = null;

        public override string ResourceUrl => Endpoint.Base().CollectionEntry(Collections.Room, id);

        public static implicit operator RoomModel(string id) => Make<RoomModel>(id);

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

        public StrapiImage[] image = null;

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
        public StrapiImage[]   image         = Array.Empty<StrapiImage>();
        public AvatarComponent profileAvatar = null;

        [DefaultValue("")]
        public string platform_unique_id = string.Empty;

        #region Class Definitions / Methods

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

        public static implicit operator Profile(string id) => Make<Profile>(id);

        public override string ResourceUrl => Endpoint.Base().CollectionEntry(Collections.Profile, id);

        #endregion
    }

    [Serializable]
    public class StrapiUser : StrapiModelCommon
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

        public Role role = null;

        public Profile selected_profile = null;

        //TODO -- selected profile  

        public string resetPasswordToken;

        public bool confirmed;

        public bool blocked;

        public static implicit operator StrapiUser(string id) => Make<StrapiUser>(id);
        public override                 string ResourceUrl    => Endpoint.Base().CollectionEntry(Collections.User, id);
    }

    public class Role : StrapiModelCommon
    {
        #region Serialized

        [DefaultValue("")]
        public string description = "";

        [DefaultValue("")]
        public string type = "";

        [DefaultValue("")]
        public new string name = "";

        #endregion

        #region Operators/Props

        public static implicit operator Role(string id)    => Make<Role>(id);
        public override                 string ResourceUrl => Endpoint.Base().CollectionEntry(Collections.Role, id);

        #endregion
    }

    #endregion

    #region Other

    /// <summary>
    /// Authentication response from strapi auth routes (login, register, oauth2).
    /// Contains own user profile and jwt token.
    /// </summary>
    [Serializable]
    public class AuthResponse
    {
        public StrapiUser user;
        public string     jwt;
    }

    [Serializable]
    public class StrapiError
    {
        public class MessageList
        {
            public List<Message> messages;
        }

        public class Message
        {
            public string id;
            public string message;
        }

        public int           statusCode;
        public string        error;
        public MessageList[] message;
    }

    #endregion
}