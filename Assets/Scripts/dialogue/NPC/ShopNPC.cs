using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNPC : NPC
{
    [System.NonSerialized]
    public Shop shop;

    [SerializeField]
    public ApprovalState approvalState = ApprovalState.Dislike;

    [SerializeField]
    private ShopDialogue friendDialogue;
    [SerializeField]
    private ShopDialogue regularDialogue;
    [SerializeField]
    private ShopDialogue strangerDialogue;

    private void Start()
    {
        shop = gameObject.GetComponent<Shop>();
    }

    //depending on state do different things
    public override void Interact() //override replaces Interact base code //abstract requires override
    {
        //Same values as those that increase the players discounts.
        if (shop.Profit >= 500)
        {
            approvalState = ApprovalState.Like;
        }
        else if (shop.Profit >= 25)
        {
            approvalState = ApprovalState.Neutral;
        }
        else
        {
            approvalState = ApprovalState.Dislike;
        }

        switch (approvalState)
        {
            case ApprovalState.Dislike:
                strangerDialogue.showDialogue = true;
                break;
            case ApprovalState.Neutral:
                regularDialogue.showDialogue = true;
                break;
            case ApprovalState.Like:
                friendDialogue.showDialogue = true;
                break;
            default:
                break;
        }
    }
}
