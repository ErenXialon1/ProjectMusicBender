using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewStatDefinition", menuName = "Stat System/Stat Definition")]
public class StatDefinition : ScriptableObject
{
    // statName of the stat, e.g., "Health", "Attack"
    public string statName;

    // statDescription of what the stat does
    public string statDescription;

    // Default base value of the stat, can be overridden by individual entities
    public float defaultValue;

    // Optional minimum and maximum value for the stat to avoid negative or unrealistic calculatedValues
    public float minValue;
    public float maxValue; 

    

    // Method to validate if a value falls within defined limits
    public float ClampValue(float value)
    {
        return Mathf.Clamp(value, minValue, maxValue);
    }
}
