using UnityEngine;
using System;
using TMPro;

/// <summary>
/// Attach this script as the component of a dialogue choice button GameObject. When you reach a choice event in the dialogue, and
/// instantiate a button for each available option, each option has the name of a block to go to if you select it.
/// </summary>
public class DialogueChoiceButton : MonoBehaviour
{
    public static event Action<string> dialogueChoiceMade;
    public string dialogueChoiceBlock;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize event
        dialogueChoiceMade += (s) => { };
    }

    /// <summary>
    /// Call this when the choice is selected.
    /// </summary>
    public void buttonPressed()
    {
        dialogueChoiceMade.Invoke(dialogueChoiceBlock);
    }

    /// <summary>
    /// When preparing dialogue choices, provide the name of the dialogue block to go to if it is selected (opt) and what
    /// the dialogue button actually "says" (disp)
    /// </summary>
    public void setButton(string opt, string disp)
    {
        dialogueChoiceBlock = opt;
        this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = disp;
    }
}
