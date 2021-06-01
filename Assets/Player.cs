using System;
using System.Collections;
using System.Collections.Generic;
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
        claping.OnValueChanged += ClapingChanged;
    }

    private void OnDisable()
    {
        gameCanvas.Clap -= ToggleClapping;
        claping.OnValueChanged -= ClapingChanged;
    }
    
    private void ClapingChanged(bool oldValue, bool newValue)
    {
        if (IsLocalPlayer)
        {
            
        }
    }

    void MovePlayer()
    {
        var move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        move = Vector3.ClampMagnitude(move, 1f);
        cc.SimpleMove(move * 5f);
    }

    private void ToggleClapping()
    {
        if (IsLocalPlayer)
        {
            claping.Value = true;
        }
    }
}
