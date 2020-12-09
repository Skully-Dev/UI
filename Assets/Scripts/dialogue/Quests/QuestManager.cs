using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public GameManager gameManager;

    public Player player;
    public Inventory inventory;

    //QuestGiver
    private Quest currentQuest;

    private bool showRewards;

    //screen
    public Vector2 scr;

    private void Start()
    {
        
    }

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
        if (currentQuest.goal.isCompleted() == true)
        {
            inventory.money += currentQuest.goldReward;
            //add exp
            player.LevelUp();


            currentQuest.goal.questState = QuestState.Claimed;
            Debug.Log("Quest Claimed");

            showRewards = true;
        }
    }

    private void OnGUI() //virtual allows us to override OnGUI in derived classes
    {
        if (currentQuest != null) //if player has a current quest
        {
            //set up our ratio for 16:9
            scr.x = Screen.width / 16;
            scr.y = Screen.height / 9;

            GUI.Box(new Rect(12.75f * scr.x, scr.y * 0.25f,
                             3 * scr.x, scr.y * 0.5f),
                             "Quest: " + currentQuest.title);
        }

        if (showRewards)
        {
            GUI.Box(new Rect(6 * scr.x, scr.y * 2f,
                 4 * scr.x, scr.y * 3f),
                 "Quest Rewards" +
                 "\nGold: " + currentQuest.goldReward + 
                 "\nLevel: " + "+1");

            if (GUI.Button( new Rect(7.5f * scr.x, scr.y * 4.25f, 1f * scr.x, 0.5f * scr.y), "Close"))
            {
                showRewards = false;
                currentQuest = null;
                gameManager.EnableControls();
            }


        }
    }
}
