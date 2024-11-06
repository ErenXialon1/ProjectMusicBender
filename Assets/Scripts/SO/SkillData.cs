
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill System/Skill Data")]
public class SkillData : ScriptableObject
{
    public string skillName; // statName of the skill
    public string combination; // The combination needed to execute the skill (e.g., "WA", "WASD")
    public float damage; // Damage value of the skill
    public Animation skillAnimation; // Animation for the skill
    public float skillAnimationTime; // Duration of the skill animation
    public GameObject skillPrefab; // Prefab representing the skill in-game
    public Sprite skillImage; // Image used to represent the skill in UI

    [Tooltip("Addressable path for loading this skill via Addressables")]
    public string skillAddress; // The addressable path of this skill (added field)
}