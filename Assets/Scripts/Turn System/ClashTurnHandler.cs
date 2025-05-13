using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClashTurnHandler : MonoBehaviour
{
    private enum ClashResult
    {
        ChainWin,
        ChainEqual,
        ChainLose
    }
    [Header("Player References")]
    public Player playerScript; // Reference to the Player script
    public PlayerStatSheet playerStatSheet; // Reference to the Player's stat sheet

    [Header("Enemy References")]
    public Enemy enemyScript; // Reference to the Enemy script
    public EnemyStatSheet enemyStatSheet; // Reference to the Enemy's stat sheet
    public EnemyTurnHandler enemyTurnHandler;

    [Header("Clash Settings")]
    
    [Tooltip("Buffer time after clash resolution.")]
    [SerializeField] private float resolutionBufferTime = 2f; // Buffer time between clashes


  
    [Header("UI Settings")]
    [Tooltip("UI Handler for displaying clash-related elements.")]
    [SerializeField] private UIHandler uiHandler;
    

    public IEnumerator StartTurn()
    {
        Debug.Log("Clash Turn Started!");

        Clash();

        // Add buffer time before the next turn
        yield return new WaitForSeconds(resolutionBufferTime);

        Debug.Log("Clash Turn Ended!");
    }
    private void Clash()
    {
        Queue<List<string>> playerQueue = playerScript.GetPlayerQueue();
        Queue<List<string>> enemyQueue = enemyTurnHandler.GetEnemyActionQueue();

        if(playerQueue.Count <= 0 && enemyQueue.Count > 0) 
        {
            Debug.LogWarning("Enemy wins the clash! Player takes damage.");
            ExecuteEnemyChain(enemyQueue.Peek());
        }
        else if(playerQueue.Count <= 0 && enemyQueue.Count <= 0)
        {
            Debug.Log("Queues are empty! Nothing happened.");
        }
        while (playerQueue.Count > 0)
        {
            // Dequeue the first chain of skills
            List<string> playerChain = playerQueue.Dequeue();
            List<string> enemyChain = enemyQueue.Count > 0 ? enemyQueue.Dequeue() : null;

            // Evaluate the clash outcome
            ClashResult clashResult = EvaluateClashOutcome(playerChain, enemyChain);

            if (clashResult == ClashResult.ChainWin)
            {
                Debug.Log("Player wins the clash!");
                ExecutePlayerChain(playerChain);
            }
            else if (clashResult == ClashResult.ChainEqual)
            {
                Debug.Log("Clash is equal. No damage dealt.");
                ExecuteEqualChain(); // Placeholder for now
            }
            else if (clashResult == ClashResult.ChainLose)
            {
                Debug.LogWarning("Enemy wins the clash! Player takes damage.");
                ExecuteEnemyChain(enemyChain);
                break; // End clash turn if the enemy wins
            }
        }
        

    }
    
    private ClashResult EvaluateClashOutcome(List<string> playerChain, List<string> enemyChain)
    {
        

        if (enemyChain == null || enemyChain.Count == 0)
        { 
            return ClashResult.ChainWin;// Enemy has no chain to counter
        } 
        if (enemyChain.Count != playerChain.Count)
        { 
            return ClashResult.ChainLose;
        }
        // Make a mutable copy of the enemyChain to track used skills
        List<string> matchedSkills = new List<string>(enemyChain);

        // Compare player's chain with enemy's chain
        foreach (var playerSkill in playerChain)
        {
            if (enemyChain.Contains(playerSkill))
            {
                matchedSkills.Remove(playerSkill);
            }
        }

        if (matchedSkills.Count == 0)
        {
            return ClashResult.ChainEqual;// All skills matched equally
        } 

        

        return ClashResult.ChainLose; // Player failed to counter enemy's chain
    }
    

    private void ExecutePlayerChain(List<string> playerChain)
    {
        foreach (var skillCombo in playerChain)
        {
            SkillData playerSkill = playerScript.GetSkillByCombination(skillCombo);
            if (playerSkill != null)
            {
                StartCoroutine(ExecuteSkillCoroutine(playerSkill, playerStatSheet, enemyStatSheet));
            }
            else
            {
                Debug.Log($"No valid skill for player's combination: {skillCombo}");
            }
        }
    }
    private void ExecuteEqualChain()
    {
        //TO DO Some Feedback Mechanism
    }
    private void ExecuteEnemyChain(List<string> enemyChain)
    {
        SkillData enemySkill = enemyScript.GetSkillByCombination(enemyChain[0]);
        StartCoroutine(ExecuteSkillCoroutine(enemySkill, enemyStatSheet, playerStatSheet));
    }

    /// <summary>
    /// Executes a skill on the target's stat sheet.
    /// </summary>
    /// <param name="skillData">The skill to execute.</param>
    /// <param name="attackerStatSheet">The stat sheet of the attacker.</param>
    /// <param name="targetStatSheet">The stat sheet of the target.</param>
    private IEnumerator ExecuteSkillCoroutine(SkillData skillData, StatSheet attackerStatSheet, StatSheet targetStatSheet)
    {
        float skillAnimationLength;

        if (skillData == null || attackerStatSheet == null || targetStatSheet == null)
        {
            Debug.LogWarning("SkillData, Attacker StatSheet, or Target StatSheet is null. Skipping execution.");
            yield break;
        }

        // Create an instance of the Skill class for the current skill
        Skill skill = new Skill(skillData, attackerStatSheet);

        // Execute the skill
        skill.Execute(targetStatSheet);

        // Handle skill animation
        if (skillData.skillAnimation != null)
        {
            skillAnimationLength = skillData.skillAnimation.clip.length;
        }
        else
        {
            Debug.Log("Skill animation not assigned. Using default length of 1 second.");
            skillAnimationLength = 1f;
        }

        yield return new WaitForSeconds(skillAnimationLength); // Wait for the animation to complete
    }


}
