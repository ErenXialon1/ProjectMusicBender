using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionMapManager : MonoBehaviour
{
    public static ActionMapManager Instance { get; private set; }
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Player playerScript;
    [SerializeField] PlayerTurnHandler playerTurnHandler;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found on ActionMapManager. Please attach PlayerInput.");
        }

        // Switch to QTE map initially
        ActivateQTEMap(); // TO DO What if player changes scene

        
        
    }


    
   

    public void ActivateBattleMap()
    {
        playerInput.enabled = true;
        SwitchActionMap("Battle");
    }

    public void ActivateQTEMap()
    {
        playerInput.enabled = true;
        SwitchActionMap("QTE");
    }

    public void ActivateOverworldMap()
    {
        playerInput.enabled = true;
        SwitchActionMap("Overworld");
    }
    private void UnsubscribeAllCurrentMapEvents()
    {
        
            playerScript.UnsubscribeInputEvents(); // Unsubscribe battle events
        
        
        
        
            //TO DO unsubscribe overworld action map
        
    }

    private void SubscribeToCurrentMapEvents()
    {
        if (playerInput.currentActionMap.name == "Battle")
        {
            StartCoroutine(playerScript.SubscribeInputEvents()); // Unsubscribe battle events
        }
       
        else if(playerInput.currentActionMap.name == "Overworld")
        {
            //TO DO subscribe overworld action map
        }
    }
    public void SwitchActionMap(string mapName)
    {
        UnsubscribeAllCurrentMapEvents(); // Unsubscribe from current map events
        
            playerInput.SwitchCurrentActionMap(mapName);
            Debug.Log($"Switched to action map: {mapName}");
            SubscribeToCurrentMapEvents(); // Subscribe to new map events
       
    }

    
}
