using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;


public class Player : MonoBehaviour
{
    public float playerHp = 100;
    public List<GameObject> playerAttacks = new List<GameObject>();
    public List<GameObject> playerInputtedAttacks = new List<GameObject>();
    public PlayerInput playerInput;
    public UpdatePlayerAttacks updatePlayerAttacksScript;
    

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        updatePlayerAttacksScript = GameObject.FindFirstObjectByType<UpdatePlayerAttacks>().GetComponent<UpdatePlayerAttacks>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AttackUpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            playerInputtedAttacks.Add(playerAttacks[0]);
            updatePlayerAttacksScript.spriteBlocks.Add(playerAttacks[0].gameObject);
           
        }
    }
    public void AttackRightInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            playerInputtedAttacks.Add(playerAttacks[1]);

        }
    }
    public void AttackLeftInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            playerInputtedAttacks.Add(playerAttacks[2]);

        }
    }
    public void AttackDownInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerInputtedAttacks.Add(playerAttacks[3]);


        }

    }
    public void AttackInputClear(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerInputtedAttacks.Clear();


        }
        
    }
    
    public void AttackConfirmInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

            playerInputtedAttacks.Clear();

        }
    }
    public void CreateAttack()
    {

    }
}
