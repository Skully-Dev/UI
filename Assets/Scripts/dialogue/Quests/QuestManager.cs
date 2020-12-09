using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public bool showOnGUI = false;

    public GameManager gameManager;

    public Player player;
    public Inventory inventory;

    //QuestGiver
    private Quest currentQuest;

    private bool showRewards;

    [Header("Rewards References")]
    [SerializeField]
    private GameObject questRewardsWindow;
    [SerializeField]
    private TextMeshProUGUI rewardsText;

    [Header("Current Quest References")]
    [SerializeField]
    private GameObject currentQuestWindow;
    [SerializeField]
    private TextMeshProUGUI currentQuestText;

    //screen
    private Vector2 scr;

    private void Start()
    {
        
    }

    public void AcceptQuest(Quest acceptedQuest)
    {
        if (acceptedQuest != null)
        {
            currentQuest = acceptedQuest;
            currentQuest.goal.questState = QuestState.Active;

            currentQuestWindow.SetActive(true);
            currentQuestText.text = "Quest: " + currentQuest.title;
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
            for (int i = 0; i < currentQuest.levelGained; i++)
            {
                player.LevelUp();
            }
            
            currentQuest.goal.questState = QuestState.Claimed;
            Debug.Log("Quest Claimed");

            showRewards = true;
            ShowRewards();

            currentQuestWindow.SetActive(false);
        }
    }

    private void ShowRewards()
    {
        questRewardsWindow.SetActive(true);
        rewardsText.text = "QUEST REWARDS" +
                           "\nGold: " + currentQuest.goldReward;
        if (currentQuest.levelGained > 0)
        {
            rewardsText.text += "\nLevel: +" + currentQuest.levelGained;
        }
    }

    public void CloseWindow()
    {
        DialogueManager.isDialogue = false;

        showRewards = false;
        currentQuest = null;

        questRewardsWindow.SetActive(false);

        gameManager.EnableControls();
    }

    private void OnGUI() //virtual allows us to override OnGUI in derived classes
    {
        if (showOnGUI)
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

                if (GUI.Button(new Rect(7.5f * scr.x, scr.y * 4.25f, 1f * scr.x, 0.5f * scr.y), "Close"))
                {
                    showRewards = false;
                    currentQuest = null;
                    gameManager.EnableControls();
                }
            }
        }
    }
}
