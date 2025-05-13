using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a skill specifically used by enemies, with additional properties for skill combinations.
/// </summary>
public class EnemySkill : Skill
{
    /// <summary>
    /// Constructor to create an EnemySkill with a single SkillData.
    /// </summary>
    /// <param name="skillData">The data representing the skill.</param>
    /// <param name="attackerStatSheet">The stat sheet of the enemy using this skill.</param>
    public EnemySkill(SkillData skillData, StatSheet attackerStatSheet)
        : base(skillData, attackerStatSheet)
    {
    }

    /// <summary>
    /// Executes the skill on a target.
    /// </summary>
    /// <param name="targetStatSheet">The target's stat sheet.</param>
    public override void Execute(StatSheet targetStatSheet)
    {
        Debug.Log($"Executing enemy skill: {skillData.skillName}");
        base.Execute(targetStatSheet);
    }


}
