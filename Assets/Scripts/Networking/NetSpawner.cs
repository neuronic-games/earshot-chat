using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

namespace Networking
{
    public class NetSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject avatar;
        
        [ServerRpc]
        public void InstantiateAvatarServerRpc(Transform parent)
        {
            GameObject go = Instantiate(avatar, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(parent);
            go.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
        }
    }
}