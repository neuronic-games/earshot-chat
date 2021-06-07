using System;
using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using MumbleProto;
using UnityEngine;
using Whoo;

namespace Networking
{
    public class NetSpawner : NetworkBehaviour // todo vz consider to rename this class 
    {
        private List<UserDTO> _userDtos = new List<UserDTO>();

        public List<UserDTO> UserDtos => _userDtos;  // is not being synchronized    
        public NetworkVariableString NameOfNewUser 
            = new NetworkVariableString(new NetworkVariableSettings() { WritePermission = NetworkVariablePermission.Everyone },"username");

        private void OnEnable()
        {
            NameOfNewUser.OnValueChanged += AddToList;
        }

        private void OnDisable()
        {
            NameOfNewUser.OnValueChanged -= AddToList;
        }

        public void ChangeNetworkVarUsername()
        {
           NameOfNewUser.Value = Authentication.UserDTO.Username;
        }


        void AddToList(string p, string n)
        {
            Debug.Log("Next " + n);
            
             _userDtos.Add(new UserDTO(n));
             foreach (var userDTO in _userDtos)
             {
                 Debug.Log("From List: " + userDTO.Username);
             }
             
             
        }

    }
}