using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("UI blocks to display recent inputs")]
    public List<GameObject> recentInputUIBlocks;

    [Tooltip("UI blocks to display confirmed skills")]
    public List<GameObject> confirmedSkillUIBlocks;

    [SerializeField, Tooltip("Reference to the Player script")]
    private Player playerScript;

    [SerializeField, Tooltip("Capacity of UI slots for confirmed skills")]
    private int uiCapacity = 6;

    /// <summary>
    /// Updates the UI to display the player's recent inputs.
    /// </summary>
    public void UpdateRecentInputUI(List<string> recentInputs, Dictionary<string, Sprite> directionSprites)
    {
        int inputsCount = Mathf.Min(recentInputs.Count, recentInputUIBlocks.Count);

        for (int i = 0; i < recentInputUIBlocks.Count; i++)
        {
            var image = recentInputUIBlocks[i].GetComponent<Image>();
            if (i < inputsCount && directionSprites.TryGetValue(recentInputs[i], out Sprite sprite))
            {
                image.sprite = sprite;
                image.DOFade(1f, 0.25f); // Fade in to fully opaque
            }
            else
            {
                image.sprite = null;
                image.DOFade(0f, 0.25f); // Fade out to fully transparent
            }
        }
    }

    /// <summary>
    /// Adds a confirmed skill to the UI.
    /// </summary>
    public void AddConfirmedSkillToUI(SkillData confirmedSkill, int index)
    {
        if (index < confirmedSkillUIBlocks.Count)
        {
            Image skillImage = confirmedSkillUIBlocks[index].GetComponent<Image>();
            skillImage.sprite = confirmedSkill.skillImage;
            skillImage.DOFade(1, 0); // Ensure visibility
        }
    }

    /// <summary>
    /// Discards the oldest confirmed skill from the UI and shifts the remaining items.
    /// </summary>
    public void DiscardConfirmedSkillFromUI(int startIndex)
    {
        for (int i = startIndex; i < confirmedSkillUIBlocks.Count - 1; i++)
        {
            var currentImage = confirmedSkillUIBlocks[i].GetComponent<Image>();
            var nextImage = confirmedSkillUIBlocks[i + 1].GetComponent<Image>();
            currentImage.sprite = nextImage.sprite;
        }

        confirmedSkillUIBlocks[^1].GetComponent<Image>().sprite = null; // Clear last item
    }

    /// <summary>
    /// Updates the entire confirmed skills UI to reflect the confirmed combinations list.
    /// </summary>
    public void UpdateConfirmedSkillsUI(List<SkillData> confirmedSkills)
    {


        int maxDisplay = Mathf.Min(uiCapacity, confirmedSkillUIBlocks.Count);

        for (int i = 0; i < maxDisplay; i++)
        {
            var image = confirmedSkillUIBlocks[i].GetComponent<Image>();
            if (i < confirmedSkills.Count)
            {
                image.sprite = confirmedSkills[i].skillImage;
                image.DOFade(1, 0); // Ensure it is fully visible
            }
            else
            {
                image.sprite = null;
                image.DOFade(0, 0); // Hide unused slots
            }
        }
    }
    
    /// <summary>
    /// Coroutine to remove the executed skill's image from the UI after a delay.
    /// </summary>
    public IEnumerator RemoveExecutedSkillFromUI(SkillData skill)
    {
       
        int index = playerScript.confirmedCombinations.IndexOf(skill);
        if (index == -1) yield break; // Exit if the skill is not found in the list
        float fadingTime = 0.25f;
        // Get the UI block for the confirmed skill
        Image skillImage = confirmedSkillUIBlocks[index].GetComponent<Image>();
        RectTransform skillRect = skillImage.GetComponent<RectTransform>();

        // Fade out the image and move it upwards simultaneously
        skillImage.DOFade(0f, fadingTime ); // Fade out over 0.25 seconds
        skillRect.DOAnchorPosY(skillRect.anchoredPosition.y + 30f, fadingTime); // Move upwards by 30 units over 0.25 seconds

        // Wait for the animation to complete
        yield return new WaitForSeconds(fadingTime);

        // After the fade and move animation is complete, reset the image and update the UI
        skillImage.sprite = null;
        skillImage.DOFade(1f, 0f); // Reset the alpha to fully opaque (in case it's reused)
        skillRect.anchoredPosition = new Vector2(skillRect.anchoredPosition.x, skillRect.anchoredPosition.y - 30f); // Reset the position

        // Remove the skill from the confirmed combinations list and update the UI
        playerScript.confirmedCombinations.RemoveAt(index);
        UpdateConfirmedSkillsUI(playerScript.confirmedCombinations);
    }

    /// <summary>
    /// Clears the UI for recent inputs.
    /// </summary>
    public void ClearRecentInputUI()
    {
        foreach (var block in recentInputUIBlocks)
        {
            var image = block.GetComponent<Image>();
            if (image != null)
            {
                image.DOFade(0f, 0.25f); // Fade out to fully transparent
            }
        }
    }
}