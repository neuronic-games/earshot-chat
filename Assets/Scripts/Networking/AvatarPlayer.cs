using System;
using MLAPI;
using MLAPI.NetworkVariable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Whoo;

namespace Networking
{
    public class AvatarPlayer : NetworkBehaviour
    {
        private NetworkVariableString userName =
            new NetworkVariableString(
                new NetworkVariableSettings() {WritePermission = NetworkVariablePermission.OwnerOnly}, "username");

        private TextMeshPro tmp;

        private void Awake()
        {
            tmp = gameObject.GetComponentInChildren<TextMeshPro>();
        }

        private void Start()
        {
            if (!IsLocalPlayer)
                return;
            var currentUserName = Authentication.UserDTO.Username;
            userName.Value = currentUserName;
            
        }

        private void Update()
        {
            if (IsLocalPlayer)
            {
                var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"),0);
                transform.position += move * 5f * Time.deltaTime;
            }

            tmp.text = userName.Value;
        }
    }
}