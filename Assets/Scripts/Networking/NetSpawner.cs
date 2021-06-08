using System;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

namespace Networking
{
    public class NetSpawner : NetworkBehaviour // todo vz consider to rename this class 
    {

        public int GetConnectedClientsCount()
        {
            return NetworkManager.ConnectedClientsList.Count;
        }
    }
}