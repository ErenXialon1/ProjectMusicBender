using System;
using System.Collections.Generic;
using UnityEngine;

public class StatSheet : MonoBehaviour
{
    // Dictionary to store stats by their StatDefinition keys
    private Dictionary<StatDefinition, Stat> stats = new Dictionary<StatDefinition, Stat>();

    // Events for stat changes
    public event Action<StatDefinition> OnStatChanged;

    /// <summary>
    /// Initializes the StatSheet with default stats using the StatFactory.
    /// </summary>
    public void Initialize(List<StatDefinition> defaultDefinitions)
    {
        foreach (var definition in defaultDefinitions)
        {
            if (!stats.ContainsKey(definition))
            {
                Stat stat = StatFactory.Instance.GetOrCreateStat(definition.statName);
                stats.Add(definition, stat);
            }
        }
    }

    /// <summary>
    /// Retrieves a stat by its definition.
    /// </summary>
    public Stat GetStat(StatDefinition definition)
    {
        if (stats.TryGetValue(definition, out var stat))
        {
            return stat;
        }
        else
        {
            Debug.LogWarning($"Stat {definition.statName} not found in StatSheet.");
            return null;
        }
    }

    /// <summary>
    /// Applies a modifier to a specific stat.
    /// </summary>
    public void ApplyModifier(StatDefinition definition, Modifier modifier)
    {
        if (stats.TryGetValue(definition, out var stat))
        {
            stat.AddModifier(modifier);
            OnStatChanged?.Invoke(definition);  // Notify listeners of the stat change
        }
        else
        {
            Debug.LogWarning($"Stat {definition.statName} not found in StatSheet.");
        }
    }

    /// <summary>
    /// Removes a modifier from a specific stat.
    /// </summary>
    public void RemoveModifier(StatDefinition definition, Modifier modifier)
    {
        if (stats.TryGetValue(definition, out var stat))
        {
            stat.RemoveModifier(modifier);
            OnStatChanged?.Invoke(definition);  // Notify listeners of the stat change
        }
        else
        {
            Debug.LogWarning($"Stat {definition.statName} not found in StatSheet.");
        }
    }

    /// <summary>
    /// Resets all stats to their base values.
    /// </summary>
    public void ResetStats()
    {
        foreach (var stat in stats.Values)
        {
            stat.Reset();
        }
    }
}
