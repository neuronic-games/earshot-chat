using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Networking
{
    public class NetSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject avatar;
        private Transform incomeParent;

        public void InstantiateAvatar(Transform parent)
        {
            incomeParent = parent;
            InstantiateAvatarServerRpc();
        }
        
        [ServerRpc]
        private void InstantiateAvatarServerRpc()
        {
            GameObject go = Instantiate(avatar, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(incomeParent);
            go.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
        }
    }
}