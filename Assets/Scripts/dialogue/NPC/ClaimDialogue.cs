using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaimDialogue : Dialogue
{
    [SerializeField]
    private QuestDialogue questDialogue;

    protected override void RefreshDialogue()
    {
        dialogueManager.dialogueText.text = npc.name + ": " + dialogueText[currentLineIndex];

        if (currentLineIndex >= dialogueText.Length - 1)
        {
            dialogueManager.buttonsText[0].text = "Claim";
            dialogueManager.buttons[0].onClick.RemoveAllListeners();
            dialogueManager.buttons[0].onClick.AddListener(EndDialogue);
        }
    }

    protected override void EndDialogue()
    {
        //DialogueManager.isDialogue = false;

        showDialogue = false;
        currentLineIndex = 0;

        dialogueManager.dialogueWindow.SetActive(false);

        questDialogue.questManager.ClaimQuest();

        //gameManager.EnableControls();
    }

    protected override void EndDialogueOnGUI()
    {
        if (GUI.Button(new Rect(15 * scr.x, 8.5f * scr.y,
                        scr.x, scr.y * 0.5f), "Claim"))
        {
            if (questDialogue.quest.goal.isCompleted())
            {
                Debug.Log("Quest Claimed");
                questDialogue.questManager.ClaimQuest();
                //TODO: Show Quest Rewards.
            }
            else
            {
                Debug.Log("Quest Not Claimed");
            }


            showDialogue = false;
            currentLineIndex = 0;

            //gameManager.EnableControls();
        }
    }
}
