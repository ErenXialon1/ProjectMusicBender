using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;

public class UIHandler : MonoBehaviour
{
    [Header("Turn Text")]
    [Tooltip("Text to display the current turn (e.g., 'Player Turn', 'Enemy Turn')")]
    [SerializeField] TextMeshProUGUI turnText;

    [Header("Timer Display")]
    [Tooltip("Text to display the remaining time for the player's turn")]
    [SerializeField] TextMeshProUGUI timerText;

    [Header("Animation Settings")]
    [Tooltip("Duration for the fade-in and fade-out of the turn text")]
    public float turnTextFadeDuration = 0.5f;
    [Header("Events")]
    [Tooltip("Event triggered when the turn text animation completes")]
   
    [Header("Turn Settings")]
    [SerializeField] PlayerTurnHandler playerTurnHandler;
    [SerializeField] EnemyTurnHandler enemyTurnHandler;
    

    
    

    /// <summary>
    /// Displays the turn text with an animation.
    /// </summary>
    /// <param name="text">The text to display (e.g., 'Player Turn')</param>
    public void ShowTurnText(string text)
    {
        if (turnText.text != text || turnText.color.a == 0)
        {
            turnText.text = text;
            if(text == playerTurnHandler.turnText)
            {
                UpdateTimer(playerTurnHandler.turnDuration);
            }
            turnText.DOFade(1, turnTextFadeDuration).OnComplete(() =>
            {
                turnText.DOFade(0, turnTextFadeDuration).SetDelay(1.0f).OnComplete(() =>
                {
                    
                    
                    if(text == playerTurnHandler.turnText)
                    {


                        playerTurnHandler.canStartTurn = true;
                    }
                    if(text == enemyTurnHandler.turnText)
                    {
                        
                    }
                    
                });
            });
        }
    }

    /// <summary>
    /// Updates the countdown timer display during the player's turn.
    /// </summary>
    /// <param name="timeRemaining">The time remaining in seconds</param>
    public void UpdateTimer(float timeRemaining)
    {
        if (timeRemaining > 0)
        {
            timerText.text = $"Time: {timeRemaining:F2}s"; // Format with 1 decimal place
            if (timerText.color.a < 0.1f) timerText.DOFade(1, 0f);
        }
        else
        {
            if (timerText.color.a > 0.9f) timerText.DOFade(0, turnTextFadeDuration);
        }
    }
}
