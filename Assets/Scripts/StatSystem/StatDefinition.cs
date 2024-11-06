using UnityEngine;

[System.Serializable]
public class StatDefinition
{
    // statName of the stat, e.g., "Health", "Attack"
    public string statName { get; private set; }

    // statDescription of what the stat does
    public string statDescription { get; private set; }

    // Default base value of the stat, can be overridden by individual entities
    public float defaultValue { get; private set; }

    // Optional minimum and maximum value for the stat to avoid negative or unrealistic values
    public float minValue { get; private set; }
    public float maxValue { get; private set; }

    // Constructor with only essential fields (expandable with more parameters if needed)
    public StatDefinition(string name, string description, float defaultValue, float minValue = 0, float maxValue = Mathf.Infinity)
    {
        statName = name;
        statDescription = description;
        this.defaultValue = defaultValue;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }

    // Method to validate if a value falls within defined limits
    public float ClampValue(float value)
    {
        return Mathf.Clamp(value, minValue, maxValue);
    }
}
