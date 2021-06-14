using System;
using System.Collections;
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
        //private NetSpawner _netSpawner;
        private int _count = 0;
        float _startTime = 0f;
        
        private void Start()
        {
            //_netSpawner = FindObjectOfType<NetSpawner>();
        }
        
        
        private void Update()
        {
            var newCount = GetAvatarPlayersCount();

            if (newCount != _count)
            {
                Debug.Log("NewCount " + newCount);
                StartCoroutine(UpdateLobby());
                _count = newCount;
            }
        }

        IEnumerator UpdateLobby()
        {
            yield return new WaitForSeconds(2f);
            ClearLobby();
            PopulateLobby();
        }

        
        private int GetAvatarPlayersCount()
        {           
            _players = FindObjectsOfType<AvatarPlayer>();
            return _players.Length;
        }

        private void ClearLobby()
        {
            var avatars = FindObjectsOfType<AvatarViewInLobby>();
            foreach (var avatarView in avatars)
            {
                Destroy(avatarView.gameObject);
            }
        }

        private void PopulateLobby()
        {
            foreach (var player in _players)
            {
                var avatarViewObject = Instantiate(avatarViewPrefab, Vector3.zero, Quaternion.identity);
                avatarViewObject.transform.SetParent(parent);

                var playerName = GetPlayerName(player.gameObject);
                Debug.Log(playerName);
                 var tmp = avatarViewObject.GetComponentInChildren<TextMeshProUGUI>();
                tmp.text = playerName;

                var icon = GetPlayerIcon(player);
                var image = avatarViewObject.GetComponent<AvatarViewInLobby>().IconImage;
                image.sprite = icon.sprite;
            }
        }

        private SpriteRenderer GetPlayerIcon(AvatarPlayer player)
        {
            return null;
        }

        private string GetPlayerName(GameObject player)
        {
            return player.GetComponentInChildren<TextMeshPro>().text;
        }
    }
}