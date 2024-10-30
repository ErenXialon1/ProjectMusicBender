using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skill System/Skill Data")]
public class SkillData : ScriptableObject
{
    public string skillName; // Name of the skill
    public string combination; // The combination needed to execute the skill (e.g., "WA", "WASD")
    public float damage; // Damage value of the skill
    public GameObject skillPrefab; // Prefab that represents the skill in-game
    public Sprite skillImage; // Image used to represent the skill in UI
}