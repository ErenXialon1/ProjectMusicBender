using UnityEngine;


public class SkillCommand : ICommand
{
    [Header("Skill Settings")]
    [Tooltip("Prefab of the skill to be executed.")]
    private GameObject skillPrefab;

    [Tooltip("Amount of damage the skill inflicts.")]
    private float damageAmount;

    [Tooltip("The target enemy of the skill.")]
    private Transform target;

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the SkillCommand class with specified parameters.
    /// </summary>
    /// <param name="skill">The skill prefab to be used.</param>
    /// <param name="damage">The damage amount dealt by the skill.</param>
    /// <param name="enemyTarget">The target of the skill.</param>
    public SkillCommand(GameObject skill, float damage, Transform enemyTarget)
    {
        skillPrefab = skill;
        damageAmount = damage;
        target = enemyTarget;
    }

    #endregion

    #region Command Execution

    /// <summary>
    /// Executes the skill command by spawning the skill prefab and applying damage to the target.
    /// </summary>
    public void Execute()
    {
        Debug.Log($"Executing skill: {skillPrefab.name}");

        // Example skill effect instantiation, could be more complex
        var skillInstance = GameObjectPool.Instance.GetFromPool(skillPrefab);
        skillInstance.transform.position = target.position; // Set to target position or desired location

        // Apply damage if target has a HealthComponent
        var healthComponent = target.GetComponent<HealthComponent>();
        if (healthComponent != null)
        {
            healthComponent.TakeDamage(damageAmount);
        }
    }

    #endregion
}
