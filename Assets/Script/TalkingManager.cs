using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkingManager : MonoBehaviour
{
    public TextAsset[] characterDialogueFiles;
    private int currCharacter;

    private void OnEnable()
    {
        CameraManager.cameraTransition += startListening;
    }

    private void OnDisable()
    {
        CameraManager.cameraTransition -= startListening;
    }

    public void beginDialogueForCurrent()
    {
        // TODO: Don't start at start block if not your first time talking to them
        DialogueManager.instance.processConversation(characterDialogueFiles[currCharacter], "start");
    }

    private void startListening(bool finishedTransition, int charNum)
    {
        if(finishedTransition)
        {
            currCharacter = charNum;
        }
    }
}
