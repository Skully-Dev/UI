using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour
{
    #region Inventory Variables
    [SerializeField] private List<Item> inventory = new List<Item>();
    private Item selectedItem;
    [SerializeField] private Player player;

    public int money = 100;
    #endregion

    #region Display Inv Variables
    [SerializeField] public bool showInventory = false;
    private Vector2 scr;
    private Vector2 scrollPosition;
    private string sortType = "";
    #endregion

    #region Equipment
    [Serializable]
    public struct Equipment
    {
        public string slotName; //chest legs etc
        public Transform equipLocation; //where on the character model will it be, visuals
        [NonSerialized] public GameObject currentItem; //the actual item
        [NonSerialized] public Item item; //nonSerialized means public but NOT in the inspector.
    };
    public Equipment[] equipmentSlots;
    #endregion

    public GUIStyle[] Styles;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) //I for Inventory
        {
            showInventory = !showInventory;
        }
    }

    public Item FindItem(string itemName)
    {
        //for each item in inventory, check if inventory item name == item name
            //if so return that as found item
        //goes through each item in inventory and compairs to item passed in
        Item foundItem = inventory.Find(findItem => findItem.Name == itemName); //things on the left is paramater, lambda =>  right is expression, each itteration findItem will be the specific item that itteration and it will test it againt the item werre trying to find.

        return foundItem;
    }

    public void AddItem(Item item)
    {
        if (item.Type.ToString() == "Money")
        {
            money += item.Value; //money converts from item to player money.
        }
        else
        {
            Item foundItem = inventory.Find(findItem => findItem.Name == item.Name); //things on the left is paramater, lambda =>  right is expression, each itteration findItem will be the specific item that itteration and it will test it againt the item werre trying to find.

            if (foundItem != null)
            {
                foundItem.Amount++;
            }
            else
            {
                Item newItem = new Item(item, 1);
                inventory.Add(newItem);
            }
        }
    }

    private void Display()
    {
        if (sortType == "") //display all items
        {
            scrollPosition = GUI.BeginScrollView(new Rect(0, 0.25f * scr.y, 3.75f * scr.x, 8.5f * scr.y), scrollPosition, new Rect(0, 0, 0, inventory.Count * .25f * scr.y), false, true);
            for (int i = 0; i < inventory.Count; i++)
            {
                if (GUI.Button(new Rect(0.5f * scr.x, 0.25f * scr.y + i * (0.25f * scr.y), 3 * scr.x, 0.25f * scr.y), inventory[i].Name))
                {
                    selectedItem = inventory[i];
                }
            }
            GUI.EndScrollView();
        }
        else
        {
            ItemType type = (ItemType) Enum.Parse(typeof(ItemType), sortType);
            int slotCount = 0;

            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].Type == type)
                {
                    if (GUI.Button(new Rect(0.5f * scr.x,
                        0.25f * scr.y + slotCount * (0.25f * scr.y),
                        3f * scr.x,
                        0.25f * scr.y),
                        inventory[i].Name))
                    {
                        selectedItem = inventory[i];
                    }
                    slotCount++;
                }
            }
        }
    }

    private void UseItem()
    {
        //Icon
        GUI.Box(new Rect(4.25f * scr.x, 0.5f * scr.y,
                         3 * scr.x, 3 * scr.y), selectedItem.Icon);
        GUI.Box(new Rect(4.55f * scr.x, 3.5f * scr.y,
                          2.5f * scr.x, 0.5f * scr.y), selectedItem.Name);
        GUI.Box(new Rect(4.25f * scr.x, 4 * scr.y,
                         3 * scr.x,
                         3 * scr.y),
                         selectedItem.Description +
                         "\nValue: " + selectedItem.Value +
                         "\nAmount: " + selectedItem.Amount);
        //Description, value, amount
        //Style

        switch (selectedItem.Type) //using switch TAB to auto fill, when you type (selectedItem.Type) and press enter it will auto fill the cases.
        {
            case ItemType.Food:
                if (player.playerStats.CurrentHealth < player.playerStats.stats.maxHealth)
                {
                    if (GUI.Button(new Rect(4.5f * scr.x, 6.5f * scr.y,
                        scr.x, 0.25f * scr.y), "Eat"))
                    {
                        selectedItem.Amount--;

                        player.Heal(selectedItem.Heal);

                        if (selectedItem.Amount <= 0)
                        {
                            inventory.Remove(selectedItem);
                            selectedItem = null;
                            break;
                        }
                    }
                }
                break;
            case ItemType.Weapon:
                if (equipmentSlots[2].currentItem == null || selectedItem.Name != equipmentSlots[2].item.Name)
                {
                    if (GUI.Button(new Rect(4.75f * scr.x, 6.5f * scr.y, scr.x, 0.25f * scr.y), "Equip"))
                    {
                        if (equipmentSlots[2].currentItem != null)
                        {
                            Destroy(equipmentSlots[2].currentItem);
                        }
                        GameObject currentItem = Instantiate(selectedItem.Mesh, equipmentSlots[2].equipLocation);
                        equipmentSlots[2].currentItem = currentItem;
                        equipmentSlots[2].item = selectedItem;
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(4.75f * scr.x, 6.5f * scr.y, 
                        scr.x, 0.25f * scr.y), "Unequip"))
                    {
                        Destroy(equipmentSlots[2].currentItem);
                        equipmentSlots[2].item = null;
                    }
                }
                break;
            case ItemType.Apparel:
                break;
            case ItemType.Crafting:
                break;
            case ItemType.Ingredients:
                break;
            case ItemType.Potions:
                break;
            case ItemType.Scrolls:
                break;
            case ItemType.Quest:
                break;
            case ItemType.Money:
                //Is auto converted and added into money float variables
                break;
            default:
                break;
        }
    }

private void OnGUI()
    {
        //even square boxes
        scr.x = Screen.width / 16;
        scr.y = Screen.height / 9; 

        if (showInventory)
        {
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
            string[] itemTypes = Enum.GetNames(typeof(ItemType));
            int CountOfItemTypes = itemTypes.Length;

            for (int i = 0; i < CountOfItemTypes; i++)
            {
                if (GUI.Button(new Rect(4 * scr.x + i * scr.x, 0, scr.x, 0.25f * scr.y), itemTypes[i]))
                {
                    sortType = itemTypes[i];
                }
            }
            Display();
            if (selectedItem != null)
            {
                UseItem();
            }
        }
    }
}
