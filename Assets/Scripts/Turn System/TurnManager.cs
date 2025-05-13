using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnManager : MonoBehaviour
{
    

    public enum TurnState { EnemyTurn, PlayerTurn, ClashTurn }
    public TurnState CurrentTurn { get; private set; }

    [Header("Turn Settings")]
    public PlayerTurnHandler playerTurnHandler;
    public EnemyTurnHandler enemyTurnHandler;
    public ClashTurnHandler clashTurnHandler;
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
            // Start Enemy Turn
            CurrentTurn = TurnState.EnemyTurn;
            yield return StartCoroutine(enemyTurnHandler.StartTurn());

            // Start Player Turn
            CurrentTurn = TurnState.PlayerTurn;
            yield return StartCoroutine(playerTurnHandler.StartTurn());

            // Start Clash Turn
            CurrentTurn = TurnState.ClashTurn;
            yield return StartCoroutine(clashTurnHandler.StartTurn());

            //Check If Anyone Dead
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
