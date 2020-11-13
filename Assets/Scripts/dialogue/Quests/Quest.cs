using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //make visable in inspector
public class Quest
{
    public string title;
    public string description;

    public int requiredLevel;

    #region Rewards Variables
    public int experienceReward;
    public int goldReward;
    #endregion

    public QuestGoal goal;
}
