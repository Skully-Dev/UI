using UnityEngine;

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

    public override void ShowDialogue()
    {
        DialogueManager.isDialogue = true;

        gameManager.DisableControls(false);

        dialogueManager.dialogueWindow.SetActive(true);

        dialogueManager.buttons[0].gameObject.SetActive(true);
        dialogueManager.buttons[1].gameObject.SetActive(true);
        dialogueManager.buttons[2].gameObject.SetActive(false);

        dialogueManager.buttonsText[0].text = "Accept";
        dialogueManager.buttons[0].onClick.RemoveAllListeners();
        dialogueManager.buttons[0].onClick.AddListener(AcceptEvent);
        if (player.Level < quest.requiredLevel)
        {
            dialogueManager.buttons[0].interactable = false;
        }

        dialogueManager.buttonsText[1].text = "Don't Accept";
        dialogueManager.buttons[1].onClick.RemoveAllListeners();
        dialogueManager.buttons[1].onClick.AddListener(EndDialogue);

        //RefreshDialogue();
        dialogueManager.dialogueText.text = npc.name + ": " + dialogueText[currentLineIndex];
    }

    private void AcceptEvent()
    {
        dialogueManager.buttons[0].interactable = true;

        DialogueManager.isDialogue = false;       

        questManager.AcceptQuest(quest);

        showDialogue = false;
        currentLineIndex = 0;

        dialogueManager.dialogueWindow.SetActive(false);

        gameManager.EnableControls();
    }

    protected override void EndDialogue()
    {
        dialogueManager.buttons[0].interactable = true;
        base.EndDialogue();
    }

    protected override void EndDialogueOnGUI()
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
