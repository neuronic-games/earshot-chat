using System;
using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

public class GameCanvas : NetworkBehaviour
{
    public Action Clap;

    


    public void HandleClap()
    {
        Clap?.Invoke();
    }

    
}
