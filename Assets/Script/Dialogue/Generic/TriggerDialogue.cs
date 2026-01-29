using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{
    public TextAsset rawFile;
    public string startingBlock;

    public void processConversation()
    {
        DialogueManager.instance.processConversation(rawFile, startingBlock);
    }
}
