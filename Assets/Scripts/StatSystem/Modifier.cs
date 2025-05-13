using System;
using UnityEngine;

[System.Serializable]
public class Modifier
{
    public ModifierDefinition modifierDefinition;  // Reference to the ModifierDefinition

    public object modifierSource;      // Optional source for tracking origin (e.g., item, skill, effect)

    // Constructor to initialize a modifier with its definition and optional source
    public Modifier(ModifierDefinition definition, object source = null)
    {
        modifierDefinition = definition ?? throw new ArgumentNullException(nameof(definition));
        modifierSource = source;
    }

    /// <summary>
    /// Applies this modifier to a given base value based on its type.
    /// </summary>
    public float Apply(float baseValue)
    {
        switch (modifierDefinition.modifierType)
        {
            case ModifierType.Percentage:
                return baseValue * (1 + modifierDefinition.modifierValue / 100f);
            case ModifierType.BaseValueModifier:
                return baseValue + modifierDefinition.modifierValue;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// Log details for debugging the modifier.
    /// </summary>
    public void LogModifierDetails()
    {
        Debug.Log($"Modifier Type: {modifierDefinition.modifierType}, Value: {modifierDefinition.modifierValue}, Source: {modifierSource}");
    }
}
