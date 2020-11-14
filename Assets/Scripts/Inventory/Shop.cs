using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [Tooltip("Stock of the shop")]
    public List<Item> shopInventory = new List<Item>();
    [Tooltip("Currently selected item to display info for")]
    private Item selectedItem;

    public float profitMarginHalved = 0.2f;

    [SerializeField]
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

    private Inventory playerInventory;

    #region Display shop Variables
    [SerializeField] private bool showShop = false;
    private Vector2 scr;
    //There might be some Dialogue
    #endregion

    private void Start()
    {
        // FIXME: Would make more sense to get the player inventory stright from the player, rather than search the scene for an inventroy.
        playerInventory = (Inventory) FindObjectOfType<Inventory>();
        if (playerInventory == null)
        {
            Debug.LogError("There is no player with an inventory in this scene");
        }
    }

    private void OnGUI()
    {
        scr.x = Screen.width / 16;
        scr.y = Screen.height / 9;

        if (showShop)
        {
            for (int i = 0; i < shopInventory.Count; i++)
            {
                if (GUI.Button(new Rect(12.5f * scr.x, (0.25f * scr.y) + i * (0.25f * scr.y),
                                         3 * scr.x, .25f * scr.y), shopInventory[i].Name))
                {
                    selectedItem = shopInventory[i];
                }
            }

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
                        if (playerInventory.AddItem(selectedItem))
                        {
                            //if successful, then pay for item and update stock.
                            playerInventory.money -= (int)(selectedItem.Value * (1f + profitMarginHalved)); //maybe convert money variable by removing and just using objects of type money, where you give money objects to buy and get money objects to sell.
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
            playerInventory.state = Inventory.State.Shop;
            playerInventory.shop = this;

            showShop = true;

            playerInventory.gameManager.DisableControls(false);
        }
    }

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
