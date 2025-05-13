using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatSheet : MonoBehaviour
{
    [Header("Health Settings")]
    public float currentHealth;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => CalculateMaxHealth();
    public float healthMultiplier = 150f;

    // Events for notifying listeners when health changes or when the entity dies
    public event Action<float, float> HealthChanged;  // Notifies of current and max health
    public event Action OnDeath;

    // Event triggered when any stat changes
    public event Action<StatDefinition> OnStatChanged;

    [Header("Stat Definitions")]
    [Tooltip("Default stats for this entity, defined by ScriptableObjects.")]
    [SerializeField] private List<StatDefinition> defaultStats; // Add your StatDefinitions here in the inspector.

    [Header("Stats")]
    // Dictionary to store stats using their name for quick access
    public Dictionary<string, Stat> stats = new Dictionary<string, Stat>();
    public IReadOnlyDictionary<string, Stat> Stats => stats;


    protected virtual void Start()
    {
        // Initialize health based on the calculated max health from relevant stat (e.g., Vigor)
        

        // Subscribe to stat changes, especially any that might impact health
        OnStatChanged += HandleStatChange;
        InitializeStats();
        currentHealth = MaxHealth;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        OnStatChanged -= HandleStatChange;
    }

    /// <summary>
    /// Calculates the maximum health based on a health-relevant stat (e.g., Vigor).
    /// </summary>
    private float CalculateMaxHealth()
    {
        // Assuming "Vigor" stat influences health
        Stat vigorStat = GetStat("VIGOR");
        if (vigorStat != null)
        {
            return vigorStat.CalculateFinalValue() * healthMultiplier;  // Adjust multiplier as needed
        }
        return 1000f;  // Default max health if "Vigor" is unavailable
    }

    /// <summary>
    /// Called when relevant stats affecting health change (e.g., Vigor).
    /// </summary>
    private void HandleStatChange(StatDefinition statDefinition)
    {
        if (statDefinition.statName == "VIGOR")  // Assuming Vigor affects health
        {
            float newMaxHealth = MaxHealth;
            currentHealth = Mathf.Min(currentHealth, newMaxHealth);  // Cap current health at max
            HealthChanged?.Invoke(currentHealth, newMaxHealth);
        }
    }

    /// <summary>
    /// Deals damage to the entity.
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        HealthChanged?.Invoke(currentHealth, MaxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Heals the entity up to the maximum health limit.
    /// </summary>
    public virtual void Heal(float healAmount)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Min(currentHealth + healAmount, MaxHealth);
        HealthChanged?.Invoke(currentHealth, MaxHealth);
    }

    /// <summary>
    /// Called when the entity's health reaches zero.
    /// </summary>
    protected virtual void Die()
    {
        OnDeath?.Invoke();
        Debug.Log($"{gameObject.name} has died.");
        //Destroy(gameObject);  // Consider using pooling or disabling instead of destroying
    }

    #region Stat Management

    /// <summary>
    /// Retrieves a stat by name.
    /// </summary>
    public Stat GetStat(string statName)
    {
        stats.TryGetValue(statName, out var stat);
        return stat;
    }
    // Initialize stats based on the default StatDefinitions
    private void InitializeStats()
    {
        foreach (var statDef in defaultStats)
        {
            if (!stats.ContainsKey(statDef.statName))
            {
                // Create a new Stat using the definition's default value
                var stat = new Stat(statDef.statName, statDef.defaultValue);
                stats[statDef.statName] = stat;
            }
        }
    }
    /// <summary>
    /// Adds or updates a stat.
    /// </summary>
    public void AddOrUpdateStat(StatDefinition statDefinition)
    {
        if (statDefinition == null) throw new ArgumentNullException(nameof(statDefinition));

        if (!stats.ContainsKey(statDefinition.statName))
        {
            stats[statDefinition.statName] = new Stat(statDefinition.statName, statDefinition.defaultValue);
        }
    }

    /// <summary>
    /// Applies a modifier to a specific stat.
    /// </summary>
    public void ApplyModifier(string statName, Modifier modifier)
    {
        if (stats.TryGetValue(statName, out var stat))
        {
            stat.AddModifier(modifier);
           
        }
        else
        {
            Debug.LogWarning($"Stat '{statName}' not found. Cannot apply modifier.");
        }
    }

    /// <summary>
    /// Removes a modifier from a specific stat.
    /// </summary>
    public void RemoveModifier(string statName, Modifier modifier)
    {
        if (stats.TryGetValue(statName, out var stat))
        {
            stat.RemoveModifier(modifier);
            
        }
        else
        {
            Debug.LogWarning($"Stat '{statName}' not found. Cannot remove modifier.");
        }
    }

    /// <summary>
    /// Resets all stats to their base values by clearing all modifiers.
    /// </summary>
    public void ResetAllStats()
    {
        foreach (var stat in stats.Values)
        {
            stat.Reset();
           
        }
    }

    #endregion

    #region Debugging

    /// <summary>
    /// Logs all stats and their final values for debugging purposes.
    /// </summary>
    public void LogAllStats()
    {
        Debug.Log("Logging all stats:");
        foreach (var statEntry in stats)
        {
            Debug.Log($"Stat Name: {statEntry.Key}, Final Value: {statEntry.Value.CalculateFinalValue()}");
            statEntry.Value.LogDetails();
        }
    }

    #endregion
}
