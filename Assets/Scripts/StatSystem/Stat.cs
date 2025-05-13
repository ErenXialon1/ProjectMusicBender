using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Stat
{
    public string statName ; // Name of the stat from the definition
    public float baseValue;  // Base value from definition

    
    private List<Modifier> modifiers = new List<Modifier>(); // List of modifiers applied to this stat

    // Event triggered when the stat changes
    public event Action OnStatChanged;

    public Stat(string name, float baseValue)
    {
        this.statName = name;
        this.baseValue = baseValue;
    }

    /// <summary>
    /// Adds a modifier to the stat, affecting its calculated value.
    /// </summary>
    public void AddModifier(Modifier modifier)
    {
        modifiers.Add(modifier);
        OnStatChanged?.Invoke();
    }

    /// <summary>
    /// Removes a modifier from the stat.
    /// </summary>
    public void RemoveModifier(Modifier modifier)
    {
        modifiers.Remove(modifier);
        OnStatChanged?.Invoke();
    }

    /// <summary>
    /// Calculates and returns the final value of the stat, including all active modifiers.
    /// </summary>
    public float CalculateFinalValue()
    {
        float finalValue = baseValue;
        foreach (var mod in modifiers.OrderBy(m => m.modifierDefinition.modifierType))
        {
            finalValue = mod.Apply(finalValue);
        }
        //OnStatChanged?.Invoke();
        return finalValue;
        
    }

    /// <summary>
    /// Resets the stat by clearing all modifiers.
    /// </summary>
    public void Reset()
    {
        modifiers.Clear();
        OnStatChanged?.Invoke();
    }

    /// <summary>
    /// Logs details of the stat for debugging.
    /// </summary>
    public void LogDetails()
    {
        Debug.Log($"Stat Name: {statName}, Base Value: {baseValue}");
        Debug.Log("Modifiers:");
        foreach (var modifier in modifiers)
        {
            Debug.Log($" - {modifier.modifierDefinition.modifierType}: {modifier.modifierDefinition.modifierValue} (Source: {modifier.modifierSource})");
        }
        Debug.Log($"Final Calculated Value: {CalculateFinalValue()}");
    }
}
