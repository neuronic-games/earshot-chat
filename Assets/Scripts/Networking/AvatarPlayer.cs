using System;
using MLAPI;
using UnityEngine;

namespace Networking
{
    public class AvatarPlayer : NetworkBehaviour
    {
        private void Update()
        {
            if (!IsLocalPlayer) return;
            var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"),0);
            transform.position += move * 5f * Time.deltaTime;
        }
    }
}