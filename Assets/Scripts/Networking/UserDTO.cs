using Discord;

namespace Networking
{
    public class UserDTO
    {
        public string Username { get; set; }

        public UserDTO(string username)
        {
            Username = username;
        }
    }
}