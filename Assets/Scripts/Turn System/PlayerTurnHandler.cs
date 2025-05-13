
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTurnHandler : MonoBehaviour
{
    [Header("Turn Settings")]
    public float turnDuration; // Default time for the player turn
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Player playerScript; // Reference to Player script
    [Header("Enemy References")]
    [Tooltip("List of enemies participating in the turn.")]
    [SerializeField] private List<Enemy> enemies;
    public string turnText = "Player Turn";
    
    public bool canStartTurn = false; // New variable to track if turn can start
    private float remainingTime;
    [SerializeField] UIHandler uiHandler;
    [SerializeField] EnemyTurnHandler enemyTurnHandler;
    [SerializeField] private RuneRockPool runeRockPool;
    private RuneRock currentRuneRock; // Holds the active RuneRock





    private void Start()
    {
        // Initialize the list of enemies once at the start
        enemies = new List<Enemy>(FindObjectsByType<Enemy>(FindObjectsSortMode.None));
    }

    /// <summary>
    /// Starts the player's turn and listens for any button press to begin.
    /// </summary>
    public IEnumerator StartTurn()
    {
        // Update player's available skills
        UpdatePlayerAvailableSkills();
        canStartTurn = false;
        remainingTime = enemyTurnHandler.CalculateTurnDuration(enemies);
        
        // Show "Player Turn" text
        uiHandler.ShowTurnText(turnText);
        

        // Wait until any button is pressed to start the turn
        yield return new WaitUntil(() => canStartTurn);
        currentRuneRock = runeRockPool.GetFromPool().GetComponent<RuneRock>();
        currentRuneRock.transform.position = transform.position;




        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            uiHandler.UpdateTimer(remainingTime); // Update timer UI
            yield return null;
        }
       
        
        // Finalize player turn when time runs out
        EndTurn();
        
    }
    private void UpdatePlayerAvailableSkills()
    {
        // Step 1: Reset the player's available skills
        playerScript.ResetAvailableSkillList();

        // Step 2: Iterate through the pre-cached list of enemies
        foreach (var enemy in enemies)
        {
            var availableSkills = enemy.GetAvailableSkills();
            if (availableSkills == null || availableSkills.Count == 0)
            {
                Debug.LogWarning($"Enemy {enemy.name} has no available skills to add.");
                continue;
            }

            foreach (var skill in availableSkills)
            {
                if (skill != null)
                {
                    playerScript.availableSkills.Add(skill);
                }
            }
        }

        Debug.Log("Player's available skills updated successfully.");
    }
    private void EndTurn()
    {
        canStartTurn = false;
        playerScript.FinalizeTurn(); // Automatically confirm remaining inputs
        ActionMapManager.Instance.ActivateQTEMap(); // Instead of unabling the "Battle" map we activate another blank map
    }
    
   

}
