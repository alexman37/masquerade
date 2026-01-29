using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* Dialogue Parser is a static class that parses dialogue files, and returns a "DialogueSchedule"- an object that can be used by the DialogueManager.
 * 
 * These files are divided into one or more "blocks", which are specified like so:
 *   /BLOCK=(name)
 *      ...
 *   /ENDBLOCK
 * Blocks may or may not be used depending on choices you make in a conversation.
 * You ususally specify what block to start at, but if you don't, it looks for a block named "def" (default)
 * 
 *  In blocks, lines of dialogue are formatted in the following way:
        CHAR(portrait)[wait]:line
     Where
        - "CHAR" is the character speaking - e.g. HAZEL, WINTER.
        - "portrait" is the name of one of their portraits in the sliced spritesheet - by default, they're just numbers
          - If a portrait of that name was not found, use a designated default and log an error.
        - "wait" is optional. If it's specified, the dialogue box will temporarily go away and then reappear after this many seconds.
          - Used to indicate a shift in conversation / topic.
        - "line" is the actual line of dialogue the character will say.


      CONDITIONALS
        Conditionals check for 'global' variables, such as a character's mood, or whether or not they are even alive.
        TODO

      CHOICES
        Choices are when the player is given the chance to say the next line.
        They are specified like so:
            /CHOICE
                (name):(display)
                ...
            /ENDCHOICE
        - Where name is the name of this choice (AND the name of the block it represents),
            - And display is how it appears to the player.
        - When the choice is hit in parsing it will immediately appear on screen.
            - Some other text may follow afterwards before going to the next block.
 */


public class DialogueParser
{
    /// <summary>
    /// Parse a simple, raw text file. There will only be talking between characters and associated effects
    /// </summary>
    public static DialogueSchedule parseDialogue(TextAsset txtFile)
    {
        return parsingProcess(txtFile);
    }



    /// <summary>
    /// 
    /// </summary>
    private static DialogueSchedule parsingProcess(TextAsset txtFile)
    {
        string[] raw = txtFile.text.Split('\n');
        string currBlock = null;
        List<DialogueBlock> dialogueBlocks = new List<DialogueBlock>();
        List<DialogueEntry> dialogueEntries = new List<DialogueEntry>();

        bool inChoice = false;
        List<(string opt, string disp)> choiceOptions = new List<(string opt, string disp)>();
        DialogueChoice choice = null;

        for (int i = 0; i < raw.Length; i++)
        {
            string currLine = raw[i];
            currLine = currLine.Trim();

            // Ignore comments.
            if (currLine.Length > 0 && currLine[0] != '#')
            {
                // Events / Commands not processed in the simple version.
                if(currLine[0] == '$')
                {
                    Debug.LogWarning("The dialogue file " + txtFile.name + " could not run command -- " + raw[i] + " -- please specify it as a ScriptedDialogueFile");
                }
                else
                {
                    // Start of a new block.
                    if (currLine.Contains("/BLOCK="))
                    {
                        currBlock = currLine.Split('=')[1];
                    }
                    //Ignore everything before the first block.
                    else if (currBlock != null)
                    {
                        //Check for directives.
                        if (currLine.Contains("/ENDBLOCK"))
                        {
                            DialogueBlock dBlock = new DialogueBlock(currBlock, dialogueEntries);
                            dialogueBlocks.Add(dBlock);
                            dialogueEntries.Clear();
                            currBlock = null;
                        }

                        else if (currLine.Contains("/CHOICE"))
                        {
                            inChoice = true;
                        }

                        else if (currLine.Contains("/ENDCHOICE"))
                        {
                            inChoice = false;
                            choice = new DialogueChoice(new List<(string opt, string disp)>(choiceOptions));
                            dialogueEntries.Add(choice);
                            choiceOptions.Clear();
                        }

                        else if (currLine.Contains("/GOTO"))
                        {
                            string gotoThis = currLine.Split('=')[1];
                            dialogueEntries.Add(new DialogueGoTo(gotoThis));
                        }

                        else if (currLine.Contains("/FLAG"))
                        {
                            string flagThisVar = currLine.Split(' ')[1];
                            dialogueEntries.Add(new DialogueFlag(flagThisVar, true));
                        }

                        //Otherwise, this is an actual spoken line of dialogue.
                        else if (!inChoice)
                        {
                            int idx1 = currLine.IndexOf('(');
                            int idx2 = currLine.IndexOf(')');
                            int idx3 = currLine.IndexOf('[');
                            int idx4 = currLine.IndexOf(']');
                            int idx5 = currLine.IndexOf(':');

                            string charSpeaking, charPortraitName;
                            if(idx1 != -1)
                            {
                                charSpeaking = currLine.Substring(0, idx1);
                                charPortraitName = currLine.Substring(idx1 + 1, idx2 - idx1 - 1);
                            } else
                            {
                                charSpeaking = currLine.Substring(0, idx5);
                                charPortraitName = "0";
                            }
                            int charWaitTime = idx3 == -1 ? 0 : System.Int32.Parse(currLine.Substring(idx3 + 1, (idx4 - idx3) - 1));
                            string spoken = currLine.Substring(idx5 + 1);

                            DialogueLine newLine = new DialogueLine(charSpeaking, charPortraitName, charWaitTime, spoken);
                            dialogueEntries.Add(newLine);
                        }


                        // In choices, check has to come after all the directives checking...
                        else
                        {
                            string[] choiceParse = currLine.Split(':');
                            if (choiceParse.Length > 1)
                            {
                                choiceOptions.Add((choiceParse[0], choiceParse[1]));
                            }
                        }
                    }
                }
            }

        }

        return new DialogueSchedule(dialogueBlocks, txtFile.name);
    }
}
