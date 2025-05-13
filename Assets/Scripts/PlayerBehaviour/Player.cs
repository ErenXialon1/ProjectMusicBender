using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using DG.Tweening;



[RequireComponent(typeof(PlayerInput))] // Ensure that PlayerInput is attached
public class Player : MonoBehaviour
{
    
    // Player's base stats
    [Header("Player Stats")]
    [Tooltip("Current HP of the player")]
    public float playerHp = 100;

    [Tooltip("Maximum number of inputs the player can make")]
    public int playerInputCap = 4; // Default input cap, can increase later
    [Header("Addressable Skill References")]
    public List<string> skillAddresses; // List of addresses for ScriptableObjects
    [SerializeField] private PlayerStatSheet statSheet; // Player’s stat sheet
      // Player’s StatSheet for managing stats and health
    
    // Input System Reference
    [Header("Input System")]
    [Tooltip("Reference to the PlayerInput component used for handling player controls")]
    public PlayerInput playerInput;

    // Input Data
    [Header("Input Data")]
    [Tooltip("List of recent inputs the player has made")]
    public List<string> recentInputs = new List<string>(); // Holds the player's recent inputs (up to cap)

    [Tooltip("Confirmed skills to show in UI")]
    public List<SkillData> confirmedCombinations = new List<SkillData>(); // Confirmed skills to show in UI

    // Input Data
    [Header("Input Data")]
    private Queue<List<string>> playerQueue = new Queue<List<string>>(); // Queue of chains for the turn
    private List<string> currentChain = new List<string>(); // Temporarily holds the ongoing chain
    //private bool isChaining = false; // Tracks whether the player is chaining combinations(OLD SYSTEM)

    // References
    [Header("Skill and Attack Settings")]
    [Tooltip("List of skills available to the player")]
    public List<SkillData> availableSkills; // Set these up in the Inspector

    [Tooltip("Map of directions to corresponding sprites for display")]
    public Dictionary<string, Sprite> directionSprites; // Map of input directions to sprites

    // Flags and States
    [Header("Player States")]
    [Tooltip("Indicates whether the player is executing skills")]
    public bool isExecutingSkills = false;

    [Header("UI and Display")]
    [SerializeField, Tooltip("Reference to the PlayerUIManager script that handles the UI for displaying inputs")]
    private PlayerUIManager uiManager;
    private RuneRock currentRuneRock; // Holds the active RuneRock
    [SerializeField]private PlayerTurnHandler playerTurnHandler;


    void Start()
    {
        
        // Fetch and set up PlayerInput component
        playerInput = GetComponent<PlayerInput>();
        statSheet = GetComponent<PlayerStatSheet>(); // Ensure this component is attached
        
        if (statSheet == null)
        {
            Debug.LogWarning("StatSheet component not found on Player. Please add StatSheet to the Player object.");
        }
        // Attempt to find the PlayerUIManager GameObject by name
        if (uiManager == null)
        {
            Debug.LogWarning("PlayerUIManager script reference not set. Please assign it in the Inspector.");
        }

        
        InitializeDefaultDirectionSprites();
        
    }
   
    private void OnDisable()
    {
        UnsubscribeInputEvents();
    }
    public void UnsubscribeInputEvents()
    {
        playerInput.actions["UpAttackInFight"].performed -= ctx => AddInput("W"); 
        playerInput.actions["DownAttackInFight"].performed -= ctx => AddInput("S"); 
        playerInput.actions["LeftAttackInFight"].performed -= ctx => AddInput("A"); 
        playerInput.actions["RightAttackInFight"].performed -= ctx => AddInput("D"); 
        playerInput.actions["ConfirmAttack"].performed -= ctx => ConfirmInputCombination();
        playerInput.actions["AttackInputClear"].performed -= ctx => ClearInputSequence();
        //playerInput.actions["ChainAttack"].performed -= ctx => ChainInputCombination();


    }
    
    public IEnumerator SubscribeInputEvents()
    {
        yield return new WaitForEndOfFrame(); // Wait for one frame
        yield return new WaitForEndOfFrame();
        

        // Input event subscriptions
        playerInput.actions["UpAttackInFight"].performed += ctx => AddInput("W"); 
        playerInput.actions["DownAttackInFight"].performed += ctx => AddInput("S"); 
        playerInput.actions["LeftAttackInFight"].performed += ctx => AddInput("A"); 
        playerInput.actions["RightAttackInFight"].performed += ctx => AddInput("D");
        playerInput.actions["ConfirmAttack"].performed += ctx =>ConfirmInputCombination();
        playerInput.actions["AttackInputClear"].performed += ctx =>ClearInputSequence();
        //playerInput.actions["ChainAttack"].performed += ctx => ChainInputCombination();

    }
    /*private void InitializeSkillAddress(string skillAddress)
    {
        // Assign the address strings based on your Addressable settings
        if (!skillAddresses.Contains(skillAddress)) // Ensure no duplicates in the list
        {
            skillAddresses.Add(skillAddress);   // These should match the addresses you assigned in the Addressables window
        }

    }*/
    public void ResetAvailableSkillList()
    {
        availableSkills.Clear();
    }

    

    private void InitializeDefaultDirectionSprites()
    {
        directionSprites = directionSprites ?? new Dictionary<string, Sprite>
    {
        { "W", LoadSpriteFromPath("Sprites/UI/UpArrowKey") },
        { "D", LoadSpriteFromPath("Sprites/UI/RightArrowKey") },
        { "A", LoadSpriteFromPath("Sprites/UI/LeftArrowKey") },
        { "S", LoadSpriteFromPath("Sprites/UI/DownArrowKey") }
    };
    }

    // Helper method to load sprites from a specific path (for illustration purposes)
    private Sprite LoadSpriteFromPath(string path)
    {
        Sprite sprite = Addressables.LoadAssetAsync<Sprite>(path).WaitForCompletion();
        if (sprite == null)
        {
            Debug.LogWarning($"Sprite not found at path: {path}");
        }
        return sprite;
    }

   


    #region Input Handling

    /// <summary>
    /// Adds a direction input to the recent inputs list and updates the UI.
    /// </summary>
    public void AddInput(string direction)
    {
        if (recentInputs.Count < playerInputCap)
        {
            

           
            recentInputs.Add(direction);
            Vector3 directionVector = GetDirectionVector(direction);
            currentRuneRock.AddEngravePoint(directionVector, 0.2f);
            uiManager.UpdateRecentInputUI(recentInputs, directionSprites);
        }
    }
   

    

    

    #endregion
    #region Turn Management

    /// <summary>
    /// Finalizes the player's turn by confirming any remaining inputs.
    /// </summary>
    public void FinalizeTurn()
    {
        if (recentInputs.Count > 0)
        {
            
            
            ConfirmInputCombination();// Automatically confirm the current combination if there are unconfirmed inputs

        }

        

        
    }

    #endregion
    #region Skill Management

    /// <summary>
    /// Confirms the current input combination as a skill if it is available and queues it up.
    /// </summary>
    public void ConfirmInputCombination()
    {
        if (recentInputs.Count == 0) return;

        string combination = string.Join("", recentInputs);
        // Add as a standalone chain
        currentChain.Add(combination);
        playerQueue.Enqueue(new List<string>(currentChain)); // Enqueue a copy of the chain
        currentChain.Clear(); // Reset for the next combination

        ClearInputSequence();
        
    }
    public Queue<List<string>> GetPlayerQueue()
    {
        return new Queue<List<string>>(playerQueue); // Return a copy of the queue
    }

   /* 
    * OLD CHAINING SYSTEM
    * public void ChainInputCombination()
    {
        if (recentInputs.Count == 0) return;

        // Add the current combination to the chain
        string combination = string.Join("", recentInputs);
        currentChain.Add(combination);

        ClearInputSequence();
        isChaining = true; // Stay in chaining mode
    }
   
    */
    /// <summary>
    /// Clears the recent input list.
    /// </summary>
    public void ClearInputSequence()
    {
        
            recentInputs.Clear();
            uiManager.ClearRecentInputUI();
        
    }
    public void ClearCombinationQueue()
    {
        playerQueue.Clear();
        currentChain.Clear();
        
        
    }

    #endregion



    #region Helper Methods
    public SkillData GetSkillByCombination(string combination)
    {
        return availableSkills.Find(skill => skill.combination == combination);
    }
    /// <summary>
    /// Finds the closest enemy to the player.
    /// </summary>
    /// <returns>Transform of the closest enemy, or null if no enemies exist.</returns>
    private Transform FindClosestEnemy()
    {
        // Get all active EnemyHealth components in the scene
        List<EnemyStatSheet> enemies = new List<EnemyStatSheet>(
            GameObject.FindObjectsByType<EnemyStatSheet>(FindObjectsSortMode.None)
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
    private Vector3 GetDirectionVector(string input)
    {
        // Convert input string to a direction vector
        return input switch
        {
            "W" => Vector3.up,
            "A" => Vector3.left,
            "S" => Vector3.down,
            "D" => Vector3.right,
            _ => Vector3.zero
        };
    }


    #endregion
}