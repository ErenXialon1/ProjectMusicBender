using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTurnHandler : MonoBehaviour
{
    [Header("Turn Settings")]
    public float turnDuration = 10f; // Default time for the player turn
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Player playerScript; // Reference to Player script
    public string turnText = "Player Turn";
    
    public bool canStartTurn = false; // New variable to track if turn can start
    private float remainingTime;
    [SerializeField] UIHandler uiHandler;
    
    

    

    
    
    /// <summary>
    /// Starts the player's turn and listens for any button press to begin.
    /// </summary>
    public IEnumerator StartTurn()
    {
        
        canStartTurn = false;
        remainingTime = turnDuration;
        
        // Show "Player Turn" text
        uiHandler.ShowTurnText(turnText);

        // Wait until any button is pressed to start the turn
        yield return new WaitUntil(() => canStartTurn);

        



        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            uiHandler.UpdateTimer(remainingTime); // Update timer UI
            yield return null;
        }
       
        
        // Finalize player turn when time runs out
        EndTurn();
        yield return new WaitUntil(() => playerScript.skillsWaitingForExecute.Count < 5);
        float lastSkillExecutionTime = playerScript.skillsWaitingForExecute.Last().skillAnimationTime;
        yield return new WaitUntil(() => playerScript.skillsWaitingForExecute.Count == 0);
        yield return new WaitForSeconds(lastSkillExecutionTime);
    }

    private void EndTurn()
    {
        canStartTurn = false;
        playerScript.FinalizeTurn(); // Automatically confirm remaining inputs
        ActionMapManager.Instance.ActivateQTEMap();
    }
    
    public void HandleInitialInput(string direction)
    {
        // Add the initial directional input to recentInputs
        playerScript.AddInput(direction);
        CustomLogger.Log(direction);
    }
    

}
