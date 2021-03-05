using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using UnityEngine;

namespace Whoo.Data
{
    public static class Endpoint
    {
        #region Static Caching

        private static ConcurrentBag<StringBuilder> _cachedSb = new ConcurrentBag<StringBuilder>();

        private static StringBuilder GetSb()
        {
            if (_cachedSb.TryTake(out var sb)) return sb;
            return new StringBuilder();
        }

        private static void ReturnSb(StringBuilder sb)
        {
            sb.Clear();
            _cachedSb.Add(sb);
        }

        #endregion

        private const string ApiBase              = ApiBaseNoEndingSlash + "/";
        private const string ApiBaseNoEndingSlash = "https://strapi.meetwhoo.com";

        public static StrapiEndpointBase Base(bool prependUriBase = true)
        {
            StrapiEndpointBase b = new StrapiEndpointBase()
            {
                Sb = GetSb()
            };
            b.Sb.Append(prependUriBase ? ApiBase : "/");
            return b;
        }

        public static string Upload(string url)
        {
            if (!url.StartsWith("/"))
            {
                return ApiBase + url;
            }
            else
            {
                return ApiBaseNoEndingSlash + url;
            }
        }

        public struct StrapiEndpointBase
        {
            public StringBuilder Sb;

            public StrapiEndpointQueryable Collection(Collections collection)
            {
                Sb.Append(collection);
                return this;
            }

            public StrapiEndpointBuilt CollectionEntry(Collections collection, string entryId)
            {
                Sb.Append(collection);
                Sb.Append('/');
                Sb.Append(entryId);
                return this;
            }

            public override string ToString()
            {
                return this;
            }

            public static implicit operator string(StrapiEndpointBase b)
            {
                return (StrapiEndpointBuilt) b;
            }
        }

        public struct StrapiEndpointQueryable
        {
            #region Internals

            public StringBuilder Sb;
            public bool          FirstQueryParam;

            public static implicit operator StrapiEndpointQueryable(StrapiEndpointBase e)
            {
                return new StrapiEndpointQueryable()
                {
                    Sb              = e.Sb,
                    FirstQueryParam = true
                };
            }

            public string Build()
            {
                return ((StrapiEndpointBuilt) this).Build();
            }

            public override string ToString() => (string) this;

            public static implicit operator string(StrapiEndpointQueryable q) => q.Build();

            private StrapiEndpointQueryable AppendParam(string param)
            {
                Sb.Append(GetFirstChar());
                FirstQueryParam = false;
                Sb.Append(param);
                return this;
            }

            private char GetFirstChar()
            {
                return FirstQueryParam ? '?' : '&';
            }

            #endregion

            public StrapiEndpointQueryable Limit(int limit)
            {
                return AppendParam($"_limit={limit}");
            }

            public StrapiEndpointQueryable Equals(string key, string value)
            {
                return AppendParam($"{key}={value}");
            }

            private static Dictionary<string, string> _expressionCache = new Dictionary<string, string>();

            public StrapiEndpointQueryable Equals<P, T>(Expression<Func<P, T>> e, T value)
            {
                string body = ((MemberExpression) e.Body).ToString();
                if (!_expressionCache.TryGetValue(body, out string key))
                {
                    ParameterExpression param = e.Parameters[0];
                    if (body.StartsWith(param.Name))
                    {
                        int index = body.IndexOf('.');
                        key = body.Substring(index + 1);
                    }
                    else
                    {
                        key = body;
                    }

                    _expressionCache[body] = key;
                }

                return AppendParam($"{key}={value}");
            }

            public StrapiEndpointQueryable All()
            {
                return Limit(-1);
            }
        }

        public struct StrapiEndpointBuilt
        {
            #region Internals

            public StringBuilder Sb;

            public static implicit operator StrapiEndpointBuilt(StrapiEndpointBase e)
            {
                return new StrapiEndpointBuilt()
                {
                    Sb = e.Sb
                };
            }

            public static implicit operator StrapiEndpointBuilt(StrapiEndpointQueryable e)
            {
                return new StrapiEndpointBuilt()
                {
                    Sb = e.Sb
                };
            }

            public override string ToString() => (string) this;

            public static implicit operator string(StrapiEndpointBuilt b) => b.Build();

            #endregion

            public string Build()
            {
                string result = Sb.ToString();
                ReturnSb(Sb);
                return result;
            }
        }
    }

    //enum struct
    public struct Collections
    {
        public static readonly Collections Room    = new Collections("rooms");
        public static readonly Collections Layout  = new Collections("layouts");
        public static readonly Collections Zone    = new Collections("zones");
        public static readonly Collections Profile = new Collections("profiles");
        public static readonly Collections User    = new Collections("users");
        public static readonly Collections Role    = new Collections("role");

        #region Internals

        private string _apiKey;

        private Collections(string apiKey)
        {
            _apiKey = apiKey;
        }

        public static implicit operator string(Collections c) => c._apiKey;

        #endregion
    }

    /*
    public static class StrapiEndpoints
    {
        public static string Base(string endpoint) => endpoint.StartsWith("/")
            ? $"https://strapi.meetwhoo.com{endpoint}"
            : $"https://strapi.meetwhoo.com/{endpoint}";

        public static string RoomEndpoint(string roomid) => Base($"/rooms/{roomid}");

        public static string ZoneEndpoint(string    zoneid)    => Base($"/zones/{zoneid}");
        public static string ProfileEndpoint(string profileid) => Base($"/profiles/{profileid}");

        public static string LayoutEndpoint(string id) => Base($"/layouts/{id}");

        public static string AllZones(string layoutid) => Base($"/zones?layout.id={layoutid}");

        public static string UserEndpoint(string id) => Base($"/users/{id}");

        public static string OccupantEndpoint(string id) => Base($"/room-occupants/{id}");

        public static string AllOccupants(string roomId) => Base($"/room-occupants?room.id={roomId}");

        public static string AllLayouts() => Base("layouts?_limit=-1");

        //todo -- support pagination
        public static string AllRooms(string profileId) => Base($"/rooms?owner.id={profileId}");
    }*/
}