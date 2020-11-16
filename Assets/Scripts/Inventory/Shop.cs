using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public bool showOnGUI;

    [SerializeField, Tooltip("How many items the shop can hold, May make infinite later.")]
    private int capacity = 12;

    [Tooltip("Stock of the shop")]
    public List<Item> inventory = new List<Item>();
    [Tooltip("Currently selected item to display info for")]
    private Item selectedItem;

    [Tooltip("The current mark-up, undercut value of the shop.")]
    public float profitMarginHalved = 0.2f;

    [SerializeField, Tooltip("The total amount of money the shop is better off thanks to you. Influences prices.")]
    private float profit;
    public float Profit
    {
        get
        {
            return profit;
        }
        set
        {
            profit = value;
            //like affinity, sets price margin based on amount of profit made off player
            //profit is the absolute difference between trade price and true value
            if (value >= 500)
            {
                profitMarginHalved = 0.05f;
            }
            else if (value >= 100)
            {
                profitMarginHalved = 0.1f;
            }
            else if (value >= 25)
            {
                profitMarginHalved = 0.15f;
            }
            else
            {
                profitMarginHalved = 0.2f;
            }
        }
    }

    [Tooltip("reference to the players inventory")]
    private Inventory playerInventory;

    #region Display shop Variables
    [SerializeField, Tooltip("Display state of shop window")] private bool showShop = false;
    [Tooltip("Screen width and height divided by 16 and 9 (ratio 120:1)")] private Vector2 scr;
    //There might be some Dialogue
    #endregion

    // TODO: these references can be the same for Chest AND Shop. Therefore could have a Base Class OtherInventory that chest and shop inherit from, that way there only needs to be one place to reference.
    #region Canvas UI References and Variables
    [SerializeField, Tooltip("The buttons that will turn into items, TODO: may be used to find capacity later")]
    private Button[] inventoryButtons;

    //selected item
    [SerializeField, Tooltip("The display icon for selected Shop item")]
    private Image selectedIcon;
    [SerializeField, Tooltip("The display name for selected Shop item")]
    private Text selectedName;
    [SerializeField, Tooltip("The display discription for selected Shop item")]
    private Text selectedDiscription;
    [SerializeField, Tooltip("The selected item of Shop primary button")]
    private Button primaryButton;
    [SerializeField, Tooltip("The selected item of Shop secondary button, NOT USED but may use in future.")]
    private Button secondaryButton;

    //Canvas groups
    [SerializeField, Tooltip("The Canvas UI for Shop")]
    private GameObject shopInventoryGroup;
    [SerializeField, Tooltip("The Canvas UI for SPECIFICALLY for Shop SELECTED ITEM")]
    private GameObject selectedItemGroup;

    private int selectedItemStorePrice;
    #endregion

    private void Start()
    {
        //Initialize player inventory reference

        // TODO: maybe get the player inventory stright from the player, rather than search the scene for an inventroy.
        playerInventory = (Inventory) FindObjectOfType<Inventory>();
        if (playerInventory == null)
        {
            Debug.LogError("There is no player with an inventory in this scene");
        }
    }


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
            RefreshSelectedItemDescription();

            UpdateButtons();
        }
        else //otherwise no item selected
        {
            selectedItem = null;
            selectedItemGroup.SetActive(false);//hide selected item window
        }
    }


    /// <summary>
    /// Updates just the description of selected item window, handy for changing amount AND price when going up affinity
    /// </summary>
    public void RefreshSelectedItemDescription()
    {
        if (selectedItem != null)
        {
            //marks up price
            selectedItemStorePrice = (int)(selectedItem.Value * (1f + profitMarginHalved));

            selectedDiscription.text = selectedItem.Description +
                    "\nPrice: $" + selectedItemStorePrice +
                    "\nQuantity: " + selectedItem.Amount;
        }
    }

    /// <summary>
    /// Determines what buttons are available and how they work.
    /// </summary>
    public void UpdateButtons()
    {
        secondaryButton.gameObject.SetActive(false);//no need for secondary in shop

        if (selectedItem != null)
        {
            primaryButton.onClick.RemoveAllListeners();
            primaryButton.gameObject.SetActive(true); //Basically true unless otherwise specified, saves me rewritting.

            primaryButton.onClick.AddListener(PurchaseItemEvent);
            primaryButton.gameObject.GetComponentInChildren<Text>().text = "Buy";
            primaryButton.onClick.AddListener(RefreshInventory);

            //Checks if player can afford AND checks if it would fit in player inventory
            if (playerInventory.CanAddItem(selectedItem) && playerInventory.money >= selectedItemStorePrice)
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
    }

    /// <summary>
    /// Buy items, set up for canvas UI
    /// </summary>
    private void PurchaseItemEvent()
    {
        //attempt to add to player inventory, should always succeed anyways as already checked
        if (playerInventory.AddItemAttempt(selectedItem))
        {
            //if successful, then pay for item and update stock.
            playerInventory.money -= selectedItemStorePrice;
            Profit += selectedItemStorePrice - selectedItem.Value; //shop profits affects shop affinity

            //remove from shop
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
            UpdateButtons(); //if couldnt buy item due to inventory capacity or money, then you sell an item, must update the buy item option. etc.
            playerInventory.UpdateButtons(); //if couldnt sell item due to shop inventory capacity, then you buy an item, must update the sell item option. etc.

        }
    }

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

    /// <summary>
    /// Check if it would be possible to add item
    /// </summary>
    /// <param name="item">The item to test</param>
    /// <returns>Returns true if it would be successful at stocking item in shop. Otherwise false.</returns>
    public bool CanAddItem(Item item)
    {
        Item foundItem = inventory.Find(findItem => findItem.Name == item.Name); //things on the left is paramater, lambda =>  right is expression, each itteration findItem will be the specific item that itteration and it will test it againt the item werre trying to find.

        //checks to see if it can stack with existing shop items, weapons and apparel DONT STACK
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

    private void OnGUI()
    {
        if (showOnGUI)
        {
            scr.x = Screen.width / 16;
            scr.y = Screen.height / 9;

            if (showShop)
            {
                //Display all shop inventory as buttons w text as name.
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
                    //Description, Price, Quantity
                    GUI.Box(new Rect(8.75f * scr.x, 4 * scr.y, 3 * scr.x, 3 * scr.y),
                                        selectedItem.Description +
                                        "\nPrice: " + (int)(selectedItem.Value * (1f + profitMarginHalved)) +
                                        "\nAmount: " + selectedItem.Amount);

                    //Purchase option (only if you can afford)
                    if (playerInventory.money >= (int)(selectedItem.Value * (1f + profitMarginHalved)))
                    {
                        if (GUI.Button(new Rect(10.5f * scr.x, 6.5f * scr.y, scr.x, 0.25f * scr.y), "Purchase Item"))
                        {
                            //Attempts to add item to player
                            if (playerInventory.AddItemAttempt(selectedItem))
                            {
                                //if successful, then pay for item and update stock.
                                playerInventory.money -= (int)(selectedItem.Value * (1f + profitMarginHalved));
                                Profit += (int)(selectedItem.Value * (1f + profitMarginHalved)) - selectedItem.Value;

                                //remove from shop
                                selectedItem.Amount--;
                                if (selectedItem.Amount <= 0)
                                {
                                    inventory.Remove(selectedItem);
                                    selectedItem = null;
                                }
                            }

                        }
                    }
                }

                //display players inv
                playerInventory.showInventory = true;

                if (GUI.Button(new Rect(30, 1020, 120, 60), "Exit Shop"))
                {
                    OpenShopToggle();
                }
            }
        
        }
    }

    /// <summary>
    /// Switch between displaying shop and NOT displaying shop.
    /// </summary>
    public void OpenShopToggle()
    {
        if (showShop)
        {
            playerInventory.HideInventory();
            playerInventory.showInventory = false;

            shopInventoryGroup.SetActive(false);
            selectedItem = null;
            showShop = false;

            playerInventory.gameManager.EnableControls();
        }
        else
        {
            playerInventory.ShowInventory();
            playerInventory.showInventory = true;

            playerInventory.state = Inventory.State.Shop; //determines the buttons and what they do for selected item in player inventory.
            playerInventory.shop = this;

            showShop = true;
            shopInventoryGroup.SetActive(true);
            RefreshInventory();

            playerInventory.gameManager.DisableControls(false);
        }
    }

    /// <summary>
    /// Adding an item to shop inventory. Depends on shop capacity.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Returns true if successful at adding to shop. Otherwise false.</returns>
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
                Debug.Log("That won't work. The shop is over stocked...");
                return false;
            }
        }
    }

    /// <summary>
    /// Adding an item to shop inventory.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void AddItem(Item item)
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
