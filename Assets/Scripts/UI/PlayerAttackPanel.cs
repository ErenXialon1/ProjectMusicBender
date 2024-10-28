using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerAttackPanel : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("List of UI blocks (GameObjects with Image components) to display inputs.")]
    public List<GameObject> spriteBlocks; // Assign these in the Inspector

    #region Display Methods

    /// <summary>
    /// Displays the current input sequence on the panel using the given direction sprites.
    /// </summary>
    /// <param name="inputSequence">The list of input directions.</param>
    /// <param name="directionSprites">Dictionary mapping directions to corresponding sprites.</param>
    public void DisplayInput(List<string> inputSequence, Dictionary<string, Sprite> directionSprites)
    {
        // Loop through the spriteBlocks to update the displayed sprites based on the input sequence
        for (int i = 0; i < spriteBlocks.Count; i++)
        {
            if (i < inputSequence.Count)
            {
                string direction = inputSequence[i];
                if (directionSprites.TryGetValue(direction, out Sprite sprite))
                {
                    spriteBlocks[i].GetComponent<Image>().sprite = sprite;
                }
            }
            else
            {
                // Clear the remaining blocks if inputSequence is shorter than spriteBlocks count
                spriteBlocks[i].GetComponent<Image>().sprite = null;
            }
        }
    }

    /// <summary>
    /// Clears the displayed input sprites from the panel.
    /// </summary>
    public void ClearDisplay()
    {
        foreach (var block in spriteBlocks)
        {
            block.GetComponent<Image>().sprite = null;
        }
    }

    #endregion
}
