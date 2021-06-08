using System;
using System.Collections.Generic;
using MLAPI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] private GameObject avatarViewPrefab;
        [SerializeField] private Transform parent;

        private AvatarPlayer[] _players;
        private NetSpawner _netSpawner;
        private int _count = 0;

        private void Start()
        {
            _netSpawner = FindObjectOfType<NetSpawner>();
        }

        private void Update()
        {
            var newCount = _netSpawner.GetConnectedClientsCount();
            
            if (newCount != _count)
            {
                Debug.Log("NewCount " + newCount);
                ClearLobby();
                PopulateLobby();
                _count = newCount;
            }
        }

        private void ClearLobby()
        {
            var avatars = FindObjectsOfType<AvatarView>();
            foreach (var avatarView in avatars)
            {
                Destroy(avatarView.gameObject);
            }
        }

        private void PopulateLobby()
        {
            _players = FindObjectsOfType<AvatarPlayer>();
            foreach (var player in _players)
            {
                var avatarViewObject = Instantiate(avatarViewPrefab, Vector3.zero, Quaternion.identity);
                avatarViewObject.transform.SetParent(parent);

                var tmp = avatarViewObject.GetComponentInChildren<TextMeshProUGUI>();
                tmp.text = player.UserName;
            }
        }
    }
}