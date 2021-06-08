using Discord;

namespace Networking
{
    public class UserDTO
    {
        public string Username { get; private set; }

        public UserDTO(string username)
        {
            Username = username;
        }
    }
}