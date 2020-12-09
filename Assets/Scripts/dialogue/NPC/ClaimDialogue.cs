using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaimDialogue : Dialogue
{
    [SerializeField]
    private QuestDialogue questDialogue;

    protected override void EndDialogue()
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
