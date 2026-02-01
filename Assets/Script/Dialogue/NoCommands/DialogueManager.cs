using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

/* DialogueManager is the class that actually goes about displaying dialogue on the screen.
 * 
 * It adjusts the dialogue setup/box as necessary, and draws/manages the character portrait.
 * It also handles visual effects such as Text type, the symbols, and emphasized letters.
 * 
 * For it to work, it needs a "DialogueSet" object that specifies all of this.
 * DialogueSets are obtained by parsing a dialogue text file, which is done by the DialogueParser.
*/

//Why not static?: DialogueManager has to communicate with UI objects.
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    // Must specify the portrait, text area and template for choices
    public GameObject dialogueContainer;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public Button dialogueChoice;

    public GameObject controlButtons;
    public GameObject notebookButton;
    List<Button> newChoiceButtons = new List<Button>();

    // Flags persist across all conversations.
    HashSet<string> flags = new HashSet<string>();

    DialogueSchedule currSchedule = null;
    DialogueBlock currBlock;
    string nextBlock = null;
    int entryInBlock = 0;

    bool finishedDialogueLine = false;
    bool advanceAutomatically = false;
    Coroutine listenForInput;

    // Need to know whether or not we should do a transition
    string lastCharToSpeak = "";

    public static event Action anyClick;

    private void Start()
    {
        if (instance == null) instance = this;
        else
         {  Destroy(this);}

        // Get actions started
        anyClick += () => { };
    }

    private void OnEnable()
    {
        DialogueChoiceButton.dialogueChoiceMade += makeChoice;
    }

    private void OnDisable()
    {
        DialogueChoiceButton.dialogueChoiceMade -= makeChoice;
    }

    private void OnDestroy()
    {
        DialogueChoiceButton.dialogueChoiceMade -= makeChoice;
    }

    /// <summary>
    /// When you start using dialogue, you'll need to constantly listen for clicks and other ways of advancing dialogue
    /// </summary>
    private IEnumerator listenForInputCoroutine()
    {
        while(currSchedule != null)
        {
            if (finishedDialogueLine && Input.GetMouseButtonDown(0))
            {
                finishedDialogueLine = false;
                advanceAutomatically = false;
                anyClick.Invoke();
            }
            else if (advanceAutomatically)
            {
                finishedDialogueLine = false;
                advanceAutomatically = false;
                anyClick.Invoke();
            }
            yield return null;
        }
    }

    /// <summary>
    /// Start with the first line of dialogue in the specified block
    /// </summary>
    public void processConversation(TextAsset txt, string startingBlock)
    {
        if(currSchedule == null)
        {
            DialogueSchedule dialogue = DialogueParser.parseDialogue(txt);
            currSchedule = dialogue;
            currBlock = currSchedule.dialogueBlocks.Find(block => block.blockName == startingBlock);

            // If no block is specified look for a block named "def"
            if (currBlock == null)
            {
                currBlock = currSchedule.dialogueBlocks.Find(block => block.blockName == "def");
                if (currBlock == null)
                {
                    Debug.LogError("Could not find dialogue block " + startingBlock + " - specify it or use a default named 'def'!");
                    endConversation();
                    return;
                }
            }

            anyClick += advanceDialogue;
            startConversation();
        } else
        {
            Debug.LogError("Could not process conversation " + txt.name + " - a conversation is already in progress");
        }
    }

    // Either display the next line of dialogue in this block or go to the next block (or end the conversation)
    private void advanceDialogue()
    {
        dialogueText.text = "";

        // If you reach the end of a dialogue block, you have to either advance to the next one or end the conversation
        if(entryInBlock == currBlock.entries.Count - 1)
        {
            if(nextBlock != null)
            {
                gotoBlock(nextBlock);
                nextBlock = null;
            }
            // No next block: end of conversation
            else
            {
                endConversation();
            }
        } 
        // Otherwise, simply go to the next dialogue item in the current block.
        else
        {
            StartCoroutine(display(currBlock.entries[++entryInBlock]));
        }
    }

    private void gotoBlock(string newBlock)
    {
        currBlock = currSchedule.dialogueBlocks.Find(block => block.blockName == newBlock);
        if (currBlock != null)
        {
            entryInBlock = 0;
            StartCoroutine(display(currBlock.entries[0]));
        }
        else
        {
            Debug.LogError("Abandoning conversation, no block named " + newBlock + " was found");
            endConversation();
        }
    }

    // Takes necessary visual actions to display a line of dialogue- such as set character portrait
    IEnumerator display(DialogueEntry entry)
    {
        if(entry is DialogueLine)
        {
            DialogueLine dl = entry as DialogueLine;
            // If there is a wait time associated with this line, then wait before deploying it
            if (dl.waitTime > 0)
            {
                dialogueContainer.SetActive(false);
                yield return new WaitForSeconds(dl.waitTime);
                dialogueContainer.SetActive(true);
            }

            //speakerText.text = dl.character;

            // Set sprite, and text
            // TODO: Play talking animations here

            // Text type animation
            yield return textType(dl.line);

            //You must wait a small amount of time before advancing to the next line of dialogue on clicks
            yield return new WaitForSeconds(0.5f);
            finishedDialogueLine = true;

        } else if(entry is DialogueChoice)
        {
            DialogueChoice dc = entry as DialogueChoice;
            deployChoice(dc);
        } else if(entry is DialogueGoTo)
        {
            DialogueGoTo dg = entry as DialogueGoTo;
            gotoBlock(dg.gotoThis);

        } else if(entry is DialogueFlag)
        {
            DialogueFlag df = entry as DialogueFlag;

            if (df.on) flags.Add(df.flagVarName);
            else flags.Remove(df.flagVarName);

            advanceDialogue();
        }
        yield return null;
    }

    // Text type animation
    IEnumerator textType(string line)
    {
        float timeBetweenTypes = 0.005f;
        string activeDirective = "";
        Dictionary<string, string> charHexCodes = new Dictionary<string, string>()
        {
            { "0", "#aaaaaa" }, //inner monologue
            { "1", "#418c49" }, //green
            { "2", "#007f7f" }, //teal
            { "3", "#ef5c00" }, //orange
            { "4", "#642f7c" }, //purple
            { "5", "#7f6b06" }, //gold
            { "6", "#992a2a" }, //crimson
            { "7", "#ffffff" }, //white
            { "8", "#000000" }, //black
            { "9", "#aa0000" }, //red
        };

        //TODO: Can only handle one directive at a time, per the moment
        char[] chars = line.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            char curr = chars[i];
            if (curr == '\\')
            {
                char code = chars[i + 1];
                // Newline (Not a directive)
                if(code == 'n')
                {
                    dialogueText.text = dialogueText.text + '\n';
                }
                // Change color (refer to map)
                if (code > 47 && code < 58)
                {
                    dialogueText.text = dialogueText.text + "<color=" + charHexCodes[code.ToString()] + ">";
                    activeDirective = "color";
                }
                // Bold
                if (code == 'b')
                {
                    dialogueText.text = dialogueText.text + "<b>";
                    activeDirective = "b";
                }
                // Italic
                if (code == 'i')
                {
                    dialogueText.text = dialogueText.text + "<i>";
                    activeDirective = "i";
                }
                // Small
                if (code == 's')
                {
                    dialogueText.text = dialogueText.text + "<sub>";
                    activeDirective = "sub";
                }
                // End directive
                if (code == '!')
                {
                    dialogueText.text = dialogueText.text + "</" + activeDirective + ">";
                }
                i++;
            }
            else
            {
                dialogueText.text = dialogueText.text + curr;
            }
            yield return new WaitForSeconds(timeBetweenTypes);
            
        }

        yield return null;
    }

    // Put a choice onto the screen. Wait and see which choice the player picks. Then set it as "nextBlock"
    // TODO automatically expand to the size of a choice
    private void deployChoice(DialogueChoice dc)
    {
        anyClick -= advanceDialogue;

        int numChoices = dc.blocks.Count;
        float additionalOffset = 10f;
        float offset = dialogueChoice.image.rectTransform.rect.height + additionalOffset;
        float xPos = dialogueChoice.image.rectTransform.anchoredPosition.x;

        // First we have to put the choices onto the screen- this requires a bit of math
        float startPosY = dialogueChoice.image.rectTransform.anchoredPosition.y;
        float adjustedStartPos = startPosY/* + ((offset / 2) * (numChoices - 1))*/;

        int usedIndex = 0;
        for (int i = 0; i < numChoices; i++)
        {
            if(dc.blocks[i].flag == null || flags.Contains(dc.blocks[i].flag))
            {
                Button copy = GameObject.Instantiate(dialogueChoice, dialogueContainer.transform);
                RectTransform copyRT = copy.image.GetComponent<RectTransform>();

                // Set the button's position and values
                copyRT.anchoredPosition = new Vector2(xPos, adjustedStartPos - usedIndex * offset);
                copy.GetComponent<DialogueChoiceButton>().setButton(dc.blocks[i].opt, dc.blocks[i].disp);

                usedIndex++;

                newChoiceButtons.Add(copy);
                copy.gameObject.SetActive(true);
            } else
            {
                Debug.Log("Failed flag check for " + dc.blocks[i].flag);
            }
        }
    }

    // When a choice is made, clear it from the screen and advance the conversation
    private void makeChoice(string choice)
    {
        nextBlock = choice;
        foreach(Button b in newChoiceButtons)
        {
            Destroy(b.gameObject);
        }
        newChoiceButtons.Clear();

        anyClick += advanceDialogue;
        advanceAutomatically = true;
    }




    // Begin a normal click-through conversation
    private void startConversation()
    {
        dialogueContainer.SetActive(true);
        StartCoroutine(display(currBlock.entries[0]));
        listenForInput = StartCoroutine(listenForInputCoroutine());
    }

    // End this conversation, return to gameplay
    private void endConversation()
    {
        StopCoroutine(listenForInput);
        anyClick -= advanceDialogue;
        currSchedule = null;
        currBlock = null;
        nextBlock = null;
        entryInBlock = 0;
        dialogueContainer.SetActive(false);
        controlButtons.SetActive(true);
        notebookButton.SetActive(true);
    }

    // Short circuit on scene transitions - just quietly dismantle the conversation
    private void endFromSceneTransition()
    {
        anyClick -= advanceDialogue;
        currSchedule = null;
        currBlock = null;
        nextBlock = null;
        entryInBlock = 0;
    }
}