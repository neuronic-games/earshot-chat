using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class LobbyManager : MonoBehaviour
    {
        [SerializeField] private GameObject avatarViewPrefab;
        [SerializeField] private Transform parent;
        
        private Text _name;
        private GameObject _avatarImage;
        private NetSpawner _netSpawner;

        private void Start()
        {
            _netSpawner = FindObjectOfType<NetSpawner>();
            PopulateLobby();
        }

        private void PopulateLobby()
        {
            Debug.Log(_netSpawner.CurrentUsers.Count);
            foreach (var userDTO in _netSpawner.CurrentUsers)
            {
                Debug.Log(userDTO.Username);
                var avatarViewObject = Instantiate(avatarViewPrefab, Vector3.zero, Quaternion.identity);

                avatarViewObject.transform.SetParent(parent);
                var tmp = avatarViewObject.GetComponentInChildren<TextMeshProUGUI>();
                tmp.text = userDTO.Username;
            }
            
            
            
            
            
        }
    }
}