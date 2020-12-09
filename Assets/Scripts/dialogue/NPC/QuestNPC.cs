using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNPC : NPC
{
    //reference to quest manager
    private QuestManager questManager;

    [SerializeField]
    private QuestGoal questGoal;

    [SerializeField]
    private QuestDialogue availableDialogue;
    [SerializeField]
    private Dialogue activeDialogue;
    [SerializeField]
    private ClaimDialogue claimDialogue;
    [SerializeField]
    private Dialogue claimedDialogue;

    private void Start()
    {
        questManager = FindObjectOfType<QuestManager>();

        if (questManager  == null)
        {
            Debug.LogError("There is no QuestManager in the scene");
        }

        if (availableDialogue == null)
        {
            Debug.LogError("Don't forget to include Available Quest Dialoge");
        }
    }

    //depending on state of quest do different things
    public override void Interact() //override replaces Interact base code //abstract requires override
    {
        if (questGoal.questState == QuestState.Active)
        {
            if (questGoal.isCompleted())
            {
                questGoal.questState = QuestState.Completed;
            }
        }

        switch (questGoal.questState)
        {
            case QuestState.Available:
                availableDialogue.showDialogue = true;
                availableDialogue.ShowDialogue();
                break;
            case QuestState.Active:
                activeDialogue.showDialogue = true;
                activeDialogue.ShowDialogue();
                break;
            case QuestState.Completed:
                claimDialogue.showDialogue = true;
                claimDialogue.ShowDialogue();
                break;
            case QuestState.Claimed:
                claimedDialogue.showDialogue = true;
                claimedDialogue.ShowDialogue();
                break;
            default:
                break;
        }
    }
}

/*
    if (questGoal.isCompleted())
    {
        Debug.Log("Quest Claimed");
        questManager.ClaimQuest();
    }
    else
    {
        Debug.Log("Quest Not Claimed");
    } 
 */
