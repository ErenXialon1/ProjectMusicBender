using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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

    [Tooltip("Skills that are queued to execute")]
    public List<SkillData> skillsWaitingForExecute = new List<SkillData>(); // Skills that are queued to execute

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
    [Tooltip("Reference to the PlayerUIManager script that handles the UI for displaying inputs")]
    public PlayerUIManager uiManager;

    void Start()
    {
        // Fetch and set up PlayerInput component
        playerInput = GetComponent<PlayerInput>();
        uiManager = GameObject.Find("PlayerUI").GetComponent<PlayerUIManager>();

        // Attempt to find the PlayerUIManager GameObject by name
        if (uiManager == null)
        {
            Debug.LogWarning("PlayerUIManager script reference not set. Please assign it in the Inspector.");
        }

        InitializeDefaultSkillAddressList();
        InitializeDefaultSkills();
        InitializeDirectionSprites();
    }
    private void InitializeSkillAddress(string skillAddress)
    {
        // Assign the address strings based on your Addressable settings
        if (!skillAddresses.Contains(skillAddress)) // Ensure no duplicates in the list
        {
            skillAddresses.Add(skillAddress);   // These should match the addresses you assigned in the Addressables window
        }

    }
    private void InitializeDefaultSkills()
    {
        LoadSkill("Skills/SingleInput/BasicAttack_W");
        LoadSkill("Skills/SingleInput/BasicAttack_A");
        LoadSkill("Skills/SingleInput/BasicAttack_S");
        LoadSkill("Skills/SingleInput/BasicAttack_D");
        LoadSkill("Skills/DoubleInput/DoubleInput_WA");

    }
    /// <summary>
    /// Loads all skills from Addressables based on their addresses.
    /// </summary>
    private void LoadSkill(string address)
    {

        if (!string.IsNullOrEmpty(address)) // Ensure the address is not null or empty
        {
            Addressables.LoadAssetAsync<SkillData>(address).Completed += (handle) => OnSkillLoaded(handle, address);
        }
        else
        {
            Debug.LogWarning("Attempted to load a skill with an empty or null address.");
        }

    }

    /// <summary>
    /// Called when a SkillData ScriptableObject is loaded.
    /// </summary>
    private void OnSkillLoaded(AsyncOperationHandle<SkillData> handle, string address)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            SkillData skill = handle.Result;
            if (skill != null)
            {
                // Check if the skill is already in the availableSkills list to avoid duplicates
                bool skillAlreadyExists = availableSkills.Exists(s => s.skillName == skill.skillName);

                if (!skillAlreadyExists)
                {
                    availableSkills.Add(skill);
                    Debug.Log($"Loaded and added skill: {skill.skillName}");
                }
                else
                {
                    Debug.Log($"Skill {skill.skillName} already exists in availableSkills. Skipping.");
                }
            }
        }
        else
        {
            Debug.LogWarning($"Failed to load skill from Addressable Assets at address: {address}");
        }
    }
    public void InitializeDirectionSprites()
    {
        // Initialize the dictionary if it's not already
        if (directionSprites == null)
            directionSprites = new Dictionary<string, Sprite>();

        // Manually assign sprites directly from the project (or use a ScriptableObject setup as described previously)
        directionSprites["W"] = LoadSpriteFromPath("Sprites/UI/UpArrowKey");
        directionSprites["D"] = LoadSpriteFromPath("Sprites/UI/RightArrowKey");
        directionSprites["A"] = LoadSpriteFromPath("Sprites/UI/LeftArrowKey");
        directionSprites["S"] = LoadSpriteFromPath("Sprites/UI/DownArrowKey");
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

    /// <summary>
    /// Initializes default skills that are available to the player at the start.
    /// </summary>
    private void InitializeDefaultSkillAddressList()
    {
        InitializeSkillAddress("Skills/SingleInput/BasicAttack_W");
        InitializeSkillAddress("Skills/SingleInput/BasicAttack_A");
        InitializeSkillAddress("Skills/SingleInput/BasicAttack_S");
        InitializeSkillAddress("Skills/SingleInput/BasicAttack_D");
        InitializeSkillAddress("Skills/DoubleInput/DoubleInput_WA");

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
            uiManager.UpdateRecentInputUI(recentInputs, directionSprites);
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
    /// Confirms the current input combination as a skill if it is available and queues it up.
    /// </summary>
    public void ConfirmInputCombination(InputAction.CallbackContext context)
    {
        if (!context.performed) return; // Exit if the action wasn't fully performed

        string combination = string.Join("", recentInputs);

        // Check if the confirmed combination matches any available skill
        SkillData confirmedSkill = availableSkills.Find(skill => skill.combination == combination);

        if (confirmedSkill != null)
        {
            // Clear recent inputs after confirming
            ClearInputSequence(context);
            uiManager.ClearRecentInputUI();

            // Add the confirmed skill to the queue
            skillsWaitingForExecute.Add(confirmedSkill);



            // Add the newly confirmed skill to the list
            confirmedCombinations.Add(confirmedSkill);
            uiManager.UpdateConfirmedSkillsUI(confirmedCombinations);

            // Start executing the queued skills if not already executing
            if (!isExecutingSkills)
            {
                StartCoroutine(ProcessSkillQueue());
            }
        }
        else
        {
            // Clear the UI and recent inputs if the combination is not recognized
            ClearInputSequence(context);
            uiManager.ClearRecentInputUI();
        }
    }
    /// <summary>
    /// Coroutine to process and execute queued skills in order.
    /// </summary>
    private IEnumerator ProcessSkillQueue()
    {
        if (isExecutingSkills) yield break;
        isExecutingSkills = true;

        while (skillsWaitingForExecute.Count > 0)
        {
            // Get the next skill from the queue
            SkillData currentSkill = skillsWaitingForExecute[0];
            skillsWaitingForExecute.RemoveAt(0);



            // Example delay to simulate execution (replace with actual execution logic)
            // yield return new WaitForSeconds(1f);
            yield return ExecuteSkillCoroutine(currentSkill);
            // After finishing the current skill, continue to the next one if available
        }

        isExecutingSkills = false;
    }
    /// <summary>
    /// Clears the recent input list.
    /// </summary>
    public void ClearInputSequence(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            recentInputs.Clear();
            uiManager.ClearRecentInputUI();
        }
    }


    #endregion

    #region Skill Execution

    /// <summary>
    /// Coroutine to execute all queued skills with a delay between each.
    /// </summary>

    private IEnumerator ExecuteSkillCoroutine(SkillData skill)
    {


        // Instantiate or play the skill's effect/animation
        Debug.Log($"Executing skill: {skill.skillName}");
        // Example: Play animation or instantiate effect here, based on your game's design
        // yield return new WaitForSeconds(skill.animationDuration); // Adjust based on your skill's effect
        // After skill execution, initiate a delay before removing the skill from the UI
        StartCoroutine(RemoveExecutedSkillFromUI(skill));
        yield return new WaitForSeconds(1f); // Example delay




    }
    /// <summary>
    /// Coroutine to remove the executed skill's image from the UI after a delay.
    /// </summary>
    private IEnumerator RemoveExecutedSkillFromUI(SkillData skill)
    {
        int index = confirmedCombinations.IndexOf(skill);
        if (index == -1) yield break; // Exit if the skill is not found in the list

        // Get the UI block for the confirmed skill
        Image skillImage = uiManager.confirmedSkillUIBlocks[index].GetComponent<Image>();
        RectTransform skillRect = skillImage.GetComponent<RectTransform>();

        // Fade out the image and move it upwards simultaneously
        skillImage.DOFade(0f, 0.25f); // Fade out over 0.25 seconds
        skillRect.DOAnchorPosY(skillRect.anchoredPosition.y + 30f, 0.25f); // Move upwards by 30 units over 0.25 seconds

        // Wait for the animation to complete
        yield return new WaitForSeconds(0.25f);

        // After the fade and move animation is complete, reset the image and update the UI
        skillImage.sprite = null;
        skillImage.DOFade(1f, 0f); // Reset the alpha to fully opaque (in case it's reused)
        skillRect.anchoredPosition = new Vector2(skillRect.anchoredPosition.x, skillRect.anchoredPosition.y - 30f); // Reset the position

        // Remove the skill from the confirmed combinations list and update the UI
        confirmedCombinations.RemoveAt(index);
        uiManager.UpdateConfirmedSkillsUI(confirmedCombinations);
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