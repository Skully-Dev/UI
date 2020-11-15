using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public bool showOnGUI;

    [Tooltip("Stock of the shop")]
    public List<Item> shopInventory = new List<Item>();
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

    private void OnGUI()
    {
        if (showOnGUI)
        {
            scr.x = Screen.width / 16;
            scr.y = Screen.height / 9;

            if (showShop)
            {
                //Display all shop inventory as buttons w text as name.
                for (int i = 0; i < shopInventory.Count; i++)
                {
                    if (GUI.Button(new Rect(12.5f * scr.x, (0.25f * scr.y) + i * (0.25f * scr.y),
                                             3 * scr.x, .25f * scr.y), shopInventory[i].Name))
                    {
                        selectedItem = shopInventory[i];
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
                                    shopInventory.Remove(selectedItem);
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
            playerInventory.showInventory = false;
            showShop = false;

            playerInventory.gameManager.EnableControls();
        }
        else
        {
            playerInventory.state = Inventory.State.Shop; //determines the buttons and what they do for selected item in player inventory.
            playerInventory.shop = this;

            showShop = true;

            playerInventory.gameManager.DisableControls(false);
        }
    }

    /// <summary>
    /// Adding an item to shop inventory.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void AddItem(Item item)
    {
        Item foundItem = shopInventory.Find(findItem => findItem.Name == item.Name); //things on the left is paramater, lambda =>  right is expression, each itteration findItem will be the specific item that itteration and it will test it againt the item werre trying to find.

        if (foundItem != null)
        {
            foundItem.Amount++;
        }
        else
        {
            Item newItem = new Item(item, 1);
            shopInventory.Add(newItem);
        }
    }
}
