using UnityEngine;

public class PlayerHealth : HealthComponent
{
    // Additional player-specific health logic can be implemented here if needed
    protected override void Die()
    {
        base.Die();
        // Add player-specific death handling, like triggering a game-over screen
        Debug.Log("Player has died. Game Over.");
    }
}
