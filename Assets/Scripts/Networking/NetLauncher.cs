using System;
using System.Collections.Generic;
using MLAPI;

namespace Networking
{
    public class NetLauncher : NetworkBehaviour
    {
        private static readonly List<AvatarPlayer> _avatarPlayers = new List<AvatarPlayer>();

        public Action<List<AvatarPlayer>> AddedAvatarPlayer;
        
        public static void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            //SetPlayerInactive();
            //AddPlayerToList();
        }

        public static void StartClient()
        {
            NetworkManager.Singleton.StartClient();
            //SetPlayerInactive();
            //AddPlayerToList();
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
            _avatarPlayers.Add(avatarPlayerObject);
            AddedAvatarPlayer?.Invoke(_avatarPlayers);
        }
    }
}