using System.Collections.Generic;
using MLAPI;

namespace Networking
{
    public class NetSpawner : NetworkBehaviour // todo vz consider to rename this class 
    {
        private List<UserDTO> _currentUsers = new List<UserDTO>();

        public List<UserDTO> CurrentUsers => _currentUsers;  

    }
}