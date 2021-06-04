using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] private GameObject avatarViewPrefab;
        [SerializeField] private Transform parent;
        [SerializeField] private NetLauncher netLauncher;
        
        private Text _name;
        private GameObject _avatarImage;

        private void Start()
        {
            netLauncher.AddedAvatarPlayer += PopulateLobby;
        }

        private void OnDestroy()
        {
            netLauncher.AddedAvatarPlayer -= PopulateLobby;
        }

        private void PopulateLobby(List<AvatarPlayer> avatarPlayers)
        {
            Debug.Log("Avatar players count " + avatarPlayers.Count);
        }
    }
}