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

        private void Start()
        {
            PopulateLobby();
        }

        private void PopulateLobby()
        {
            Debug.Log("Avatar players count " + NetLauncher.AvatarPlayers.Count);
            var avatarViewObject = Instantiate(avatarViewPrefab, Vector3.zero, Quaternion.identity);
           
            avatarViewObject.transform.SetParent(parent); 
            var tmp = avatarViewObject.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = UserDTO.Username;
        }
    }
}