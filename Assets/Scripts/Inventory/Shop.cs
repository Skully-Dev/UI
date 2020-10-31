using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public List<Item> shopInventory = new List<Item>();
    private Item selectedItem;

    private Inventory playerInventory;

    #region Display shop Variables
    [SerializeField] private bool showShop = false;
    private Vector2 scr;
    //There might be some Dialogue

    #endregion

    private void Start()
    {
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
                GUI.Box(new Rect(8.5f * scr.x, 0.25f * scr.y,
                                    3.5f * scr.x, 7 * scr.y), "");
                GUI.Box(new Rect(8.75f * scr.x, 0.5f * scr.y,
                                    3 * scr.x, 3 * scr.y), selectedItem.Icon);
                GUI.Box(new Rect(9.05f * scr.x, 3.5f * scr.y,
                                    2.5f * scr.x, .5f * scr.y), selectedItem.Name);
                GUI.Box(new Rect(8.75f * scr.x, 4 * scr.y, 3 * scr.x, 3 * scr.y),
                                    selectedItem.Description + "\nValue: " +
                                    selectedItem.Value + "\nAmount: " + selectedItem.Amount);

                if (playerInventory.money >= selectedItem.Value)
                {
                    if (GUI.Button(new Rect(10.5f * scr.x, 6.5f * scr.y, scr.x, 0.25f * scr.y), "Purchase Item"))
                    {
                        playerInventory.money -= selectedItem.Value; //maybe convert money variable by removing and just using objects of type money, where you give money objects to buy and get money objects to sell.

                        //add to player
                        playerInventory.AddItem(selectedItem);

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

            //display players inv
            playerInventory.showInventory = true;

            if (GUI.Button(new Rect(.25f * scr.x, 8.5f * scr.y, scr.x, .5f * scr.y), "Exit Shop"))
            {
                playerInventory.showInventory = false;
                showShop = false;
            }
        }
    }
}
