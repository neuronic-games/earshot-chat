using System.IO;

namespace Whoo.Data
{
    public static class StrapiEndpoints
    {
        public static string Base(string         endpoint) => endpoint.StartsWith("/") ? $"https://strapi.meetwhoo.com{endpoint}" : $"https://strapi.meetwhoo.com/{endpoint}";
        public static string RoomEndpoint(string roomid)   => Base($"/rooms/{roomid}");

        public static string ZoneEndpoint(string zoneid) => Base($"/zones/{zoneid}");
    }
}