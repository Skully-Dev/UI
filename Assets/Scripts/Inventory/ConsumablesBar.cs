using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumablesBar : MonoBehaviour
{
    [SerializeField]
    private Player player;
    [SerializeField]
    private Inventory playerInventory;

    private Vector2 scr;

    [Serializable]
    public struct QuickItem
    {
        public string buttonText; //1,2,3,4...
        public KeyCode keyCode; //Alpha1,Alpha2...
        /*[NonSerialized]*/ public Item item; //a Reference of the bound item
    };
    public QuickItem[] hotbar;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < hotbar.Length; i++)
        {
            hotbar[i].buttonText = (i + 1).ToString();
            hotbar[i].keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + hotbar[i].buttonText);
        }

    }

    private void Update()
    {
        for (int i = 0; i < hotbar.Length; i++)
        {
            if (hotbar[i].item.Timer > 0)
            {
                hotbar[i].item.Timer -= Time.deltaTime;
            }
        }
    }

    private void OnGUI()
    {
        //even square boxes
        scr.x = Screen.width / 16;
        scr.y = Screen.height / 9;

        for (int i = 0; i < hotbar.Length; i++)
        {
            //if button press or related hotkey keycode pressed
            if (GUI.Button(new Rect(5 * scr.x + i * scr.x, 8 * scr.y, scr.x, scr.y), hotbar[i].item.Icon) || Input.GetKeyDown(hotbar[i].keyCode))
            {
                Debug.Log(hotbar[i].buttonText);

                //if in inventory
                if (playerInventory.showInventory)
                {
                    //if selected item is consumable
                    if (playerInventory.selectedItem.Type == ItemType.Food || playerInventory.selectedItem.Type == ItemType.Potions)
                    {
                        //add selected item as item to use for hotkey
                        hotbar[i].item = playerInventory.selectedItem;
                    }
                }
                else if (!GameManager.isDisplay && hotbar[i].item.Amount > 0 && hotbar[i].item.Timer <= 0) //if no display windows open and have items in stock
                {
                    //determine how to use the item
                    switch (hotbar[i].item.Type)
                    {
                        case ItemType.Food:
                            hotbar[i].item.Amount--;

                            player.Heal(hotbar[i].item.Heal);

                            hotbar[i].item.Timer = hotbar[i].item.Cooldown;

                            if (hotbar[i].item.Amount <= 0)
                            {
                                playerInventory.inventory.Remove(hotbar[i].item);
                                break;
                            }
                            break;
                        case ItemType.Potions:
                            hotbar[i].item.Amount--;

                            player.RefillStat(hotbar[i].item.Heal, hotbar[i].item.Mana, hotbar[i].item.Stamina);

                            hotbar[i].item.Timer = hotbar[i].item.Cooldown;

                            if (hotbar[i].item.Amount <= 0)
                            {
                                playerInventory.inventory.Remove(hotbar[i].item);
                                break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            if (hotbar[i].item.Timer > 0)
            {
                GUI.Box(new Rect(5 * scr.x + i * scr.x, 8 * scr.y, scr.x, scr.y), hotbar[i].item.Timer.ToString("0"));
                //hotbar[i].item.Timer -= Time.deltaTime;
            }
            else if(hotbar[i].item.Amount <= 0)
            {
                GUI.Box(new Rect(5 * scr.x + i * scr.x, 8 * scr.y, scr.x, scr.y), "");
            }


            //hotkey Number in top left corner
            GUI.Box(new Rect(5 * scr.x + i * scr.x, 8 * scr.y, 0.2f * scr.x, 0.2f * scr.y), hotbar[i].buttonText);
        }
    }
}