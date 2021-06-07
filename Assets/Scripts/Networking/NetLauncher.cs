using System;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

namespace Networking
{
    public class NetLauncher : NetworkBehaviour
    {
        public static readonly List<AvatarPlayer> AvatarPlayers = new List<AvatarPlayer>();
    
        private void Awake()
        {
            throw new NotImplementedException();
        }

        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            //SetPlayerInactive();
            AddPlayerToList();
        }

        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
            //SetPlayerInactive();
            AddPlayerToList();
        }

        private void SetPlayerInactive()
        {
            if (IsOwner)
            {
                var clientId = NetworkManager.Singleton.LocalClientId;
                NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject.SetActive(false);
            }
        }

        private void AddPlayerToList()
        {
            var clientId = NetworkManager.Singleton.LocalClientId;
            var avatarPlayerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject.GetComponent<AvatarPlayer>();
            AvatarPlayers.Add(avatarPlayerObject);
        }
    }
}