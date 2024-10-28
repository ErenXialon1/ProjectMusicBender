using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))] // Ensure that PlayerInput is attached
public class Player : MonoBehaviour
{
    // Player's base stats
    [Header("Player Stats")]
    [Tooltip("Current HP of the player")]
    public float playerHp = 100;

    [Tooltip("Maximum number of inputs the player can make")]
    public int playerInputCap = 6;

    // Input System Reference
    [Header("Input System")]
    [Tooltip("Reference to the PlayerInput component used for handling player controls")]
    public PlayerInput playerInput;

    // References and Lists
    [Header("Skill and Attack Settings")]
    [Tooltip("List of player attack prefabs associated with each direction")]
    public List<GameObject> playerAttacks = new List<GameObject>();

    [Tooltip("List of skills available to the player")]
    public List<SkillData> availableSkills; // Set these up in the Inspector

    // Internal data and dictionaries
    [Header("Internal Data")]
    [Tooltip("Stores the current input sequence of the player")]
    public List<string> inputSequence = new List<string>();

    [Tooltip("Map of input combinations to skill GameObjects")]
    public Dictionary<string, GameObject> skillMap;

    [Tooltip("Map of directions to corresponding sprites for display")]
    public Dictionary<string, Sprite> directionSprites; // A dictionary to map directions to sprites

    // Flags and States
    [Header("Player States")]
    [Tooltip("Indicates whether the player is executing skills")]
    public bool isExecutingSkills = false;

    // References
    [Header("UI and Display")]
    [Tooltip("Reference to the PlayerAttackPanel script that handles the UI for displaying inputs")]
    public PlayerAttackPanel playerAttackPanel;

    // Internal Skill Queue
    private List<ICommand> skillQueue = new List<ICommand>();

    void Start()
    {
        // Fetch and set up PlayerInput component
        playerInput = GetComponent<PlayerInput>();

        // Initialize skill map with predefined combinations
        // You can dynamically set this up based on your game's requirements
        skillMap = new Dictionary<string, GameObject>
        {
            { "W", playerAttacks[0] },
            { "ASD", playerAttacks[1] },
            { "WASD", playerAttacks[2] }
        };

        // Initialize directionSprites dictionary with corresponding sprites
        directionSprites = new Dictionary<string, Sprite>
        {
            { "W", playerAttacks[0].GetComponent<SpriteRenderer>().sprite },
            { "D", playerAttacks[1].GetComponent<SpriteRenderer>().sprite },
            { "A", playerAttacks[2].GetComponent<SpriteRenderer>().sprite },
            { "S", playerAttacks[3].GetComponent<SpriteRenderer>().sprite }
        };

        // Attempt to find the PlayerAttackPanel GameObject by name
        playerAttackPanel = GameObject.Find("PlayerAttackPanel")?.GetComponent<PlayerAttackPanel>();
        if (playerAttackPanel == null)
        {
            Debug.LogWarning("PlayerAttackPanel GameObject not found or missing PlayerAttackPanel component.");
        }
    }

    #region Input Handling

    /// <summary>
    /// Adds a direction input to the sequence, displays it on the UI.
    /// </summary>
    public void AddInput(string direction)
    {
        if (inputSequence.Count < playerInputCap && !isExecutingSkills)
        {
            inputSequence.Add(direction);
            playerAttackPanel.DisplayInput(inputSequence, directionSprites);
        }
    }

    // Input callbacks from Unity's Input System
    public void OnAttackUp(InputAction.CallbackContext context)
    {
        if (context.performed) AddInput("W");
    }

    public void OnAttackDown(InputAction.CallbackContext context)
    {
        if (context.performed) AddInput("S");
    }

    public void OnAttackLeft(InputAction.CallbackContext context)
    {
        if (context.performed) AddInput("A");
    }

    public void OnAttackRight(InputAction.CallbackContext context)
    {
        if (context.performed) AddInput("D");
    }

    #endregion

    #region Skill Management

    /// <summary>
    /// Confirms the input combination and queues up a matching skill for execution.
    /// </summary>
    public void ConfirmInputCombination(InputAction.CallbackContext context)
    {
        if (!isExecutingSkills && context.performed)
        {
            // Combine the input sequence into a string and find a matching skill
            string combination = string.Join("", inputSequence);
            SkillData matchingSkill = availableSkills.Find(skill => skill.combination == combination);

            if (matchingSkill != null)
            {
                // Find a target (this example uses the closest enemy)
                Transform target = FindClosestEnemy();
                if (target != null)
                {
                    // Create a skill command and add it to the queue
                    ICommand command = new SkillCommand(matchingSkill.skillPrefab, 10f, target);
                    skillQueue.Add(command);
                }
            }

            // Clear the input sequence and UI
            inputSequence.Clear();
            playerAttackPanel.ClearDisplay();
        }
    }

    /// <summary>
    /// Clears the current input sequence.
    /// </summary>
    public void ClearInputSequence(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            inputSequence.Clear();
            playerAttackPanel.ClearDisplay();
        }
    }

    /// <summary>
    /// Executes all the queued skills in order.
    /// </summary>
    public void ExecuteSkillQueue(InputAction.CallbackContext context)
    {
        if (skillQueue.Count > 0 && !isExecutingSkills && context.performed)
        {
            StartCoroutine(ExecuteSkillsInOrder());
        }
    }

    #endregion

    #region Skill Execution

    /// <summary>
    /// Coroutine to execute all queued skills with a delay between each.
    /// </summary>
    private IEnumerator ExecuteSkillsInOrder()
    {
        isExecutingSkills = true;

        foreach (var command in skillQueue)
        {
            command.Execute();
            yield return new WaitForSeconds(1f); // Adjust as necessary for timing
        }

        skillQueue.Clear();
        isExecutingSkills = false;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Finds the closest enemy to the player.
    /// </summary>
    /// <returns>Transform of the closest enemy, or null if no enemies exist.</returns>
    private Transform FindClosestEnemy()
    {
        // Get all active EnemyHealth components in the scene
        List<EnemyHealth> enemies = new List<EnemyHealth>(
            GameObject.FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None)
        );

        if (enemies.Count == 0) return null;

        Transform closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy;
    }

    #endregion
}
