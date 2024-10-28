using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/SkillData", order = 1)] //Creates menu for adding new skill
public class SkillData : ScriptableObject
{
    public string skillName; // Name of the skill
    public string combination; // e.g., "WASD"
    public GameObject skillPrefab; // Prefab to instantiate or use
    public float cooldown; // Optional: Cooldown for the skill
}
