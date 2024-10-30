using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public List<GameObject> recentInputUIBlocks; // UI blocks to display recent inputs
    public List<GameObject> confirmedSkillUIBlocks; // UI blocks to display confirmed skills

    /// <summary>
    /// Updates the UI to display the player's recent inputs.
    /// </summary>
    public void UpdateRecentInputUI(List<string> recentInputs, Dictionary<string, Sprite> directionSprites)
    {
        for (int i = 0; i < recentInputUIBlocks.Count; i++)
        {
            if (i < recentInputs.Count)
            {
                string direction = recentInputs[i];
                if (directionSprites.TryGetValue(direction, out Sprite sprite))
                {
                    recentInputUIBlocks[i].GetComponent<Image>().sprite = sprite;
                }
            }
            else
            {
                recentInputUIBlocks[i].GetComponent<Image>().sprite = null;
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
            confirmedSkillUIBlocks[index].GetComponent<Image>().sprite = confirmedSkill.skillImage;
        }
    }

    /// <summary>
    /// Discards the oldest confirmed skill from the UI and shifts the remaining items.
    /// </summary>
    public void DiscardConfirmedSkillFromUI(int startIndex)
    {
        // Shift all items to the left in the UI blocks
        for (int i = startIndex; i < confirmedSkillUIBlocks.Count - 1; i++)
        {
            confirmedSkillUIBlocks[i].GetComponent<Image>().sprite = confirmedSkillUIBlocks[i + 1].GetComponent<Image>().sprite;
        }

        // Clear the last item
        confirmedSkillUIBlocks[confirmedSkillUIBlocks.Count - 1].GetComponent<Image>().sprite = null;
    }

    /// <summary>
    /// Updates the entire confirmed skills UI to reflect the confirmed combinations list.
    /// </summary>
    public void UpdateConfirmedSkillsUI(List<SkillData> confirmedSkills)
    {
        int maxDisplayCount = confirmedSkillUIBlocks.Count;

        // Loop through the UI blocks to display the first 'maxDisplayCount' skills from the confirmed skills list
        for (int i = 0; i < maxDisplayCount; i++)
        {
            if (i < confirmedSkills.Count)
            {
                confirmedSkillUIBlocks[i].GetComponent<Image>().sprite = confirmedSkills[i].skillImage;
            }
            else
            {
                confirmedSkillUIBlocks[i].GetComponent<Image>().sprite = null;
            }
        }
    }

    /// <summary>
    /// Clears the UI for recent inputs.
    /// </summary>
    public void ClearRecentInputUI()
    {
        foreach (var block in recentInputUIBlocks)
        {
            block.GetComponent<Image>().sprite = null;
        }
    }
}


