using UnityEngine;

public class ChoiceDialogue : Dialogue
{
    [SerializeField]
    private ChoiceNPC choiceNPC;

    [SerializeField]
    private string responsePositive;
    [SerializeField]
    private string responseNeutral;
    [SerializeField]
    private string responseNegative;

    public override void ShowDialogue()
    {
        DialogueManager.isDialogue = true;

        gameManager.DisableControls(false);

        dialogueManager.dialogueWindow.SetActive(true);

        dialogueManager.buttons[0].gameObject.SetActive(true);
        dialogueManager.buttons[1].gameObject.SetActive(true);
        dialogueManager.buttons[2].gameObject.SetActive(true);

        dialogueManager.buttonsText[0].text = responsePositive;
        dialogueManager.buttons[0].onClick.RemoveAllListeners();
        dialogueManager.buttons[0].onClick.AddListener(PositiveEvent);

        dialogueManager.buttonsText[1].text = responseNeutral;
        dialogueManager.buttons[1].onClick.RemoveAllListeners();
        dialogueManager.buttons[1].onClick.AddListener(NeutralEvent);

        dialogueManager.buttonsText[2].text = responseNegative;
        dialogueManager.buttons[2].onClick.RemoveAllListeners();
        dialogueManager.buttons[2].onClick.AddListener(NegativeEvent);

        RefreshDialogue();
    }

    protected new void RefreshDialogue()
    {
        dialogueManager.dialogueText.text = npc.name + ": " + dialogueText[currentLineIndex];

        if (currentLineIndex != 0)
        {
            dialogueManager.buttons[1].gameObject.SetActive(false);
            dialogueManager.buttons[2].gameObject.SetActive(false);

            dialogueManager.buttonsText[0].text = "Bye";
            dialogueManager.buttons[0].onClick.RemoveAllListeners();
            dialogueManager.buttons[0].onClick.AddListener(EndDialogue);
        }
    }

    private void PositiveEvent()
    {
        choiceNPC.approvalState = ApprovalState.Like;

        currentLineIndex = (int)choiceNPC.approvalState + 1;

        RefreshDialogue();
    }

    private void NeutralEvent()
    {
        choiceNPC.approvalState = ApprovalState.Neutral;

        currentLineIndex = (int)choiceNPC.approvalState + 1;

        RefreshDialogue();
    }

    private void NegativeEvent()
    {
        choiceNPC.approvalState = ApprovalState.Dislike;

        currentLineIndex = (int)choiceNPC.approvalState + 1;

        RefreshDialogue();
    }

    protected override void OnGUI() // override OnGUI in derived classes
    {
        if (showOnGUI)
        {
            if (showDialogue)
            {
                gameManager.DisableControls(false);
                //gameManager.DisableControls(true);

                //set up our ratio for 16:9
                scr.x = Screen.width / 16;
                scr.y = Screen.height / 9;

                //The dialigue box takes up the whole bottom 3rd of the screen and displays the NPC's name and current dialogue line
                GUI.Box(new Rect(0, 6 * scr.y,
                                 Screen.width, scr.y * 3),
                                 npc.name + " : " + dialogueText[currentLineIndex]);

                //Gives the choice
                if (currentLineIndex == 0)
                {
                    if (GUI.Button(new Rect(15 * scr.x, 8.5f * scr.y,
                    scr.x, scr.y * 0.5f), responsePositive))
                    {
                        choiceNPC.approvalState = ApprovalState.Like;

                        currentLineIndex = (int)choiceNPC.approvalState + 1;
                    }

                    if (GUI.Button(new Rect(13 * scr.x, 8.5f * scr.y,
                                scr.x, scr.y * 0.5f), responseNeutral))
                    {
                        choiceNPC.approvalState = ApprovalState.Neutral;

                        currentLineIndex = (int)choiceNPC.approvalState + 1;
                    }

                    if (GUI.Button(new Rect(11 * scr.x, 8.5f * scr.y,
                                    scr.x, scr.y * 0.5f), responseNegative))
                    {
                        choiceNPC.approvalState = ApprovalState.Dislike;

                        currentLineIndex = (int)choiceNPC.approvalState + 1;
                    }
                }
                else
                {
                    EndDialogueOnGUI();
                }
            }
        }        
    }
}