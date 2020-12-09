using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopDialogue : Dialogue
{
    protected override void EndDialogue()
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
