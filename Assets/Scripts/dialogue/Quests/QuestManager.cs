using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public Player player;
    public Inventory inventory;

    Dialogue dialogue;
    
    //QuestGiver
    private Quest currentQuest;

    public void AcceptQuest(Quest acceptedQuest)
    {
        if (acceptedQuest != null)
        {
            currentQuest = acceptedQuest;
            currentQuest.goal.questState = QuestState.Active;
        }

    }

    public void DeclineQuest()
    {

    }

    public void ClaimQuest()
    {
        if (currentQuest.goal.questState == QuestState.Active
            && currentQuest.goal.isCompleted() == true)
        {
            inventory.money += currentQuest.goldReward;
            //add exp

            currentQuest.goal.questState = QuestState.Claimed;
            Debug.Log("Quest Claimed");
        }
    }
}
