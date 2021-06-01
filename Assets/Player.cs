using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject clapIcon;
    private CharacterController cc;
    private GameCanvas gameCanvas;

    private NetworkVariableBool claping =
        new NetworkVariableBool(new NetworkVariableSettings {WritePermission = NetworkVariablePermission.OwnerOnly},
            false);

    private void Awake()
    {
        gameCanvas = FindObjectOfType<GameCanvas>();
        cc = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            MovePlayer();
        }

        clapIcon.SetActive(claping.Value); 
    }
    
    private void OnEnable()
    {
        gameCanvas.Clap += ToggleClapping;
    }

    private void OnDisable()
    {
        gameCanvas.Clap -= ToggleClapping;
    }
    
    

    void MovePlayer()
    {
        var move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = Vector3.ClampMagnitude(move, 1f);
        cc.SimpleMove(move * 5f);
    }

    private void ToggleClapping(bool playerIsClaping)
    {
        if (IsLocalPlayer)
        {
            claping.Value = playerIsClaping;
        }

        if (playerIsClaping)
        {
            StartCoroutine(TurnClapIconOff());
            // start claping animation
        }
    }

    IEnumerator TurnClapIconOff()
    {
        yield return new WaitForSeconds(10f);
        claping.Value = false;
        clapIcon.SetActive(claping.Value);
    }
}
