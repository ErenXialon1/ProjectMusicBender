using System.Collections.Generic;
using UnityEngine;

public enum SkillEffectType
{
    Damage,
    Healing
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill System/Skill Data")]
public class SkillData : ScriptableObject
{
    [Header("Basic Skill Info")]
    public string skillName;               // Name of the skill
    public string combination;
    public string description;// Input combination required to trigger the skill
    public SkillEffectType effectType;     // Type of effect (Damage or Healing)
    public float baseValue;
    

    [Header("Skill Timing")]
    [Tooltip("Interval (in seconds) at which this skill can be used.")]
    public float interval; // How often the skill is used in seconds
    [Header("Scaling and Modifiers")]
    [Tooltip("List of scaling stats and their multipliers.")]
    public List<ScalingFactor> scalingFactors;  // List of scaling stats with their multipliers
    public List<ModifierDefinition> modifiers;  // Modifiers applied when skill is executed


    [Header("Visual & Addressable")]
    public Animation skillAnimation;       // Animation for the skill
    public float skillAnimationTime;       // Duration of the skill animation
    public GameObject skillPrefab;         // Prefab for the skill’s visual representation
    public Sprite skillImage;              // UI image for the skill

    [Tooltip("Addressable path for loading this skill via Addressables")]
    public string skillAddress;            // Addressable path to load this skill

    /// <summary>
    /// A struct to define each scaling factor: the stat it scales with and the multiplier.
    /// </summary>
    [System.Serializable]
    public struct ScalingFactor
    {
        public string statName;    // The name of the stat (e.g., "ATK" or "VIGOR")
        public float multiplier;   // Multiplier applied to this stat's final value
    }

}
