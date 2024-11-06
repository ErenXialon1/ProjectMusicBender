using System.Collections.Generic;
using UnityEngine;

public class StatFactory
{
    // Singleton instance
    private static StatFactory instance;
    public static StatFactory Instance => instance ??= new StatFactory();

    // Private constructor to prevent external instantiation
    private StatFactory() { }

    // Stat template definition for predefined base values
    [System.Serializable]
    public class StatTemplate
    {
        public string statName;
        public float statBaseValue;
    }

    // Dictionary to store predefined stat templates
    private Dictionary<string, StatTemplate> statTemplates = new Dictionary<string, StatTemplate>();

    // Dictionary to store created stats, indexed by name for quick access
    private Dictionary<string, Stat> createdStats = new Dictionary<string, Stat>();

    /// <summary>
    /// Initializes the stat factory with predefined templates.
    /// This can be used for global stat definitions or loaded from data.
    /// </summary>
    public void InitializeTemplates(List<StatTemplate> templates)
    {
        foreach (var template in templates)
        {
            if (!statTemplates.ContainsKey(template.statName))
            {
                statTemplates.Add(template.statName, template);
            }
        }
    }

    /// <summary>
    /// Generates a stat based on a template name.
    /// If the stat is already created, it returns the existing instance.
    /// </summary>
    /// <param name="statName">The name of the stat to generate.</param>
    /// <returns>An instance of the requested stat.</returns>
    public Stat GetOrCreateStat(string statName)
    {
        // Check if the stat already exists
        if (createdStats.ContainsKey(statName))
        {
            return createdStats[statName];
        }

        // Find a template matching the requested stat name
        if (statTemplates.TryGetValue(statName, out var template))
        {
            // Create a new Stat based on the template
            Stat newStat = new Stat(template.statBaseValue);
            createdStats[statName] = newStat;
            return newStat;
        }
        else
        {
            Debug.LogWarning($"No template found for stat '{statName}'. Creating default stat.");
            Stat defaultStat = new Stat(0); // Default to base value of 0 if not found
            createdStats[statName] = defaultStat;
            return defaultStat;
        }
    }

    /// <summary>
    /// Resets and clears all created stats, useful when refreshing stats for a new character or enemy.
    /// </summary>
    public void ResetAllStats()
    {
        foreach (var stat in createdStats.Values)
        {
            stat.Reset();
        }
        createdStats.Clear();
    }

    /// <summary>
    /// Adds a modifier to a specific stat, identified by name.
    /// </summary>
    /// <param name="statName">The name of the stat to modify.</param>
    /// <param name="modifier">The modifier to apply.</param>
    public void AddModifierToStat(string statName, Modifier modifier)
    {
        Stat stat = GetOrCreateStat(statName);
        stat.AddModifier(modifier);
    }

    /// <summary>
    /// Removes a modifier from a specific stat, identified by name.
    /// </summary>
    /// <param name="statName">The name of the stat to modify.</param>
    /// <param name="modifier">The modifier to remove.</param>
    public void RemoveModifierFromStat(string statName, Modifier modifier)
    {
        if (createdStats.ContainsKey(statName))
        {
            createdStats[statName].RemoveModifier(modifier);
        }
        else
        {
            Debug.LogWarning($"Attempted to remove modifier from non-existent stat '{statName}'.");
        }
    }

    /// <summary>
    /// Retrieves the calculated final value of a stat.
    /// </summary>
    /// <param name="statName">The name of the stat to retrieve.</param>
    /// <returns>The final calculated value of the stat.</returns>
    public float GetStatFinalValue(string statName)
    {
        if (createdStats.ContainsKey(statName))
        {
            return createdStats[statName].CalculateFinalValue();
        }
        else
        {
            Debug.LogWarning($"Stat '{statName}' not found.");
            return 0;
        }
    }

    /// <summary>
    /// Logs details of all created stats for debugging.
    /// </summary>
    public void LogAllStats()
    {
        Debug.Log("Logging all stats:");
        foreach (var statEntry in createdStats)
        {
            Debug.Log($"Stat Name: {statEntry.Key}");
            statEntry.Value.LogDetails();
        }
    }
}
