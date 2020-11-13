using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNPC : NPC
{
    //reference to quest manager
    protected QuestManager questManager;

    [SerializeField]
    protected Quest NPCsQuest;

    [SerializeField]
    protected QuestDialogue dialogue;

    [SerializeField] protected string[] dialogueText;

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();

        if (questManager  == null)
        {
            Debug.LogError("There is no QuestManager in the scene");
        }

        if (dialogue == null)
        {
            Debug.LogError("Don't forget to include Quest Dialoge");
        }
    }

    //depending on state of quest do different things
    public override void Interact() //override replaces Interact base code //abstract requires override
    {
        switch (NPCsQuest.goal.questState)
        {
            case QuestState.Available:
                // TODO: have accept quest at END of dialogue
                dialogue.npcName = name;
                dialogue.dialogueText = dialogueText;
                dialogue.quest = NPCsQuest;
                dialogue.showDialogue = true;
                break;
            case QuestState.Active:
                if (NPCsQuest.goal.isCompleted())
                {
                    Debug.Log("Quest Claimed");
                    questManager.ClaimQuest();
                }
                else
                {
                    Debug.Log("Quest Not Claimed");
                }
                break;
            case QuestState.Completed:
                break;
            case QuestState.Claimed:
                //Some dialogue
                //you have already done enough for me
                break;
            default:
                break;
        }
    }
}
