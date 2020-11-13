using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enums outside class so accessable by other scripts in same namespace (DEFAULT NAMESPACE CURRENTLY)
public enum QuestState
{
    Available,
    Active,
    Completed,
    Claimed
}

public enum GoalType
{
    Gahter,
    //Kill,
    //Escort,
    //Locate
}

[System.Serializable] //make visable in inspector
public abstract class QuestGoal : MonoBehaviour
{
    public QuestState questState;

    public GoalType goalType;

    //depends on type of quest
    public abstract bool isCompleted();

    /*public void ItemCollected(string name)
    {
        if (goalType == GoalType.Gahter && itemName == name)
        {
            currentAmount++;
            if (currentAmount >= requiredAmount)
            {
                questState = QuestState.Completed;
                Debug.Log("QUEST COMPLETE");
            }
        }
    }*/
}
