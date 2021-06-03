using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class AvatarPlayer : NetworkBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    void OnMouseDown()
    {
        if (IsLocalPlayer)
        {
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            offset = gameObject.transform.position -
                     Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                         screenPoint.z));
        }
    }

    void OnMouseDrag()
    {
        if (IsLocalPlayer)
        {
            Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
            transform.position = cursorPosition;
        }
    }
}