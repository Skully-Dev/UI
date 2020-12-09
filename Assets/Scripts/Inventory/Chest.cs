using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    #region References AND Variables
    public bool showOnGUI;

    [Header("External Reference")]
    [Tooltip("reference to the players inventory")]
    private Inventory playerInventory;

    [Header("Chest Settings")]
    [SerializeField, Tooltip("How many items the chest can store.")]
    private int capacity = 10;
    [Tooltip("Items within the chest")]
    public List<Item> inventory = new List<Item>();
    [Tooltip("Currently selected item to display info for")]
    private Item selectedItem;

    #region Display Shop References and Variables
    // DISPLAY CHEST
    [Header("Display Settings")]
    [SerializeField, Tooltip("Display state of chest window")] private bool showChest = false;
    [Tooltip("Screen width and height divided by 16 and 9 (ratio 120:1)")] private Vector2 scr;

    // TODO: these references can be the same for Chest AND Shop. Therefore could have a Base Class OtherInventory that chest and shop inherit from, that way there only needs to be one place to reference.
    #region Canvas UI References
    [SerializeField, Tooltip("The buttons that will turn into items, TODO: may be used to find capacity later")]
    private Button[] inventoryButtons;

    [Header("Selected Item Display References")]
    //selected item
    [SerializeField, Tooltip("The display icon for selected Chest item")]
    private Image selectedIcon;
    [SerializeField, Tooltip("The display name for selected Chest item")]
    private Text selectedName;
    [SerializeField, Tooltip("The display discription for selected Chest item")]
    private Text selectedDiscription;
    [SerializeField, Tooltip("The selected item of chest primary button")]
    private Button primaryButton;
    [SerializeField, Tooltip("The selected item of chest secondary button, NOT USED but may use in future.")]
    private Button secondaryButton;

    [Header("Canvas Group References")]
    //Canvas groups
    [SerializeField, Tooltip("The Canvas UI for Chest")]
    private GameObject chestInventoryGroup;
    [SerializeField, Tooltip("The Canvas UI for SPECIFICALLY for Chest SELECTED ITEM")]
    private GameObject selectedItemGroup;
    #endregion

    #endregion

    #endregion

    private void Start()
    {
        //Initialize player inventory reference

        // TODO: maybe get the player inventory stright from the player, rather than search the scene for an inventroy.
        playerInventory = (Inventory)FindObjectOfType<Inventory>();
        if (playerInventory == null)
        {
            Debug.LogError("There is no player with an inventory in this scene");
        }
    }

    #region Select Item, Refresh Description, Update Buttons Methods
    /// <summary>
    /// OnClick of Canvas UI Chest Items button
    /// </summary>
    /// <param name="i"></param>
    public void SelectItem(int i)
    {
        if (i < inventory.Count) //item selected
        {
            selectedItemGroup.SetActive(true);//show selected item window

            //get selected item
            selectedItem = inventory[i];

            //set up selected item icon
            Sprite mySprite = Sprite.Create(inventory[i].Icon, selectedIcon.sprite.rect, selectedIcon.sprite.pivot);
            selectedIcon.sprite = mySprite;

            selectedName.text = inventory[i].Name;
            //RefreshSelectedItemDescription() could replace
            selectedDiscription.text = selectedItem.Description +
                                        "\nValue: $" + selectedItem.Value +
                                        "\nQuantity: " + selectedItem.Amount;

            UpdateButtons();
        }
        else //otherwise no item selected
        {
            selectedItem = null;
            selectedItemGroup.SetActive(false);//hide selected item window
        }

        playerInventory.gameManager.PlayButtonSound();
    }

    /// <summary>
    /// Updates just the description of selected item window, handy for changing amount
    /// </summary>
    public void RefreshSelectedItemDescription()
    {
        if (selectedItem != null)
        {
            selectedDiscription.text = selectedItem.Description +
                    "\nValue: $" + selectedItem.Value +
                    "\nQuantity: " + selectedItem.Amount;
        }
    }

    /// <summary>
    /// Determines what buttons are available and how they work.
    /// </summary>
    public void UpdateButtons()
    {
        secondaryButton.gameObject.SetActive(false);//no need for secondary in chest

        if (selectedItem != null)
        {
            primaryButton.onClick.RemoveAllListeners();
            primaryButton.gameObject.SetActive(true); //Basically true unless otherwise specified, saves me rewritting.

            primaryButton.onClick.AddListener(TakeItemEvent);
            primaryButton.gameObject.GetComponentInChildren<Text>().text = "Take";
            primaryButton.onClick.AddListener(RefreshInventory);

            //attempt to add to player inventory
            if (playerInventory.CanAddItem(selectedItem))
            {
                //If it is possible for player to take item
                primaryButton.interactable = true;
            }
            else
            {
                primaryButton.interactable = false;
            }
        }
        else
        {
            primaryButton.gameObject.SetActive(false); //Just to avoid taking null items.
        }

        primaryButton.onClick.AddListener(playerInventory.gameManager.PlayButtonSound);
    }
    #endregion

    #region Refresh Inventory Method
    /// <summary>
    /// Refresh the names of the buttons.
    /// </summary>
    public void RefreshInventory()
    {
        Text buttonText;
        int itemCount = inventory.Count;

        if (playerInventory.sortType == "")
        {
            //DISPLAY ALL ITEMS and empty item slots

            for (int i = 0; i < inventoryButtons.Length; i++)
            {
                //activate all inventory buttons
                inventoryButtons[i].gameObject.SetActive(true);

                //Update Inventory Button Text
                buttonText = inventoryButtons[i].GetComponentInChildren<Text>();

                if (i < itemCount)
                {
                    buttonText.text = inventory[i].Name; //item name on button
                }
                else
                {
                    buttonText.text = "-Empty Slot-";
                }
            }
        }
        else //Otherwise only display items of type
        {
            ItemType type = (ItemType)Enum.Parse(typeof(ItemType), playerInventory.sortType);

            for (int i = 0; i < inventoryButtons.Length; i++)
            {
                if (i < itemCount && inventory[i].Type == type)
                {
                    //activate relevant inventory buttons
                    inventoryButtons[i].gameObject.SetActive(true);

                    buttonText = inventoryButtons[i].GetComponentInChildren<Text>();
                    buttonText.text = inventory[i].Name;
                }
                else
                {
                    //hides unrelated buttons
                    inventoryButtons[i].gameObject.SetActive(false);
                    //due to layout, will automatically move all active buttons to the top of the list
                }
            }
        }

        if (selectedItem == null)
        {
            selectedItemGroup.SetActive(false); //hide selected item window
        }
    }
    #endregion

    #region Take Item Button Event Method
    /// <summary>
    /// Take item, set up for canvas UI
    /// </summary>
    private void TakeItemEvent()
    {
        //attempt to add to player inventory, should always succeed anyways as already checked
        if (playerInventory.AddItemAttempt(selectedItem))
        {
            //If taking the item was successful, remove from chest
            selectedItem.Amount--;
            if (selectedItem.Amount <= 0)
            {
                inventory.Remove(selectedItem);
                selectedItem = null;
                selectedItemGroup.SetActive(false);
            }
            else
            {
                RefreshSelectedItemDescription();
            }

            playerInventory.RefreshInventory(); //refresh the players items
            playerInventory.RefreshSelectedItemDescription(); //refresh the players selected item description
            playerInventory.UpdateButtons(); //if couldnt store item, then you take an item, must update the store item option. etc.
        }
    }
    #endregion

    #region Open Chest Toggle Method
    /// <summary>
    /// Switch between displaying chest and NOT displaying chest.
    /// </summary>
    public void OpenChestToggle()
    {
        if (showChest)
        {
            playerInventory.HideInventory();
            playerInventory.showInventory = false;

            playerInventory.state = Inventory.State.Other; //other is used when not in an inventory state

            chestInventoryGroup.SetActive(false);
            selectedItem = null;
            showChest = false;

            playerInventory.gameManager.EnableControls();
        }
        else
        {
            playerInventory.ShowInventory();
            playerInventory.showInventory = true;

            playerInventory.state = Inventory.State.Chest; //determines the buttons and what they do for selected item in player inventory.
            playerInventory.chest = this;

            showChest = true;
            chestInventoryGroup.SetActive(true);
            RefreshInventory();

            playerInventory.gameManager.DisableControls(false);
        }
    }
    #endregion

    #region CanAddItem, AttemptAddItem, Methods
    /// <summary>
    /// Check if it would be possible to add item
    /// </summary>
    /// <param name="item">The item to test</param>
    /// <returns>Returns true if it would be successful at adding to chest. Otherwise false.</returns>
    public bool CanAddItem(Item item)
    {
        Item foundItem = inventory.Find(findItem => findItem.Name == item.Name); //things on the left is paramater, lambda =>  right is expression, each itteration findItem will be the specific item that itteration and it will test it againt the item werre trying to find.

        //checks to see if it can stack with existing chest items, weapons and apparel DONT STACK
        if ((item.Type != ItemType.Apparel && item.Type != ItemType.Weapon) && foundItem != null)
        {
            return true;
        }
        else //If unstackable
        {
            //Check if room to add
            if (inventory.Count < capacity)
            {
                //enough room, adds item
                return true;
            }
            else
            {
                //no room, item add fails.
                return false;
            }
        }
    }

    /// <summary>
    /// Adding an item to chest inventory. Depends on chest capacity.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Returns true if successful at adding to Chest. Otherwise false.</returns>
    public bool AddItemAttempt(Item item)
    {
        Item foundItem = inventory.Find(findItem => findItem.Name == item.Name); //things on the left is paramater, lambda =>  right is expression, each itteration findItem will be the specific item that itteration and it will test it againt the item werre trying to find.

        //checks to see if it can stack with existing inventory items, weapons and apparel DONT STACK
        if ((item.Type != ItemType.Apparel && item.Type != ItemType.Weapon) && foundItem != null)
        {
            foundItem.Amount++;
            return true;
        }
        else //If unstackable
        {
            //Check if room to add
            if (inventory.Count < capacity)
            {
                //enough room, adds item
                Item newItem = new Item(item, 1);
                inventory.Add(newItem);
                return true;
            }
            else
            {
                //no room, item add fails.
                Debug.Log("That won't fit. The chest is too full...");
                return false;
            }
        }
    }
    #endregion

    #region OnGUI IMGUI Redundant
    private void OnGUI()
    {
        if (showOnGUI)
        {
            scr.x = Screen.width / 16;
            scr.y = Screen.height / 9;

            if (showChest)
            {
                //for each item in chest inventory, add button to select item to Layout List
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (GUI.Button(new Rect(12.5f * scr.x, (0.25f * scr.y) + i * (0.25f * scr.y),
                                             3 * scr.x, .25f * scr.y), inventory[i].Name))
                    {
                        selectedItem = inventory[i];
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
                        //attempt to add to player inventory
                        if (playerInventory.AddItemAttempt(selectedItem))
                        {
                            //If taking the item was successful, remove from chest
                            selectedItem.Amount--;
                            if (selectedItem.Amount <= 0)
                            {
                                inventory.Remove(selectedItem);
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
    }
    #endregion
}
