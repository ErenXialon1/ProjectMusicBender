using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Attributes")]
    [SerializeField] private EnemyStatSheet statSheet; // Reference to the enemy's stat sheet
    public EnemyStatSheet StatSheet => statSheet; // Expose stat sheet for external use
    [SerializeField] private string enemyName; // Name of the enemy for debugging/UI purposes

    [Header("Skills and Balancing")]
    [Tooltip("List of skills available to this enemy.")]
    [SerializeField] private List<SkillData> availableSkills; // List of enemy's skills
    [SerializeField] private List<int> maxUsesPerSkill; // Corresponding max use limits for each skill
    
    private Dictionary<SkillData, int> remainingUses; // Tracks remaining uses of each skill in a turn

    [Header("Other Balancings")]
    [Header("Limits")]
    [Tooltip("Maximum number of skills the enemy can use per turn.")]
    [SerializeField] private int maxSkillsUsablePerTurn = 2;
    [Header("Health Thresholds for Available Skills")]
    [SerializeField] private float phase2Threshold = 0.66f;
    [SerializeField] private float phase3Threshold = 0.33f;
    [Header("Skill Ranges")]
    [SerializeField, Tooltip("Start index for Phase 1 skills.")]
    private int phase1StartIndex = 0;
    [SerializeField, Tooltip("Number of skills in Phase 1.")]
    private int phase1SkillCount = 3;

    [SerializeField, Tooltip("Start index for Phase 2 skills.")]
    private int phase2StartIndex = 3;
    [SerializeField, Tooltip("Number of skills in Phase 2.")]
    private int phase2SkillCount = 3;

    [SerializeField, Tooltip("Start index for Phase 3 skills.")]
    private int phase3StartIndex = 6;
    [SerializeField, Tooltip("Number of skills in Phase 3.")]
    private int phase3SkillCount = 3;
    private void Start()
    {
        // Ensure the stat sheet is initialized
        if (statSheet == null)
        {
            statSheet = GetComponent<EnemyStatSheet>();
            if (statSheet == null)
            {
                Debug.LogError("StatSheet is missing from the enemy.");
                return;
            }
        }
        // Ensure the maxUsesPerSkill list matches availableSkills
        if (maxUsesPerSkill.Count != availableSkills.Count)
        {
            Debug.LogError("Mismatch between availableSkills and maxUsesPerSkill. Please check the inspector setup.");
        }

        // Initialize remaining uses dictionary
        remainingUses = new Dictionary<SkillData, int>();
    }


    /// <summary>
    /// Resets the remaining uses of skills at the start of a turn.
    /// </summary>
    public void ResetRemainingUses()
    {
        // Safety check for mismatched skill lists
        if (availableSkills.Count != maxUsesPerSkill.Count)
        {
            Debug.LogError("Mismatch between availableSkills and maxUsesPerSkill during ResetRemainingUses.");
            return;
        }
        remainingUses.Clear();

        for (int i = 0; i < availableSkills.Count; i++)
        {
            SkillData skill = availableSkills[i];
            int maxUses = maxUsesPerSkill[i];
            remainingUses[skill] = maxUses;
        }
    }
    /// <summary>
    /// Retrieves the remaining uses for a specific skill.
    /// </summary>
    public int GetRemainingUses(SkillData skill)
    {
        if (remainingUses.TryGetValue(skill, out int uses))
        {
            return uses;
        }
        return 0; // If the skill isn't found, return 0 remaining uses
    }
    /// <summary>
    /// Decrease the remaining uses for a specific skill.
    /// </summary>
    public void DecreaseRemainingUse(SkillData skill, int amount)
    {
        
            if (remainingUses.ContainsKey(skill) && remainingUses[skill] > 0)
            {
                remainingUses[skill] = Mathf.Max(0, remainingUses[skill] - amount); // Clamp to avoid negative values
            }
        
    }
    /// <summary>
    /// Increases the remaining uses for a specific skill.
    /// </summary>
    public void IncreaseRemainingUse(SkillData skill, int amount)
    {
        if (remainingUses.ContainsKey(skill))
        {
            int maxUses = maxUsesPerSkill[availableSkills.IndexOf(skill)];
            remainingUses[skill] = Mathf.Min(maxUses, remainingUses[skill] + amount); // Clamp to maxUsesPerSkill
        }
    }
    /// <summary>
    /// Dynamically increases the maximum uses of a specific skill during a turn.
    /// </summary>
    /// <param name="skill">The skill whose max uses should be increased.</param>
    /// <param name="amount">The amount to increase.</param>
    public void IncreaseMaxUses(SkillData skill, int amount)
    {
        int skillIndex = availableSkills.IndexOf(skill);

        if (skillIndex == -1)
        {
            Debug.LogWarning($"Skill {skill.skillName} not found in availableSkills.");
            return;
        }

        maxUsesPerSkill[skillIndex] += amount;
        Debug.Log($"Max uses for {skill.skillName} increased by {amount}. New max: {maxUsesPerSkill[skillIndex]}.");
    }
    /// <summary>
    /// Dynamically decreases the maximum uses of a specific skill during a turn.
    /// </summary>
    /// <param name="skill">The skill whose max uses should be decreased.</param>
    /// <param name="amount">The amount to decrease.</param>
    public void DecreaseMaxUses(SkillData skill, int amount)
    {
        int skillIndex = availableSkills.IndexOf(skill);

        if (skillIndex == -1)
        {
            Debug.LogWarning($"Skill {skill.skillName} not found in availableSkills.");
            return;
        }

        maxUsesPerSkill[skillIndex] = Mathf.Max(0, maxUsesPerSkill[skillIndex] - amount);
        Debug.Log($"Max uses for {skill.skillName} decreased by {amount}. New max: {maxUsesPerSkill[skillIndex]}.");
    }
    /// <summary>
    /// Dynamically determines which skills are available based on the enemy's current health.
    /// </summary>
    public List<SkillData> GetAvailableSkills()
    {
        int startIndex;
        int skillCount;

        if (statSheet.CurrentHealth > statSheet.MaxHealth * phase2Threshold)
        {
            startIndex = phase1StartIndex;
            skillCount = phase1SkillCount; // Skills 1, 2, 3
        }
        else if (statSheet.CurrentHealth > statSheet.MaxHealth * phase3Threshold)
        {
            startIndex = phase2StartIndex;
            skillCount = phase2SkillCount; // Skills 4, 5, 6
        }
        else
        {
            startIndex = phase3StartIndex;
            skillCount = phase3SkillCount; // Skills 7, 8, 9
        }

        return availableSkills.GetRange(startIndex, Mathf.Min(skillCount, availableSkills.Count - startIndex));
    }
    /// <summary>
    /// Dynamically gets the max number of skills the enemy can use per turn.
    /// </summary>
    public int GetMaxSkillsPerTurn()
    {
        int dynamicMax = maxSkillsUsablePerTurn;
        if (statSheet.CurrentHealth < statSheet.MaxHealth * phase2Threshold)
        {
            dynamicMax += 1; // Allow more skills if health is low
        }
        return dynamicMax;
    }

    /// <summary>
    /// Retrieves a SkillData object based on the combination string.
    /// </summary>
    /// <param name="combination">The combination string to look up.</param>
    /// <returns>The matching SkillData or null if not found.</returns>
    public SkillData GetSkillByCombination(string combination)
    {
        foreach (var skill in availableSkills)
        {
            if (skill.combination == combination)
                return skill;
        }
        Debug.LogWarning($"No skill found for combination: {combination}");
        return null;
    }
    

    

   
}
