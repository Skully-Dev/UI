using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDialogue : Dialogue
{
    QuestManager questManager;

    public Quest quest;

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();

        if (questManager == null)
        {
            Debug.LogError("There is no quest manager in scene");
        }
    }

    protected override void EndDialogue()
    {
        if (GUI.Button(new Rect(15 * scr.x, 8.5f * scr.y,
                        scr.x, scr.y * 0.5f), "Accept"))
        {
            questManager.AcceptQuest(quest);

            showDialogue = false;
            currentLineIndex = 0;

            playerMovement.enabled = true;
            //also maybe mouse/screen aim control enable/disable

            //Code to add quest aka accept

            //enable and disable
            //Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (GUI.Button(new Rect(13 * scr.x, 8.5f * scr.y,
                scr.x, scr.y * 0.5f), "Don't Accept"))
        {
            showDialogue = false;
            currentLineIndex = 0;

            playerMovement.enabled = true;
            //also maybe mouse/screen aim control enable/disable

            //Code to add quest aka accept

            //enable and disable
            //Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
