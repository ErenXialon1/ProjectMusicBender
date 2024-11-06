using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class QTEHandler : MonoBehaviour
{
    [Header("QTE Settings")]
    [SerializeField] PlayerInput playerInput;
    public bool hasHandledInitialInput = true; // Flag to track if input was handled
    [SerializeField] PlayerTurnHandler playerTurnHandler;

    private void Awake()
    {
        
        // Ensure playerInput and the QTE action map exist
        var qteActionMap = playerInput.actions.FindActionMap("QTE", true);
        if (qteActionMap == null)
        {
            Debug.LogError("QTE action map not found in PlayerInput.");
            return;
        }
        

    }
    // Coroutine to subscribe to QTE events after a one-frame delay
    public IEnumerator SubscribeQTEInputEvents()
    {
        // Wait until the end of the frame to ensure everything is set up
        yield return new WaitForEndOfFrame();

        // Subscribe to each QTE direction action with a method that handles input once
        playerInput.actions["QTE_Up"].performed += HandleQTEUp;
        playerInput.actions["QTE_Down"].performed += HandleQTEDown;
        playerInput.actions["QTE_Left"].performed += HandleQTELeft;
        playerInput.actions["QTE_Right"].performed += HandleQTERight;

        
    }

    // Method to unsubscribe from QTE events
    public void UnsubscribeQTEInputEvents()
    {
        // Unsubscribe each direction
        playerInput.actions["QTE_Up"].performed -= HandleQTEUp;
        playerInput.actions["QTE_Down"].performed -= HandleQTEDown;
        playerInput.actions["QTE_Left"].performed -= HandleQTELeft;
        playerInput.actions["QTE_Right"].performed -= HandleQTERight;
    }
    // Each QTE direction handler method
    private void HandleQTEUp(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!hasHandledInitialInput)
        {
            playerTurnHandler.HandleInitialInput("W");
            hasHandledInitialInput = true;
            OnFirstQTEInput();
        }
    }

    private void HandleQTEDown(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!hasHandledInitialInput)
        {
            playerTurnHandler.HandleInitialInput("S");
            hasHandledInitialInput = true;
            OnFirstQTEInput();
        }
    }

    private void HandleQTELeft(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!hasHandledInitialInput)
        {
            playerTurnHandler.HandleInitialInput("A");
            hasHandledInitialInput = true;
            OnFirstQTEInput();
        }
    }

    private void HandleQTERight(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
            if(!hasHandledInitialInput)
        {
            playerTurnHandler.HandleInitialInput("D");
            hasHandledInitialInput = true;
            OnFirstQTEInput();
        }
    }

    // Method to be called after the first input is captured
    private void OnFirstQTEInput()
    {

        playerTurnHandler.canStartTurn = true;
        // Switch to Battle action map after initial input
        ActionMapManager.Instance.ActivateBattleMap();
        
    }

    private void OnDisable()
    {
        UnsubscribeQTEInputEvents();
    }

    
}
