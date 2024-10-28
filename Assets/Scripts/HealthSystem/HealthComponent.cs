using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("The maximum health of this entity.")]
    public float maxHealth = 100f;

    [Tooltip("The current health of this entity. Initialized to maxHealth on Start.")]
    private float currentHealth;

    // Events for notifying listeners when health changes or when the entity dies
    [Tooltip("Event triggered when the entity's health changes. Passes current and maximum health.")]
    public delegate void OnHealthChanged(float currentHealth, float maxHealth);
    public event OnHealthChanged HealthChanged;

    [Tooltip("Event triggered when the entity dies.")]
    public delegate void OnDeath();
    public event OnDeath Death;

    #region Unity Methods

    /// <summary>
    /// Called when the script instance is being loaded. Initializes current health to max health.
    /// </summary>
    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    #endregion

    #region Health Management

    /// <summary>
    /// Reduces the current health by a specified damage amount and checks for death.
    /// </summary>
    /// <param name="damage">The amount of damage to inflict.</param>
    public virtual void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Heals the entity by a specified amount, clamped to the max health value.
    /// </summary>
    /// <param name="healAmount">The amount of healing to apply.</param>
    public virtual void Heal(float healAmount)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    #endregion

    #region Death Handling

    /// <summary>
    /// Handles the entity's death by invoking the death event and optionally destroying the GameObject.
    /// </summary>
    protected virtual void Die()
    {
        Death?.Invoke();
        // Additional logic such as triggering animations, dropping loot, etc.
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject); // This can be changed to pooling or disabling instead of destroying.
    }

    #endregion
}
