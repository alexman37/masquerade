using System.Collections;
using System.Collections.Generic;

/// <summary>
/// DialogueSchedule objects are obtained through the DialogueParser.
/// A DialogueSchedule represents a single "conversation", or several of them which are related in some way
/// 
/// When playing a conversation you chew through lines in the DialogueSchedule, navigating between blocks for
/// things like choices.
/// </summary>
public class DialogueSchedule
{
    public List<DialogueBlock> dialogueBlocks;
    public string setName;

    public DialogueSchedule(List<DialogueBlock> blocks, string setName)
    {
        dialogueBlocks = blocks;
        this.setName = setName;
    }
}



/// <summary>
/// A DialogueBlock is a subsection of a conversation, which may or may not be reached depending on the choices you make.
/// 
/// When triggering dialogue, you typically specify the name of the block you want to start at.
/// If you don't specify a starting block it will search for a default block named 'def', otherwise, throw an error.
/// 
/// A DialogueBlock can have at most one choice in it- although you can have other lines of dialogue play after the choice.
/// </summary>
public class DialogueBlock
{
    public string blockName;
    public List<DialogueEntry> entries;

    public DialogueBlock(string bname, List<DialogueEntry> ls)
    {
        blockName = bname;
        entries = new List<DialogueEntry>(ls);
    }
}

/// <summary>
/// A DialogueEntry is anything the dialogue manager processes. Including:
/// - Spoken line of dialogue (DialogueLine)
/// - Choice (DialogueChoice)
/// - Command / Event to trigger (DialogueCommand)
/// </summary>
public interface DialogueEntry { }



/// <summary>
/// DialogueLines is a single thing said by a character in the text box.
/// 
/// A line will have a character speaking, a portrait for them, and a waittime.
/// 
/// See the DialogueParser docs for the full list of modifications you can make to lines-
/// like making words bold/italic, having special effects, etc.
/// </summary>
public class DialogueLine : DialogueEntry
{
    public string character;
    public string portrait;
    public int waitTime;
    public string line = "";

    public DialogueLine(string c, string p, int w, string ls)
    {
        character = c;
        portrait = p;
        waitTime = w;
        line = ls;
    }
}


/// <summary>
/// A DialogueChoice represents when you, the player, are given the option to choose what someone says
/// It may impact the rest of the conversation from there on out.
/// So text files are structured in a way that specifies "if you choose this, go here and finish out the conversation."
/// </summary>
public class DialogueChoice : DialogueEntry
{
    public List<(string opt, string disp)> blocks; // Names of blocks to go to, and how they appear in text.

    public DialogueChoice(List<(string opt, string dist)> bs)
    {
        blocks = bs;
    }
}


/// <summary>
/// DialogueGoto immediately moves to another dialogue block, specified by name.
/// 
/// Useful for splitting up long chunks of dialogue, or making them reusable
/// </summary>
public class DialogueGoTo : DialogueEntry
{
    public string gotoThis;

    public DialogueGoTo(string g)
    {
        gotoThis = g;
    }
}


/// <summary>
/// DialogueFlag enables a "flag" for use
/// 
/// Sometimes DialogueChoice's will only be available if a flag is present,
/// Sometimes DialogueLines will only be said at all if a flag is present
/// </summary>
public class DialogueFlag : DialogueEntry
{
    public string flagVarName;
    public bool on;

    public DialogueFlag(string fvn, bool t)
    {
        flagVarName = fvn;
        on = t;
    }
}