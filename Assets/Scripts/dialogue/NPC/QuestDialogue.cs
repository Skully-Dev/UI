using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to make dialogue options appropriate to QUESTS like accept and decline at end of quest offer.
/// Only need one in scene, to be referenced by QUEST-NPC.
/// Be sure to attach player cam and controls though!
/// </summary>
public class QuestDialogue : Dialogue
{
    [System.NonSerialized]
    public QuestManager questManager;

    public Quest quest;

    public Player player;

    protected override void Start()
    {
        base.Start();

        questManager = FindObjectOfType<QuestManager>();

        if (questManager == null)
        {
            Debug.LogError("There is no quest manager in scene");
        }
    }

    protected override void EndDialogue()
    {
        if (player.Level >= quest.requiredLevel) //Can't accept quest if not of level yet.
        {
            if (GUI.Button(new Rect(15 * scr.x, 8.5f * scr.y,
                scr.x, scr.y * 0.5f), "Accept"))
            {
                questManager.AcceptQuest(quest);

                showDialogue = false;
                currentLineIndex = 0;

                gameManager.EnableControls();
            }
        }

        if (GUI.Button(new Rect(13 * scr.x, 8.5f * scr.y,
                scr.x, scr.y * 0.5f), "Don't Accept"))
        {
            showDialogue = false;
            currentLineIndex = 0;

            gameManager.EnableControls();
        }
    }
}
