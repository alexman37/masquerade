using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TalkingManager : MonoBehaviour
{
    public TextAsset[] characterDialogueFiles;
    private int currCharacter;
    private HashSet<int> charactersIntroduced = new HashSet<int>();

    [SerializeField] private GameObject dialogueButtons;
    [SerializeField] private TextMeshProUGUI tm;

    private void OnEnable()
    {
        CameraManager.cameraTransition += startListening;
        Notebook.notebookOpened += disableDialogueButtons;
        Notebook.notebookClosed += enableDialogueButtons;
        LockboxMaster.stoppedLockbox += enableDialogueButtons;
    }

    private void OnDisable()
    {
        CameraManager.cameraTransition -= startListening;
        Notebook.notebookOpened -= disableDialogueButtons;
        Notebook.notebookClosed -= enableDialogueButtons;
        LockboxMaster.stoppedLockbox -= enableDialogueButtons;
    }

    public void beginDialogueForCurrent()
    {
        if(!CameraManager.inTransition)
        {
            // lockbox
            if (currCharacter == 5)
            {
                disableDialogueButtons();
                LockboxMaster.instance.useLockbox();
            }
            else
            {
                string entryBlock = "start";
                if (charactersIntroduced.Contains(currCharacter)) entryBlock = "second";
                charactersIntroduced.Add(currCharacter);

                DialogueManager.instance.processConversation(characterDialogueFiles[currCharacter], entryBlock);
                disableDialogueButtons();
            }
        }
    }

    private void startListening(bool finishedTransition, int charNum)
    {
        if(finishedTransition)
        {
            currCharacter = charNum;
            if(currCharacter == 5)
            {
                tm.text = "Use";
            } else
            {
                tm.text = "Question";
            }
        }
    }




    private void enableDialogueButtons()
    {
        dialogueButtons.SetActive(true);
    }

    private void disableDialogueButtons()
    {
        dialogueButtons.SetActive(false);
    }
}
