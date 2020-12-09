using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopDialogue : Dialogue
{
    protected override void RefreshDialogue()
    {
        base.RefreshDialogue();
        dialogueManager.buttonsText[0].text = "Shop";
        dialogueManager.buttons[0].onClick.AddListener(ShopEvent);
    }
    private void ShopEvent()
    {
        showDialogue = false;

        ShopNPC shopNPC = npc as ShopNPC;
        shopNPC.shop.OpenShopToggle();
    }

    protected override void EndDialogueOnGUI()
    {
        if (GUI.Button(new Rect(15 * scr.x, 8.5f * scr.y,
            scr.x, scr.y * 0.5f), "Shop"))
        {
            showDialogue = false;

            ShopNPC shopNPC = npc as ShopNPC;
            shopNPC.shop.OpenShopToggle();
        }
    }
}
