﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField]
    private int capacity = 10;

    [Tooltip("Items within the chest")]
    public List<Item> chestInventory = new List<Item>();
    [Tooltip("Currently selected item to display info for")]
    private Item selectedItem;

    private Inventory playerInventory;

    //Display Chest
    [SerializeField] private bool showChest = false;
    private Vector2 scr;

    private void Start()
    {
        // FIXME: Would make more sense to get the player inventory stright from the player, rather than search the scene for an inventroy.
        playerInventory = (Inventory)FindObjectOfType<Inventory>();
        if (playerInventory == null)
        {
            Debug.LogError("There is no player with an inventory in this scene");
        }
    }

    private void OnGUI()
    {
        scr.x = Screen.width / 16;
        scr.y = Screen.height / 9;

        if (showChest)
        {
            //for each item in chest inventory, add button to select item to Layout List
            for (int i = 0; i < chestInventory.Count; i++)
            {
                if (GUI.Button(new Rect(12.5f * scr.x, (0.25f * scr.y) + i * (0.25f * scr.y),
                                         3 * scr.x, .25f * scr.y), chestInventory[i].Name))
                {
                    selectedItem = chestInventory[i];
                }
            }

            //when item is selected
            if (selectedItem != null)
            {
                //DISPLAY SELECTED ITEM
                //backdrop
                GUI.Box(new Rect(8.5f * scr.x, 0.25f * scr.y,
                                    3.5f * scr.x, 7 * scr.y), "");
                //Icon
                GUI.Box(new Rect(8.75f * scr.x, 0.5f * scr.y,
                                    3 * scr.x, 3 * scr.y), selectedItem.Icon);
                //Name Title
                GUI.Box(new Rect(9.05f * scr.x, 3.5f * scr.y,
                                    2.5f * scr.x, .5f * scr.y), selectedItem.Name);
                //Description, Value, Quantity
                GUI.Box(new Rect(8.75f * scr.x, 4 * scr.y, 3 * scr.x, 3 * scr.y),
                                    selectedItem.Description + "\nValue: " +
                                    selectedItem.Value + "\nAmount: " + selectedItem.Amount);

                //Option to add item to own inventory
                if (GUI.Button(new Rect(10.5f * scr.x, 6.5f * scr.y, scr.x, 0.25f * scr.y), "Take Item"))
                {
                    //add to player
                    if (playerInventory.AddItemAttempt(selectedItem))
                    {
                        //If taking the item was successful, remove from chest
                        selectedItem.Amount--;
                        if (selectedItem.Amount <= 0)
                        {
                            chestInventory.Remove(selectedItem);
                            selectedItem = null;
                        }
                    }
                    
                }
            }

            //display players inventory
            playerInventory.showInventory = true;

            if (GUI.Button(new Rect(30, 1020, 120, 60), "Exit Chest"))
            {
                OpenChestToggle();
            }
        }
    }

    public void OpenChestToggle()
    {
        if (showChest)
        {
            playerInventory.showInventory = false;
            showChest = false;

            playerInventory.gameManager.EnableControls();
        }
        else
        {
            playerInventory.state = Inventory.State.Chest;
            playerInventory.chest = this;

            showChest = true;

            playerInventory.gameManager.DisableControls(false);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns>If adding the item was sucessful</returns>
    public bool AddItem(Item item)
    {
        Item foundItem = chestInventory.Find(findItem => findItem.Name == item.Name); //things on the left is paramater, lambda =>  right is expression, each itteration findItem will be the specific item that itteration and it will test it againt the item werre trying to find.

        if (foundItem != null)
        {
            foundItem.Amount++;
            return true;
        }
        else
        {
            if (chestInventory.Count < capacity)
            {
                Item newItem = new Item(item, 1);
                chestInventory.Add(newItem);
                return true;
            }
            else
            {
                Debug.Log("That won't fit. The chest is too full...");
                return false;
            }
        }
    }
}
