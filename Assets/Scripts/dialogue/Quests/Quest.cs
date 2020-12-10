using UnityEngine;

//[System.Serializable] //make visable in inspector when NOT derived from MonoBehaviour
public class Quest : MonoBehaviour
{
    public string title;
    public string description;

    public int requiredLevel;

    #region Rewards Variables
    //public int experienceReward;
    public int levelGained;
    public int goldReward;
    #endregion

    public QuestGoal goal;
}
