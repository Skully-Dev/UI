using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ApprovalState
{
    Like,
    Neutral,
    Dislike,
}

public class ChoiceNPC : NPC
{
    [SerializeField]
    public ApprovalState approvalState = ApprovalState.Neutral;

    [SerializeField]
    private ChoiceDialogue likeDialogue;
    [SerializeField]
    private ChoiceDialogue neutralDialogue;
    [SerializeField]
    private ChoiceDialogue dislikeDialogue;

    //depending on state do different things
    public override void Interact() //override replaces Interact base code //abstract requires override
    {
        switch (approvalState)
        {
            case ApprovalState.Dislike:
                dislikeDialogue.showDialogue = true;
                dislikeDialogue.ShowDialogue();
                break;
            case ApprovalState.Neutral:
                neutralDialogue.showDialogue = true;
                neutralDialogue.ShowDialogue();
                break;
            case ApprovalState.Like:
                likeDialogue.showDialogue = true;
                likeDialogue.ShowDialogue();
                break;
            default:
                break;
        }     
    }
}
