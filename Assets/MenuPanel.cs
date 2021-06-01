using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class MenuPanel : MonoBehaviour
{
    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        gameObject.SetActive(false);
    }
    
    public void Join()
    {
        NetworkManager.Singleton.StartClient();
        gameObject.SetActive(false);
    }
}
