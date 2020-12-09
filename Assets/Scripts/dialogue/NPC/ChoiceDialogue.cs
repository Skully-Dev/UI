using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to make dialogue options appropriate to QUESTS like accept and decline at end of quest offer.
/// Only need one in scene, to be referenced by QUEST-NPC.
/// Be sure to attach player cam and controls though!
/// </summary>
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

    protected override void OnGUI() // override OnGUI in derived classes
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
                EndDialogue();
            }
        }
    }
}