using UnityEngine;

public class Skill
{
    public SkillData skillData;
    private readonly StatSheet attackerStatSheet;
    

    public Skill(SkillData skillData, StatSheet attackerStatSheet)
    {
        this.skillData = skillData;
        this.attackerStatSheet = attackerStatSheet;
        
    }

    /// <summary>
    /// Executes the skill on a target's StatSheet, applying its effect (e.g., damage or healing).
    /// </summary>
    public virtual void Execute(StatSheet targetStatSheet)
    {
        if (targetStatSheet == null)
        {
            Debug.LogWarning("Target StatSheet is null.");
            return;
        }
        // Pass both attackerStatSheet and targetStatSheet to ValueCalculator
        ValueCalculator valueCalculator = new ValueCalculator(attackerStatSheet, targetStatSheet);

        // Calculate the effect value
        float effectValue = valueCalculator.CalculateDamage(skillData);

        // Apply the effect
        ApplyEffect(targetStatSheet, effectValue);

        // Trigger animations and visuals
        PlayAnimation();
        InstantiateSkillPrefab(targetStatSheet);

        Debug.Log($"{skillData.skillName} executed on {targetStatSheet.name} with effect value {effectValue}");
    }




    /// <summary>
    /// Applies the effect (damage or healing) to the target's health in StatSheet.
    /// </summary>
    protected virtual void ApplyEffect(StatSheet targetStatSheet, float effectValue)
    {
        if (skillData.effectType == SkillEffectType.Damage)
        {
            targetStatSheet.TakeDamage(effectValue);
        }
        else if (skillData.effectType == SkillEffectType.Healing)
        {
            targetStatSheet.Heal(effectValue);
        }
    }

    /// <summary>
    /// Plays the skill animation if available.
    /// </summary>
    protected virtual void PlayAnimation()
    {
        if (skillData.skillAnimation != null)
        {
            Debug.Log($"Playing animation for skill: {skillData.skillName}");
            // Trigger the actual animation if necessary
        }
    }

    /// <summary>
    /// Instantiates the skill's visual prefab at the target's location.
    /// </summary>
    protected virtual void InstantiateSkillPrefab(StatSheet targetStatSheet)
    {
        if (skillData.skillPrefab != null)
        {
            GameObject.Instantiate(skillData.skillPrefab, targetStatSheet.transform.position, Quaternion.identity);
        }
    }
    /// <summary>
    /// Logs debug information about the skill.
    /// </summary>
    public void LogSkillDetails()
    {
        Debug.Log($"Skill: {skillData.skillName}, Effect Type: {skillData.effectType}, Base Value: {skillData.baseValue}");
    }
}
