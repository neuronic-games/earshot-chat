using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Networking
{
    public class NetSpawner : NetworkBehaviour
    {
        //[SerializeField] private GameObject avatar;
        private GameObject avatarObject;
        private List<UserAvatar> _avatars = new List<UserAvatar>();
        private Transform _contentParent;

        public void InstantiateAvatar(Transform parent)
        {
           // avatarObject = Instantiate(avatar, Vector3.zero, Quaternion.identity);
            avatarObject.transform.SetParent(parent);
            //InstantiateAvatarServerRpc();
        }

        public void FindAvatars(Transform parent)
        {
            _contentParent = parent;
            _avatars = FindObjectsOfType<UserAvatar>().ToList();
            PopulateLobby();
        }

        [ServerRpc]
        private void PopulateLobbyServerRpc()
        {
            if (IsServer)
            {
                foreach (var avatar in _avatars)
                {
                    avatar.gameObject.transform.SetParent(_contentParent, false);
                }
            }
        }
        
        private void PopulateLobby()
        {
            foreach (var avatar in _avatars)
            {
                if (IsOwner)
                {
                    avatar.gameObject.transform.SetParent(_contentParent);
                }
                
            }
            
        }

        [ServerRpc]
        private void InstantiateAvatarServerRpc()
        {
            avatarObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
        }
    }
}