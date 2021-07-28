﻿using System.Linq;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using TMPro;
using UnityEngine;

namespace Networking
{
    public class AvatarPlayer : NetworkBehaviour
    {
        [SerializeField] private AvatarCollection avatarCollection;
        [SerializeField] private SpriteRenderer iconSprite;
        [SerializeField] private SpriteRenderer background;
        
        private NetworkVariableString userName =
            new NetworkVariableString(
                new NetworkVariableSettings() {WritePermission = NetworkVariablePermission.OwnerOnly}, "username");
 
        private NetworkVariableString avatarIconName =
            new NetworkVariableString(
                new NetworkVariableSettings() {WritePermission = NetworkVariablePermission.OwnerOnly}, "avatarname");

        private NetworkVariableColor backGroundColor =
            new NetworkVariableColor(
                new NetworkVariableSettings() {WritePermission = NetworkVariablePermission.OwnerOnly}, Color.cyan);
        
        private TextMeshPro tmp;
        private RaycastHit2D hit;

        public Sprite AvatarSprite => iconSprite.sprite;
        public Color BackgroundColor => background.color;
        
        private void Awake()
        {
            tmp = gameObject.GetComponentInChildren<TextMeshPro>();
        }

        private void Start()
        {
            if (!IsLocalPlayer)
                return;
            var currentUserName = UserDTO.Username;
            userName.Value = currentUserName;
            avatarIconName.Value = UserDTO.AvatarSprite.name;
            backGroundColor.Value = UserDTO.BackgroundColor;
        }

        private void Update()
        {
            if (IsLocalPlayer)
            {
                var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"),0);
                transform.position += move * 5f * Time.deltaTime;

                SetAvatarPlayerPosition();
            }

            SetUserName();
            SetAvatarSprite();
            SetAvatarBackground();
        }

        private void SetUserName()
        {
            tmp.text = userName.Value;
        }

        private void SetAvatarBackground()
        {
            background.color = backGroundColor.Value;
        }

        private void SetAvatarSprite()
        {
            Sprite sprite = avatarCollection.AvatarVariants.FirstOrDefault(a => a.name == avatarIconName.Value);
            iconSprite.sprite = sprite;
        }

        private void SetAvatarPlayerPosition()
        {
            if (Input.GetMouseButtonDown(0))
            {
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            }
        }
    }
}