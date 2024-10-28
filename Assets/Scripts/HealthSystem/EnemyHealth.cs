using UnityEngine;

public class EnemyHealth : HealthComponent
{
    // Additional enemy-specific health logic can be implemented here if needed
    protected override void Die()
    {
        base.Die();
        // Add enemy-specific death handling, like awarding points to the player
        Debug.Log($"{gameObject.name} has been defeated.");
    }
}
