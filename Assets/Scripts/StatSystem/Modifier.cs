using System;

public enum ModifierType
{
    Percentage,
    BaseValueModifier,
    Additive
}

[System.Serializable]
public class Modifier
{
    // modifierType of modifier (Percentage, BaseValueModifier, Additive)
    public ModifierType modifierType { get; private set; }

    // The value to be applied by this modifier
    public float modifierValue { get; private set; }

    // Optional source for tracking where this modifier comes from (e.g., item, skill, effect)
    public object modifierSource { get; private set; }

    // Constructor to initialize a modifier with type, value, and optional source
    public Modifier(ModifierType type, float value, object source = null)
    {
        modifierType = type;
        modifierValue = value;
        modifierSource = source;
    }

    /// <summary>
    /// Applies this modifier to a given base value, based on its type.
    /// </summary>
    /// <param name="baseValue">The original value of the stat before any modifications.</param>
    /// <returns>The modified value after applying this modifier.</returns>
    public float ApplyModifier(float baseValue)
    {
        switch (modifierType)
        {
            case ModifierType.Percentage:
                return baseValue * (1 + modifierValue / 100);

            case ModifierType.BaseValueModifier:
                return baseValue + modifierValue;

            case ModifierType.Additive:
                return modifierValue;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
