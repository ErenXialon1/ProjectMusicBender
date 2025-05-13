using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(EnemyStatSheet))]
public class EnemyStatSheetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Get the target StatSheet object
        EnemyStatSheet statSheet = (EnemyStatSheet)target;

        // Display Stats Dictionary
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);

        // Iterate over the stats and display each stat's final calculated value
        if (statSheet.Stats != null && statSheet.Stats.Count > 0)
        {
            foreach (var stat in statSheet.Stats)
            {
                EditorGUILayout.LabelField($"{stat.Key}", $"Value: {stat.Value.CalculateFinalValue()}");
            }
        }
        else
        {
            EditorGUILayout.LabelField("No stats available.");
        }

        // Display persistent value, like current health
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Persistent Values", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Current Health", statSheet.currentHealth.ToString());
    }
}
