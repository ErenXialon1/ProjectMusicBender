using UnityEngine;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    

    public enum TurnState { PlayerTurn, EnemyTurn }
    public TurnState CurrentTurn { get; private set; }

    [Header("Turn Settings")]
    public PlayerTurnHandler playerTurnHandler;
    public EnemyTurnHandler enemyTurnHandler;
    public UIHandler uiHandler;

    private void Awake()
    {
        
        
    }
    private void Start()
    {
        StartCoroutine(GameLoop());
    }


    public IEnumerator GameLoop()
    {
        while (true)
        {
            // Start Player Turn
            CurrentTurn = TurnState.PlayerTurn;
            
            
            yield return StartCoroutine(playerTurnHandler.StartTurn());

            if (GameIsOver()) yield break;

            // Start Enemy Turn
            CurrentTurn = TurnState.EnemyTurn;
            
            
            yield return StartCoroutine(enemyTurnHandler.StartTurn());

            if (GameIsOver()) yield break;
        }
    }

    private bool GameIsOver()
    {
        bool isOver = false;
        // Placeholder for game-over condition
        return isOver;
    }
}
