using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnHandler : MonoBehaviour
{
    

    [Tooltip("Text to display during the enemy's turn.")]
    public string turnText = "Enemy Turn";

    [Tooltip("Reference to the UI handler for displaying turn information.")]
    [SerializeField] private UIHandler uiHandler;

    [Header("Enemy References")]
    [Tooltip("List of enemies participating in the turn.")]
    [SerializeField] private List<Enemy> enemies;

    [Tooltip("Queue to store the final combined actions of all enemies.")]
    [SerializeField] private Queue<List<string>> enemyActionQueue;
    [SerializeField] private PlayerStatSheet playerStatSheet;

    [Header("Turn Duration Settings")]
    [Tooltip("Minimum duration of a turn, in seconds.")]
    [SerializeField] private float minTurnDuration = 5f;

    [Tooltip("Maximum duration of a turn, in seconds.")]
    [SerializeField] private float maxTurnDuration = 20f;
    private void Awake()
    {
        // Initialize the actionGroup queue
        enemyActionQueue = new Queue<List<string>>();
    }
    
    /// <summary>
    /// Starts the enemy's turn, selecting selectedSkillsOfEnemy and generating a combined actionGroup queue.
    /// </summary>
    public IEnumerator StartTurn()
    {
        Debug.Log("Enemy Turn Started!");

        // Display turn text
        uiHandler.ShowTurnText(turnText);

        // Generate actions for the turn
        GenerateEnemyActions();

        // Wait for the turn duration
        yield return new WaitForSeconds(5f);

        EndTurn();
    }
    /// <summary>
    /// Generates enemy actions by selecting selectedSkillsOfEnemy and chaining them based on intervals.
    /// </summary>
    private void GenerateEnemyActions()
    {
        enemyActionQueue.Clear(); // Reset the queue for the turn

        
       
        foreach (var enemy in enemies)
        {
            enemy.ResetRemainingUses(); // Reset skill usage at the start of the turn

            List<SkillData> availableSkills = enemy.GetAvailableSkills();
            int maxSkills = enemy.GetMaxSkillsPerTurn();
            List<SkillData> chosenSkills = SelectRandomSkills(availableSkills, maxSkills, enemy);

            // Directly enqueue selected skills
            foreach (var skill in chosenSkills)
            {
                // Add skill combination to the action queue
                enemyActionQueue.Enqueue(new List<string> { skill.combination });

                // Reduce the remaining uses of the skill
                enemy.DecreaseRemainingUse(skill,1);
            }

        }

        // Step 2: Chain Skills Based on Intervals (OLD SYSTEM)
        // ChainSkills(selectedSkills); (OLD SYSTEM) (We're trying to change this right now)(We should queue them without chaining.)

        // Debugging: Log the generated actions
        Debug.Log("Generated Enemy Action Queue:");
        foreach (var actionGroup in enemyActionQueue)
        {
            Debug.Log(string.Join(", ", actionGroup));
        }
    }
    /// UNUSED METHODS (OLD SYSTEM)
    /// <summary>
    /// Chains selected selectedSkillsOfEnemy based on their intervals and adds them to the actionGroup queue.
    /// </summary>
    
    /*private void ChainSkills(Dictionary<Enemy, List<SkillData>> selectedSkills)
    {
        float turnDuration = CalculateTurnDuration(enemies); // Calculate the overall turn duration
        for (float time = 0; time < turnDuration; time += 1f)
        {
            List<string> currentGroup = new List<string>();
            foreach (var enemyAndSkillPair in selectedSkills)
            {
                Enemy enemy = enemyAndSkillPair.Key;
                List<SkillData> selectedSkillsOfEnemy = enemyAndSkillPair.Value;

                foreach (var skill in selectedSkillsOfEnemy)
                {
                    if (Mathf.Approximately(time % skill.interval, 0f))
                    {
                        currentGroup.Add(skill.combination); // Add skill to the current group
                    }
                }
            }
            if (currentGroup.Count > 0)
            {
                enemyActionQueue.Enqueue(currentGroup); // Enqueue the group
            }
        }
    }*/

    /// <summary>
    /// Calculates the turn duration based on the player's SPD and the average SPD of active enemies.
    /// </summary>
    public float CalculateTurnDuration(List<Enemy> activeEnemies)
    {
        if (playerStatSheet == null || activeEnemies == null || activeEnemies.Count == 0)
        {
            Debug.LogError("Missing required components for turn duration calculation.");
            return maxTurnDuration; // Default fallback duration
        }

        // Calculate the average SPD of all active enemies
        float enemyAverageSPD = 0f;
        foreach (Enemy enemy in activeEnemies)
        {
            enemyAverageSPD += enemy.StatSheet.GetStat("EnemySPD").CalculateFinalValue(); // Use the helper method
        }
        enemyAverageSPD /= activeEnemies.Count;

        // Calculate turn duration
        float playerSPD = playerStatSheet.GetStat("PlayerSPD").CalculateFinalValue(); // Use the helper method
        float turnDuration = Mathf.Clamp((int)(playerSPD - enemyAverageSPD), minTurnDuration, maxTurnDuration); // Clamp values for balance

        Debug.Log($"Turn Duration: {turnDuration} seconds | Player SPD: {playerSPD}, Enemy Avg SPD: {enemyAverageSPD}");
        return turnDuration;
    }
    /// <summary>
    /// Retrieves the final enemy actionGroup queue for the current turn.
    /// </summary>
    public Queue<List<string>> GetEnemyActionQueue()
    {
        return new Queue<List<string>>(enemyActionQueue); // Return a copy to avoid external modifications
    }


    /// <summary>
    /// Shuffles a list randomly.
    /// </summary>
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
    /// <summary>
    /// Selects a random subset of selectedSkillsOfEnemy for an enemy, ensuring usage limits are respected.
    /// </summary>
    private List<SkillData> SelectRandomSkills(List<SkillData> availableSkills, int maxSkills, Enemy enemy)
    {
        List<SkillData> selectedSkills = new List<SkillData>();

        // Populate a weighted pool based on remaining uses
        List<SkillData> weightedPool = new List<SkillData>();
        foreach (var skill in availableSkills)
        {
            int remaining = enemy.GetRemainingUses(skill); // Get the remaining uses of the skill
            for (int i = 0; i < remaining; i++)
            {
                weightedPool.Add(skill); // Add the skill as many times as its remaining uses
            }
        }

        // Randomly select skills until the maxSkills limit is reached
        while (selectedSkills.Count < maxSkills  )
        {
            if (weightedPool.Count > 0)
            {
                // Select a random skill from the weighted pool
                int randomIndex = Random.Range(0, weightedPool.Count);
                SkillData selectedSkill = weightedPool[randomIndex];

                selectedSkills.Add(selectedSkill);

                // Decrement the remaining uses for the selected skill
                enemy.DecreaseRemainingUse(selectedSkill, 1);

                // Remove the selected skill instance from the pool
                weightedPool.RemoveAt(randomIndex);
            }
        }

        return selectedSkills;
    }



    private void EndTurn()
    {
        
        Debug.Log("Enemy Turn Ended");

        ActionMapManager.Instance.ActivateBattleMap();
        
    }
}
