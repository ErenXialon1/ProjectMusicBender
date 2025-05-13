using UnityEngine;


public class ValueCalculator
{
    private readonly StatSheet attackerStatSheet;
    private readonly StatSheet defenderStatSheet;

    public ValueCalculator(StatSheet attacker, StatSheet defender = null)
    {
        attackerStatSheet = attacker;
        defenderStatSheet = defender;
    }

    /// <summary>
    /// Calculates the effect value of a skill based on its base value, scaling stat, and modifiers.
    /// </summary>
    public float CalculateSkillEffect(SkillData skillData)
    {
        float effectValue = skillData.baseValue;

        foreach (var scalingFactor in skillData.scalingFactors)
        {
            Stat scalingStat = attackerStatSheet.GetStat(scalingFactor.statName);
            if (scalingStat != null)
            {
                effectValue += scalingStat.CalculateFinalValue() * scalingFactor.multiplier;
            }
            else
            {
                Debug.LogWarning($"Scaling stat '{scalingFactor.statName}' not found in attacker StatSheet.");
            }
        }

        foreach (var modifierDefinition in skillData.modifiers)
        {
            Modifier modifier = modifierDefinition.CreateModifierInstance();
            effectValue = modifier.Apply(effectValue);
        }

        return Mathf.Max(effectValue, 0f);
    }

    /// <summary>
    /// Calculates the final damage dealt to the defender after considering the defender's defense stat.
    /// </summary>
    public float CalculateDamage(SkillData skillData)
    {
        // Calculate the base skill effect (damage) value
        float baseDamage = CalculateSkillEffect(skillData);
        float defMultiplier = 100f;
        // If there's no defender, return the base damage value directly
        if (defenderStatSheet == null)
        {
            Debug.Log("Defender sheet is null");
            return baseDamage;
        }

        // Retrieve the defender's DEF stat to reduce incoming damage
        Stat defenderDefStat = defenderStatSheet.GetStat("DEF");
        float defenderDefValue = defenderDefStat?.CalculateFinalValue() ?? 0f;

        // Calculate final damage after defense reduction
        float finalDamage = baseDamage - (defenderDefValue * defMultiplier);

        return Mathf.Max(finalDamage, 0f); // Ensure non-negative final damage
    }

    /// <summary>
    /// Calculates the healing amount based on the skill's base value and attacker's scaling stat.
    /// </summary>
    public float CalculateHealing(SkillData skillData)
    {
        // Calculate healing value similar to damage but without defense consideration
        return CalculateSkillEffect(skillData);
    }

    
}
