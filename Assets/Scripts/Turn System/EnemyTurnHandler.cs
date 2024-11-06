using System.Collections;
using UnityEngine;

public class EnemyTurnHandler : MonoBehaviour
{
    [Header("Turn Settings")]
    public float enemyTurnDuration = 3f;
    public string turnText = "Enemy Turn";
    [SerializeField] UIHandler uiHandler;

    

    public IEnumerator StartTurn()
    {
        
        uiHandler.ShowTurnText(turnText);

        // Example QTE trigger: Start QTE action map for enemy attack phase

        // Uncomment when QTE is ready to be implemented

        
        //yield return StartCoroutine(HandleQTE()); // Handle QTE sequence



        yield return new WaitForSeconds(enemyTurnDuration);
        EndTurn();
    }

    

    private IEnumerator HandleQTE()
    {
        // Placeholder for QTE handling (e.g., pressing WASD in quick succession)
        yield return new WaitForSeconds(2f); // Simulate QTE duration
    }

    private void EndTurn()
    {
        
        Debug.Log("Enemy Turn Ended");
        
        // Notify TurnManager to proceed to the next turn
        
    }
}
