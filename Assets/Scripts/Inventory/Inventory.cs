using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour
{
    public GameManager gameManager;

    #region Inventory Variables
    public List<Item> inventory = new List<Item>(); //was [SerializeField] private, made public for ConsumablesBar
    public Item selectedItem; //was private, made public for ConsumablesBar
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
            if (!GameManager.isDisplay)
            {
                showInventory = true;
                state = State.Inventory;

                gameManager.DisableControls(false);
            }
            else if (showInventory && state == State.Inventory)
            {
                showInventory = false;
                selectedItem = null;

                gameManager.EnableControls();
            }
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

            if (foundItem != null || item.Type == ItemType.Apparel || item.Type == ItemType.Weapon)
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

    private void DisplayItems()
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
                        0.25f * scr.y + (slotCount+1) * (0.25f * scr.y),
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

    public enum State { Inventory, Chest, Shop }
    public State state = State.Inventory;

    private void UseItem()
    {
        //Description, value, amount
        GUI.Box(new Rect(4.25f * scr.x, 4 * scr.y,
                         3 * scr.x,
                         3 * scr.y),
                         selectedItem.Description +
                         "\nValue: " + selectedItem.Value +
                         "\nAmount: " + selectedItem.Amount);

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
                #region Equip/Unequip code
                if (equipmentSlots[2].currentItem == null) //if holding no weapons
                {
                    if (GUI.Button(new Rect(4.75f * scr.x, 6.5f * scr.y, scr.x, 0.25f * scr.y), "Equip"))
                    {
                        GameObject currentItem = Instantiate(selectedItem.Mesh, equipmentSlots[2].equipLocation); //the spawn of the item
                        equipmentSlots[2].currentItem = currentItem; //reference to instance of the spawned object
                        equipmentSlots[2].item = selectedItem; //copy the info of the new equipted weapon
                    }
                }
                else if (equipmentSlots[3].currentItem == null && selectedItem.Name != equipmentSlots[2].item.Name) //if holding 1 weapon and selected item is different
                {
                    if (GUI.Button(new Rect(4.75f * scr.x, 6.5f * scr.y, scr.x, 0.25f * scr.y), "Equip"))
                    {
                        GameObject currentItem = Instantiate(selectedItem.Mesh, equipmentSlots[3].equipLocation); //the spawn of the item
                        equipmentSlots[3].currentItem = currentItem; //reference to instance of the spawned object
                        equipmentSlots[3].item = selectedItem; //copy the info of the new equipted weapon
                    }
                }
                else if (selectedItem.Name != equipmentSlots[2].item.Name && selectedItem.Name != equipmentSlots[3].item.Name) //if holding 2 weapons but both are different to selected one.
                {
                    if (GUI.Button(new Rect(4.75f * scr.x, 6.5f * scr.y, scr.x, 0.25f * scr.y), "Equip"))
                    {
                        if (equipmentSlots[3].currentItem != null)
                        {
                            Destroy(equipmentSlots[3].currentItem); //destroies the spawn of the weapon to unequip
                        }
                        GameObject currentItem = Instantiate(selectedItem.Mesh, equipmentSlots[3].equipLocation); //spawn the new one
                        equipmentSlots[3].currentItem = currentItem; //replace reference to instance of the spawned object
                        equipmentSlots[3].item = selectedItem; //copy the info of the new equipted weapon
                    }
                }
                else //otherwise you already have one of this weapon equipted
                {
                    if (GUI.Button(new Rect(4.75f * scr.x, 6.5f * scr.y, scr.x, 0.25f * scr.y), "Unequip"))
                    {
                        if (selectedItem.Name == equipmentSlots[2].item.Name) //if it is the primary weapon you are trying to unequip
                        {
                            Destroy(equipmentSlots[2].currentItem); //remove the spawn of primary
                            GameObject currentItem = Instantiate(equipmentSlots[3].item.Mesh, equipmentSlots[2].equipLocation); //spawn the secondary into primary position
                            equipmentSlots[2].currentItem = currentItem; //replace reference to instance
                            equipmentSlots[2].item = equipmentSlots[3].item; //copy info into primary
                        }
                        Destroy(equipmentSlots[3].currentItem); //delete the spawn of the secondary in both cases.
                        equipmentSlots[3].item = null; //secondary no longer has info so null
                    }
                }
                /* When it was only 1 weapon OLD WIP
                if (equipmentSlots[2].currentItem == null || selectedItem.Name != equipmentSlots[2].item.Name )
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
                */
                #endregion
                break;
            case ItemType.Apparel:
                if (equipmentSlots[0].currentItem == null || selectedItem.Name != equipmentSlots[0].item.Name)
                {
                    if (GUI.Button(new Rect(4.75f * scr.x, 6.5f * scr.y, scr.x, 0.25f * scr.y), "Equip"))
                    {
                        if (equipmentSlots[0].currentItem != null)
                        {
                            Destroy(equipmentSlots[0].currentItem);
                        }
                        GameObject currentItem = Instantiate(selectedItem.Mesh, equipmentSlots[0].equipLocation);
                        equipmentSlots[0].currentItem = currentItem;
                        equipmentSlots[0].item = selectedItem;
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(4.75f * scr.x, 6.5f * scr.y,
                        scr.x, 0.25f * scr.y), "Unequip"))
                    {
                        Destroy(equipmentSlots[0].currentItem);
                        equipmentSlots[0].item = null;
                    }
                }
                break;
            case ItemType.Crafting:
                break;
            case ItemType.Ingredients:
                break;
            case ItemType.Potions:
                if (GUI.Button(new Rect(4.5f * scr.x, 6.5f * scr.y,
                    scr.x, 0.25f * scr.y), "Drink"))
                {
                    selectedItem.Amount--;

                    player.RefillStat(selectedItem.Heal, selectedItem.Mana, selectedItem.Stamina);

                    if (selectedItem.Amount <= 0)
                    {
                        inventory.Remove(selectedItem);
                        selectedItem = null;
                        break;
                    }
                }
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

    public Chest chest;
    private void StoreItem()
    {
        //Description, value, amount
        GUI.Box(new Rect(4.25f * scr.x, 4 * scr.y,
                         3 * scr.x,
                         3 * scr.y),
                         selectedItem.Description +
                         "\nValue: " + selectedItem.Value +
                         "\nAmount: " + selectedItem.Amount);

        if (GUI.Button(new Rect(4.5f * scr.x, 6.5f * scr.y,
                scr.x, 0.25f * scr.y), "Store"))
        {
            selectedItem.Amount--;
            chest.AddItem(selectedItem);

            if (selectedItem.Amount <= 0)
            {
                inventory.Remove(selectedItem);
                selectedItem = null;
            }
        }
    }

    public Shop shop;

    private void SellItem()
    {
        //Description, value, amount
        GUI.Box(new Rect(4.25f * scr.x, 4 * scr.y,
                         3 * scr.x,
                         3 * scr.y), 
                         selectedItem.Description +
                         "\nAmount: " + selectedItem.Amount +
                         "\nTrade value: " + (int)(selectedItem.Value * (1f - shop.profitMarginHalved)));

        // SELL, can't sell quest items.
        if (selectedItem.Type != ItemType.Quest)
        {
            if (GUI.Button(new Rect(4.5f * scr.x, 6.5f * scr.y,
                scr.x, 0.25f * scr.y), "Sell"))
            {
                selectedItem.Amount--;
                shop.AddItem(selectedItem);

                money += (int)(selectedItem.Value * (1f - shop.profitMarginHalved));
                shop.Profit += selectedItem.Value - (int)(selectedItem.Value * (1f - shop.profitMarginHalved));

                if (selectedItem.Amount <= 0)
                {
                    inventory.Remove(selectedItem);
                    selectedItem = null;
                }
            }
        }
    }

    private void OnGUI()
    {
        //even square boxes
        scr.x = Screen.width / 16;
        scr.y = Screen.height / 9; 

        if (showInventory)
        {
            //full screen backdrop
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

            GUI.Box(new Rect(250, 20, 170, 40), "Money: " + money);

            string[] itemTypes = Enum.GetNames(typeof(ItemType));
            int CountOfItemTypes = itemTypes.Length;

            //Sorting options buttons
            for (int i = 0; i < CountOfItemTypes; i++)
            {
                if (itemTypes[i] != "Money") //because money is stored in money var once collected
                {
                    if (GUI.Button(new Rect(4 * scr.x + i * scr.x, 0, scr.x, 0.25f * scr.y), itemTypes[i]))
                    {
                        sortType = itemTypes[i];
                    }
                }
            }
            if (GUI.Button(new Rect(4 * scr.x + 8 * scr.x, 0, scr.x, 0.25f * scr.y), "ALL")) //dont sort option
            {
                sortType = "";
            }

            DisplayItems();
            if (selectedItem != null)
            {
                //selected item backdrop
                GUI.Box(new Rect(4f * scr.x, 0.25f * scr.y,
                                    3.5f * scr.x, 7 * scr.y), "");
                //selected item Icon
                GUI.Box(new Rect(4.25f * scr.x, 0.5f * scr.y,
                                 3 * scr.x, 3 * scr.y), selectedItem.Icon);
                //Selected item Name
                GUI.Box(new Rect(4.55f * scr.x, 3.5f * scr.y,
                                  2.5f * scr.x, 0.5f * scr.y), selectedItem.Name);

                if (state == State.Inventory)
                {
                    UseItem();
                }
                if (state == State.Chest)
                {
                    StoreItem();
                }
                if (state == State.Shop)
                {
                    SellItem();
                }

            }
            if (state == State.Inventory)
            {
                if (GUI.Button(new Rect(30, 1020, 120, 60), "Exit Inventory"))
                {
                    showInventory = false;

                    gameManager.EnableControls();
                }
            }
        }
    }
}
