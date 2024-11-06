using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Stat
{
    // The base value of the stat, before any modifiers
    public float baseValue { get; private set; }

    // List to hold all active modifiers on this stat
    private List<Modifier> modifiers;

    public Stat(float baseValue)
    {
        this.baseValue = baseValue;
        modifiers = new List<Modifier>();
    }

    /// <summary>
    /// Adds a modifier to the stat, affecting its calculated value.
    /// </summary>
    /// <param name="modifier">The modifier to add.</param>
    public void AddModifier(Modifier modifier)
    {
        modifiers.Add(modifier);
    }

    /// <summary>
    /// Removes a modifier from the stat.
    /// </summary>
    /// <param name="modifier">The modifier to remove.</param>
    public void RemoveModifier(Modifier modifier)
    {
        modifiers.Remove(modifier);
    }

    /// <summary>
    /// Calculates and returns the final value of the stat, including all active modifiers.
    /// </summary>
    public float CalculateFinalValue()
    {
        float finalValue = baseValue;

        // Separate modifiers by type for correct application order
        var baseValueModifiers = modifiers.Where(m => m.modifierType == ModifierType.BaseValueModifier).ToList();
        var percentageModifiers = modifiers.Where(m => m.modifierType == ModifierType.Percentage).ToList();
        var additiveModifiers = modifiers.Where(m => m.modifierType == ModifierType.Additive).ToList();

        // Apply Base modifierValue Modifiers first
        foreach (var mod in baseValueModifiers)
        {
            finalValue += mod.modifierValue;
        }

        // Apply Percentage Modifiers
        foreach (var mod in percentageModifiers)
        {
            finalValue *= 1 + mod.modifierValue / 100;
        }

        // Apply Additive Modifiers (added last)
        foreach (var mod in additiveModifiers)
        {
            finalValue += mod.modifierValue;
        }

        return finalValue;
    }

    /// <summary>
    /// Resets the stat to its base value, clearing all modifiers.
    /// </summary>
    public void Reset()
    {
        modifiers.Clear();
    }

    /// <summary>
    /// Logs details of the stat for debugging purposes.
    /// </summary>
    public void LogDetails()
    {
        Debug.Log($"Stat Base modifierValue: {baseValue}");
        Debug.Log("Modifiers:");

        foreach (var modifier in modifiers)
        {
            Debug.Log($" - {modifier.modifierType}: {modifier.modifierValue} (modifierSource: {modifier.modifierSource})");
        }

        Debug.Log($"Final Calculated modifierValue: {CalculateFinalValue()}");
    }
}
