using UnityEngine;
public enum ModifierType
{
    BaseValueModifier,
    Percentage,
    
}

[CreateAssetMenu(fileName = "NewModifierDefinition", menuName = "Stat System/Modifier Definition")]
public class ModifierDefinition : ScriptableObject
{
    [Header("Modifier Settings")]
    public ModifierType modifierType;         // Type of modifier (Percentage, BaseValueModifier, Additive)
    public float modifierValue;               // Base value for this modifier
    public string sourceDescription;          // Description of the source, e.g., "Skill Bonus", "Item Effect"

    [TextArea]
    public string description;                // Description of what this modifier does, for UI or debugging

    /// <summary>
    /// Creates a Modifier instance based on this definition.
    /// </summary>
    public Modifier CreateModifierInstance(object source = null)
    {
        return new Modifier(this, source ?? sourceDescription);
    }
}
