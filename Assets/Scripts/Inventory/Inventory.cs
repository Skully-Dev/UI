using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public bool showOnGUI;

    [Tooltip("Reference GameManager for common control toggle codes, called from Chest, Shop and Inventory, hence public.")]
    public GameManager gameManager;

    /// <summary>
    /// The various states of the inventory, determines what you can do with inventory items. USE/STORE/SELL etc.
    /// </summary>
    public enum State { Inventory, Chest, Shop, Other }
    [Tooltip("The current state the inventory should be.")]
    public State state = State.Inventory;

    #region Inventory Variables
    [SerializeField, Tooltip("How many item SLOTS available till full")]
    private int capacity = 11;

    [Tooltip("The list of player held items.")]
    public List<Item> inventory = new List<Item>(); //was [SerializeField] private, made public for ConsumablesBar
    [Tooltip("The currently selected item.")]
    public Item selectedItem; //was private, made public for ConsumablesBar
    [SerializeField, Tooltip("Reference to player script")]
    private Player player;

    [Tooltip("The amount of money the player has.")]
    public int money = 100;
    #endregion

    #region Display Inv Variables
    [SerializeField, Tooltip("Is the inventory window open")]
    public bool showInventory = false;
    [Tooltip("Screen width and height divided by 16 and 9 (ratio 120:1)")]
    private Vector2 scr;
    [Tooltip("The scroll bar thingy position")]
    private Vector2 scrollPosition;
    [Tooltip("Current selected sort option, \"\" is sort to ALL")]
    public string sortType = "";
    #endregion

    #region Equipment
    /// <summary>
    /// An Equipment slot w Name, Parent location, Item Instance, and Item info
    /// </summary>
    [Serializable]
    public struct Equipment
    {
        [Tooltip("Chest, Legs, Head, R Hand, L Hand, ETC...")]
        public string slotName;
        [Tooltip("Visuals. The location on the character an object should be spawned when equipt")]
        public Transform equipLocation;
        //nonSerialized means public but NOT in the inspector.
        [Tooltip("The Game Object Spawn of the equipt item")]
        /*[NonSerialized]*/ public GameObject currentItem; //the actual item
        [Tooltip("The typical item information like in inventory, stored w equipment.")]
        /*[NonSerialized]*/ public Item item; 
    };
    /// <summary>
    /// The array of various equiptment slots, check inspector for available slots.
    /// </summary>
    public Equipment[] equipmentSlots;
    #endregion

    [SerializeField, Tooltip("A sphere as a game object for any items without a unique mesh")]
    private GameObject StandInMesh;
    [SerializeField, Tooltip("A game object in scene to store all spawned InWorldItem objects for Inspector organisation.")]
    private GameObject InWorldItemsGroup;

    [Tooltip("A place to reference a used chest")]
    public Chest chest;
    [Tooltip("A place to reference a used shop")]
    public Shop shop;

    #region Canvas UI References and Variables
    [SerializeField]
    private Text moneyText;
    [SerializeField, Tooltip("The buttons that will turn into items, TODO: may be used to find capacity later")]
    private Button[] inventoryButtons;

    //selected item
    [SerializeField, Tooltip("The display icon for selected player inventory item")]
    private Image selectedIcon;
    [SerializeField, Tooltip("The display name for selected player inventory item")]
    private Text selectedName;
    [SerializeField, Tooltip("The display discription for selected player inventory item")]
    private Text selectedDiscription;
    [SerializeField, Tooltip("The selected item of player inventory primary button")]
    private Button primaryButton;
    [SerializeField, Tooltip("The selected item of player inventory secondary button")]
    private Button secondaryButton;


    //Weapon states
    public enum CurrentArmedState { Unarmed, Single, Duel, AlreadyEquipted };
    public CurrentArmedState currentArmedState = CurrentArmedState.Unarmed;

    //Canvas groups
    [SerializeField, Tooltip("The Canvas UI for Player Inventory")]
    public GameObject inventoryGroup;
    [SerializeField, Tooltip("The Canvas UI for SPECIFICALLY for Player Inventory SELECTED ITEM")]
    public GameObject selectedItemGroup;
    #endregion

    private void Update()
    {
        //if user presses I which attempts to open inventory window
        if (Input.GetKeyDown(KeyCode.I)) //I for Inventory
        {
            if (!GameManager.isDisplay) //if not currently in any window displays
            {
                ShowInventory();
                
                state = State.Inventory; //determines the available options for selected item.

                gameManager.DisableControls(false);
            }
            else if (state == State.Inventory) //if inventory open and it is inventroy window
            {
                HideInventory();

                gameManager.EnableControls();
            }
        }
    }

    public void HideInventory()
    {
        selectedItemGroup.SetActive(false);
        inventoryGroup.SetActive(false);
        selectedItem = null;

        showInventory = false; //Close the inventory
    }

    public void ShowInventory()
    {
        inventoryGroup.SetActive(true);
        RefreshInventory();
        showInventory = true;
    }

    #region Canvas UI Methods
    /// <summary>
    /// sets sort type value
    /// </summary>
    /// <param name="typeIndex"></param>
    public void ChangeSort(int typeIndex)
    {
        string[] itemTypes = Enum.GetNames(typeof(ItemType));   //a string array of item types
        int CountOfItemTypes = itemTypes.Length;                //a count of item types

        if (typeIndex < 0 || typeIndex >= itemTypes.Length) //Anything invalid options results in ALL 
        {
            sortType = "";
        }
        else
        {
            sortType = itemTypes[typeIndex];
        }

        //update the other window if needed
        if (state == State.Chest)
        {
            chest.RefreshInventory();
        }
        else if (state == State.Shop)
        {
            shop.RefreshInventory();
        }
    }

    /// <summary>
    /// OnClick of Canvas UI inventory button
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
    /// Updates just the description of selected item window, handy for changing amount
    /// </summary>
    public void RefreshSelectedItemDescription()
    {
        if (selectedItem != null)
        {
            if (state == State.Shop)
            {
                //Description, value, amount
                selectedDiscription.text = selectedItem.Description +
                "\nTrade value: $" + (int)(selectedItem.Value * (1f - shop.profitMarginHalved)) +
                "\nAmount: " + selectedItem.Amount;
            }
            else
            {
                selectedDiscription.text = selectedItem.Description +
                "\nValue: $" + selectedItem.Value +
                "\nQuantity: " + selectedItem.Amount;
            }

        }
    }

    /// <summary>
    /// Refresh the names of the buttons.
    /// </summary>
    public void RefreshInventory()
    {
        Text buttonText;
        int itemCount = inventory.Count;

        moneyText.text = "$" + money.ToString();

        if (sortType == "")
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
                    buttonText.text = inventory[i].Name;
                }
                else
                {
                    buttonText.text = "-Empty Slot-";
                }
            }
        }
        else //Otherwise only display items of type
        {
            ItemType type = (ItemType)Enum.Parse(typeof(ItemType), sortType);

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

    #region Inventory Buttons Events Methods

    private void EatEvent()
    {
        selectedItem.Amount--;

        player.Heal(selectedItem.Heal);

        if (selectedItem.Amount <= 0)
        {
            inventory.Remove(selectedItem);
            selectedItem = null;
            selectedItemGroup.SetActive(false);//hide selected item window
        }
        else
        {
            RefreshSelectedItemDescription();
        }
    }

    private void DrinkEvent()
    {
        selectedItem.Amount--; //reduce item count

        player.RefillStat(selectedItem.Heal, selectedItem.Mana, selectedItem.Stamina); //apply effects

        if (selectedItem.Amount <= 0) //check if no more of item.
        {
            inventory.Remove(selectedItem); //empty the spot in inventory
            selectedItem = null; //clear selection to nothing.
            selectedItemGroup.SetActive(false);//hide selected item window
        }
        else
        {
            RefreshSelectedItemDescription();
        }
    }


    #region Hat Equip Methods
    private void EquipHatEvent()
    {
        if (equipmentSlots[0].currentItem != null) //if a hat is already equipt
        {
            UnequipHat(); //unequip the old hat.
        }

        EquipHat(); //equip the new hat.
    }

    private void UnequipHatEvent()
    {
        UnequipHat();
    }

    private void EquipHat()
    {
        GameObject currentItem = Instantiate(selectedItem.Mesh, equipmentSlots[0].equipLocation); //spawn selected item into appropriate location on character
        equipmentSlots[0].currentItem = currentItem; //reference the spawn
        equipmentSlots[0].item = selectedItem; //copy the item info into the equiptment slot
    }

    private void UnequipHat()
    {
        DestroyImmediate(equipmentSlots[0].currentItem); //destroy the spawn of the item. Visuals.
        equipmentSlots[0].item = null; //set the equipment slot to empty
    }
    #endregion

    #region Weapon Equip Methods
    private void EquipWeaponEvent()
    {
        //determine the situation
        if (equipmentSlots[2].currentItem == null) //if primary slot empty
        {
            currentArmedState = CurrentArmedState.Unarmed;
        }
        else if (equipmentSlots[3].currentItem == null && selectedItem.Name != equipmentSlots[2].item.Name) //if not already equpited AND secondary slot empty
        {
            currentArmedState = CurrentArmedState.Single;
        }
        else if (selectedItem.Name != equipmentSlots[2].item.Name && selectedItem.Name != equipmentSlots[3].item.Name) //if not already equipted but BOTH hands are full
        {
            currentArmedState = CurrentArmedState.Duel;
        }

        //act accordingly
        if (currentArmedState == CurrentArmedState.Unarmed)
        {
            EquipPrimaryHand();
        }
        else if (currentArmedState == CurrentArmedState.Single)
        {
            EquipSecondaryHand();
        }
        else if (currentArmedState == CurrentArmedState.Duel)
        {
            UnequipSecondaryHand(); //unequip current secondary
            EquipSecondaryHand(); //equip new secondary
        }
    }

    private void UnequipWeaponEvent()
    {
        if (selectedItem.Name == equipmentSlots[2].item.Name) //if it is the primary weapon you are trying to unequip
        {
            UnequipPrimaryHand();
            if (equipmentSlots[3].currentItem != null) //if holding a secondary weapon
            {
                SwapSecondaryToPrimaryHand(); //swap it to primary
            }
        }
        else //otherwise it is the secondary weapon you are trying to unequip
        {
            UnequipSecondaryHand();
        }
    }

    private void EquipPrimaryHand()
    {
        GameObject currentItem = Instantiate(selectedItem.Mesh, equipmentSlots[2].equipLocation); //the spawn of the item
        equipmentSlots[2].currentItem = currentItem; //reference to instance of the spawned object
        equipmentSlots[2].item = selectedItem; //copy the info of the new equipted weapon
    }
    private void EquipSecondaryHand()
    {
        GameObject currentItem = Instantiate(selectedItem.Mesh, equipmentSlots[3].equipLocation); //the spawn of the item
        equipmentSlots[3].currentItem = currentItem; //reference to instance of the spawned object
        equipmentSlots[3].item = selectedItem; //copy the info of the new equipted weapon
    }

    private void UnequipPrimaryHand()
    {
        DestroyImmediate(equipmentSlots[2].currentItem);
        //Destroy(equipmentSlots[2].currentItem); //remove the spawn of primary
        equipmentSlots[2].item = null;
    }

    private void UnequipSecondaryHand()
    {
        DestroyImmediate(equipmentSlots[3].currentItem); //delete the spawn of the secondary
        equipmentSlots[3].item = null; //secondary no longer has info so null
    }

    private void SwapSecondaryToPrimaryHand()
    {
        GameObject currentItem = Instantiate(equipmentSlots[3].item.Mesh, equipmentSlots[2].equipLocation); //spawn the secondary into primary position
        equipmentSlots[2].currentItem = currentItem; //replace reference to instance
        equipmentSlots[2].item = equipmentSlots[3].item; //copy info into primary
        UnequipSecondaryHand(); //BASICALLY remove the double up, weapon can't be in both hands.
    }
    #endregion

    public void DiscardEvent()
    {
        selectedItem.Amount--;
        if (selectedItem.Mesh == null)
        {
            selectedItem.Mesh = StandInMesh; //adds a default mesh for objects without a unique one.
        }
        if (selectedItem.Mesh != null)
        {
            //  TODO: do a ray cast, sky down, for ground layer, so item doesnt get droped through the surface of the earth! //^^^^ if done, may need to rearrage code to make (selectedItem.Amount--;) happen AFTER a successful drop.
            //drops items within a random circle infront of player
            GameObject spawn = Instantiate(selectedItem.Mesh, player.transform.position + player.transform.forward * 2 + Vector3.up + UnityEngine.Random.insideUnitSphere * 1.8f + player.transform.forward, Quaternion.identity);
            spawn.GetComponent<InWorldItem>().item = new Item(selectedItem, 1); // Creates a NEW reference, w same info but behaves independently.
            spawn.GetComponent<Rigidbody>().isKinematic = false; // Allows gravity
            spawn.layer = LayerMask.NameToLayer("Interactable"); // Allows interaction (to pick up)
            spawn.transform.parent = InWorldItemsGroup.transform; //puts the item into a group as to keep inspector tidy.
        }

        //Checks to see if item slot should be free'd up.
        if (selectedItem.Amount <= 0)
        {
            inventory.Remove(selectedItem);
            selectedItem = null;
            selectedItemGroup.SetActive(false);//hide selected item window
        }
        else
        {
            RefreshSelectedItemDescription();
        }
    }

    #endregion

    /// <summary>
    /// Determines what buttons are available and how they work based on selected item TYPE, and Equipt Status
    /// </summary>
    public void UpdateButtons()
    {
        if (selectedItem != null) //if nothing selected, do nothing.
        {
            if (state == State.Inventory)
            {
                primaryButton.onClick.RemoveAllListeners();
                primaryButton.gameObject.SetActive(true); //Basically true unless otherwise specified, saves me rewritting.
                primaryButton.interactable = true; //Basically true unless otherwise specified, saves me rewritting.

                secondaryButton.onClick.RemoveAllListeners();
                secondaryButton.gameObject.SetActive(true);
                secondaryButton.interactable = true;
                secondaryButton.GetComponentInChildren<Text>().text = "Discard";
                secondaryButton.onClick.AddListener(DiscardEvent);

                #region Primary Button
                switch (selectedItem.Type) //using switch TAB to auto fill, when you type (selectedItem.Type) and press enter it will auto fill the cases.
                {
                    case ItemType.Food:
                        primaryButton.GetComponentInChildren<Text>().text = "Eat";
                        primaryButton.onClick.AddListener(EatEvent);

                        if (player.playerStats.CurrentHealth < player.playerStats.stats.maxHealth)
                        {
                            primaryButton.enabled = true;
                        }
                        else
                        {
                            primaryButton.interactable = false;
                        }
                        break;
                    case ItemType.Weapon:
                        if (equipmentSlots[2].currentItem == null) //if primary slot empty
                        {
                            currentArmedState = CurrentArmedState.Unarmed;
                            primaryButton.GetComponentInChildren<Text>().text = "Equip";
                            primaryButton.onClick.AddListener(EquipWeaponEvent);
                        }
                        else if (equipmentSlots[3].currentItem == null && selectedItem.Name != equipmentSlots[2].item.Name) //if not already equpited AND secondary slot empty
                        {
                            currentArmedState = CurrentArmedState.Single;
                            primaryButton.GetComponentInChildren<Text>().text = "Equip";
                            primaryButton.onClick.AddListener(EquipWeaponEvent);
                        }
                        else if (selectedItem.Name != equipmentSlots[2].item.Name && selectedItem.Name != equipmentSlots[3].item.Name) //if not already equipted but BOTH hands are full
                        {
                            currentArmedState = CurrentArmedState.Duel;
                            primaryButton.GetComponentInChildren<Text>().text = "Equip";
                            primaryButton.onClick.AddListener(EquipWeaponEvent);
                        }
                        else //otherwise you already have one of this weapon equipted
                        {
                            currentArmedState = CurrentArmedState.AlreadyEquipted;
                            primaryButton.GetComponentInChildren<Text>().text = "Unequip";
                            primaryButton.onClick.AddListener(UnequipWeaponEvent);

                            secondaryButton.interactable = false; //cant drop while equipt
                        }
                        break;
                    case ItemType.Apparel:
                        if (equipmentSlots[0].currentItem == null || selectedItem.Name != equipmentSlots[0].item.Name) //If no apparel equipt or selected item is different to equipt item.
                        {
                            primaryButton.GetComponentInChildren<Text>().text = "Equip";
                            primaryButton.onClick.AddListener(EquipHatEvent);
                        }
                        else
                        {
                            primaryButton.GetComponentInChildren<Text>().text = "Unequip";
                            primaryButton.onClick.AddListener(UnequipHatEvent);

                            secondaryButton.interactable = false; //cant drop while equipt
                        }
                        break;
                    case ItemType.Crafting:
                        primaryButton.gameObject.SetActive(false); //no option, hide button
                        break;
                    case ItemType.Ingredients:
                        primaryButton.gameObject.SetActive(false); //no option, hide button
                        break;
                    case ItemType.Potions:
                        primaryButton.GetComponentInChildren<Text>().text = "Drink";
                        primaryButton.onClick.AddListener(DrinkEvent);
                        break;
                    case ItemType.Scrolls:
                        primaryButton.gameObject.SetActive(false); //no option, hide button
                        break;
                    case ItemType.Quest:
                        primaryButton.gameObject.SetActive(false); //no option, hide button
                        secondaryButton.gameObject.SetActive(false); //cant drop quest items
                        break;
                    case ItemType.Money:  //Is auto converted and added into money float variables
                        break;
                    default:
                        primaryButton.gameObject.SetActive(false); //no option, hide button
                        secondaryButton.gameObject.SetActive(false); //no option, hide button
                        break;
                }
                #endregion

                primaryButton.onClick.AddListener(UpdateButtons);
                secondaryButton.onClick.AddListener(RefreshInventory);
            }
            else if (state == State.Chest)
            {
                secondaryButton.gameObject.SetActive(false);//no need for secondary in chest

                if (selectedItem != null)
                {
                    primaryButton.onClick.RemoveAllListeners();
                    primaryButton.gameObject.SetActive(true); //Basically true unless otherwise specified, saves me rewritting.

                    primaryButton.onClick.AddListener(DepositeItemEvent);
                    primaryButton.gameObject.GetComponentInChildren<Text>().text = "Deposite";
                    primaryButton.onClick.AddListener(RefreshInventory);

                    //attempt to add to player inventory
                    if (chest.CanAddItem(selectedItem))
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
                    primaryButton.gameObject.SetActive(false); //Just to avoid depositing null items.
                }
            }
            else if (state == State.Shop)
            {
                secondaryButton.gameObject.SetActive(false);//no need for secondary in shop

                if (selectedItem != null && !IsEquipt())
                {
                    if (selectedItem.Type != ItemType.Quest)
                    {
                        primaryButton.onClick.RemoveAllListeners();
                        primaryButton.gameObject.SetActive(true); //Basically true unless otherwise specified, saves me rewritting.

                        primaryButton.onClick.AddListener(SellItemEvent);
                        primaryButton.gameObject.GetComponentInChildren<Text>().text = "Sell";
                        primaryButton.onClick.AddListener(RefreshInventory);

                        //attempt to add to player inventory
                        if (shop.CanAddItem(selectedItem))
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
                        //QUEST ITEM or CURRENTLY EQUIPT
                        primaryButton.gameObject.SetActive(false); //Cant sell quest items or equipt items

                    }

                }
                else
                {
                    primaryButton.gameObject.SetActive(false); //Just to avoid depositing null items.
                }
            }
        }
    }
    #endregion

    public void SellItemEvent()
    {
        //attempt to add to shop, should always succeed anyways as already checked
        if (shop.AddItemAttempt(selectedItem))
        {
            //If selling the item was successful

            //adjust money stuff
            int sellPrice = (int)(selectedItem.Value * (1f - shop.profitMarginHalved));
            money += sellPrice;
            shop.Profit += selectedItem.Value - sellPrice; //shop profit determines your discount


            //remove from player inventory
            selectedItem.Amount--;
            shop.AddItem(selectedItem);

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

            shop.RefreshInventory(); //refresh the shop items
            shop.RefreshSelectedItemDescription(); //refresh the chest selected item description
            UpdateButtons(); //Incase conditions have now changed.
            shop.UpdateButtons(); //Incase conditions have now changed.
        }
    }

    /// <summary>
    /// Deposite item, set up for canvas UI
    /// </summary>
    public void DepositeItemEvent()
    {
        //attempt to add to chest, should always succeed anyways as already checked
        if (chest.AddItemAttempt(selectedItem))
        {
            //If depositing the item was successful, remove from player inventory
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

            chest.RefreshInventory(); //refresh the chest items
            chest.RefreshSelectedItemDescription(); //refresh the chest selected item description
            chest.UpdateButtons(); //if couldnt take an item, then you store an item, must update the take item option. etc.
        }
    }

    /// <summary>
    /// Checks inventory for item of same name.
    /// </summary>
    /// <param name="itemName"></param>
    /// <returns>Returns inventory item if found. Otherwise null</returns>
    public Item FindItem(string itemName)
    {
        //for each item in inventory, check if inventory item name == item name
            //if so return that as found item
        //goes through each item in inventory and compairs to item passed in
        Item foundItem = inventory.Find(findItem => findItem.Name == itemName); //things on the left is paramater, lambda =>  right is expression, each itteration findItem will be the specific item that itteration and it will test it againt the item werre trying to find.

        return foundItem;
    }

    /// <summary>
    /// SHOULD BE USED AS CONDITION.
    /// Attempts to add item to inventroy.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Returns true if successful at adding to inventory. Otherwise false.</returns>
    public bool AddItemAttempt(Item item)
    {
        if (item.Type.ToString() == "Money")
        {
            money += item.Value; //If money, converts into float money instead of object.
            return true;
        }
        else
        {
            Item foundItem = inventory.Find(findItem => findItem.Name == item.Name); //things on the left is paramater, lambda =>  right is expression, each itteration findItem will be the specific item that itteration and it will test it againt the item werre trying to find.

            //checks to see if it can stack with existing inventory items, weapons and apparel DONT STACK
            if ((item.Type != ItemType.Apparel && item.Type != ItemType.Weapon) && foundItem != null )
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
                    Debug.Log("I can't carry that! I've got no more space for new items.");
                    return false;
                }
            }
        }
    }

    /// <summary>
    /// Check if it would be possible to add item
    /// </summary>
    /// <param name="item">The item to test</param>
    /// <returns>Returns true if it would be successful at adding to inventory. Otherwise false.</returns>
    public bool CanAddItem(Item item)
    {
        if (item.Type.ToString() == "Money")
        {
            return true;
        }
        else
        {
            Item foundItem = inventory.Find(findItem => findItem.Name == item.Name); //things on the left is paramater, lambda =>  right is expression, each itteration findItem will be the specific item that itteration and it will test it againt the item werre trying to find.

            //checks to see if it can stack with existing inventory items, weapons and apparel DONT STACK
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
    }


    /// <summary>
    /// Checks if the selected item is currently equipted in ANY of the equipment slots.
    /// </summary>
    /// <returns>Returns true if item is currently equpt. Otherwise false.</returns>
    public bool IsEquipt()
    {
        foreach (var equipmentSlot in equipmentSlots)
        {
            // TODO: Make paramater for item rather than using selected item.
            if (selectedItem != null)//fixed odd error, will attempt to remove later.
            {
                if (equipmentSlot.currentItem != null)
                {
                    if (selectedItem.Name == equipmentSlot.item.Name)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    #region Converted OnGUI
    /// <summary>
    /// Shows inventory items as buttons w name. Stacked vertically.
    /// Only shows items of current sort type.
    /// Scrollable.
    /// If item button clicked, sets item as selectedItem.
    /// </summary>
    private void DisplayItemsOnGUI()
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

    /// <summary>
    /// Display Selected item information. 
    /// Determines what buttons are available and how they work based on selected item TYPE, and Equipt Status
    /// </summary>
    private void UseItemOnGUI()
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
                            equipmentSlots[2].item = null;
                            if (equipmentSlots[3].currentItem != null)
                            {
                                GameObject currentItem = Instantiate(equipmentSlots[3].item.Mesh, equipmentSlots[2].equipLocation); //spawn the secondary into primary position
                                equipmentSlots[2].currentItem = currentItem; //replace reference to instance
                                equipmentSlots[2].item = equipmentSlots[3].item; //copy info into primary
                                Destroy(equipmentSlots[3].currentItem); //delete the spawn of the secondary
                                equipmentSlots[3].item = null; //secondary no longer has info so null
                            }
                        }
                        else
                        {
                            Destroy(equipmentSlots[3].currentItem); //delete the spawn of the secondary
                            equipmentSlots[3].item = null; //secondary no longer has info so null
                        }
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
                if (equipmentSlots[0].currentItem == null || selectedItem.Name != equipmentSlots[0].item.Name) //If no apparel equipt or selected item is different to equipt item.
                {
                    //show equip button
                    if (GUI.Button(new Rect(4.75f * scr.x, 6.5f * scr.y, scr.x, 0.25f * scr.y), "Equip")) 
                    {
                        if (equipmentSlots[0].currentItem != null) //if item already equipt
                        {
                            Destroy(equipmentSlots[0].currentItem); //destroy the spawn of the old item.
                        }
                        GameObject currentItem = Instantiate(selectedItem.Mesh, equipmentSlots[0].equipLocation); //spawn selected item into appropriate location on character
                        equipmentSlots[0].currentItem = currentItem; //reference the spawn
                        equipmentSlots[0].item = selectedItem; //copy the item info into the equiptment slot
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(4.75f * scr.x, 6.5f * scr.y,
                        scr.x, 0.25f * scr.y), "Unequip"))
                    {
                        Destroy(equipmentSlots[0].currentItem); //destroy the spawn of the item. Visuals.
                        equipmentSlots[0].item = null; //set the equipment slot to empty
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
                    selectedItem.Amount--; //reduce item count

                    player.RefillStat(selectedItem.Heal, selectedItem.Mana, selectedItem.Stamina); //apply effects

                    if (selectedItem.Amount <= 0) //check if no more of item.
                    {
                        inventory.Remove(selectedItem); //empty the spot in inventory
                        selectedItem = null; //clear selection to nothing.
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

        if (selectedItem.Type != ItemType.Quest) //No tossing quest items
        {
            if (GUI.Button(new Rect(6f * scr.x, 6.5f * scr.y, scr.x, 0.25f * scr.y), "Discard"))
            {
                if (IsEquipt()) //no discarding equipted items, this could happen before like quest items check, therefore it won't show discard for equipt items, but this also makes the check happen continuiously instead of just on button press...
                {
                    Debug.Log("You must unequipt first");
                }
                else
                {
                    //Therefore discardable

                    selectedItem.Amount--;
                    if (selectedItem.Mesh == null)
                    {
                        selectedItem.Mesh = StandInMesh; //adds a default mesh for objects without a unique one.
                    }
                    if (selectedItem.Mesh != null)
                    {
                        //  TODO: do a ray cast, sky down, for ground layer, so item doesnt get droped through the surface of the earth! //^^^^ if done, may need to rearrage code to make (selectedItem.Amount--;) happen AFTER a successful drop.
                        //drops items within a random circle infront of player
                        GameObject spawn = Instantiate(selectedItem.Mesh, player.transform.position + player.transform.forward * 2 + Vector3.up + UnityEngine.Random.insideUnitSphere * 1.8f + player.transform.forward, Quaternion.identity);
                        spawn.GetComponent<InWorldItem>().item = new Item(selectedItem, 1); // Creates a NEW reference, w same info but behaves independently.
                        spawn.GetComponent<Rigidbody>().isKinematic = false; // Allows gravity
                        spawn.layer = LayerMask.NameToLayer("Interactable"); // Allows interaction (to pick up)
                        spawn.transform.parent = InWorldItemsGroup.transform; //puts the item into a group as to keep inspector tidy.
                    }

                    //Checks to see if item slot should be free'd up.
                    if (selectedItem.Amount <= 0)
                    {
                        inventory.Remove(selectedItem);
                        selectedItem = null;
                    }
                }
            }
        }
    }

    /// <summary>
    /// A button for putting inventory items into a chest.
    /// </summary>
    private void DepositeItemOnGUI()
    {
        //Description, value, amount
        GUI.Box(new Rect(4.25f * scr.x, 4 * scr.y,
                         3 * scr.x,
                         3 * scr.y),
                         selectedItem.Description +
                         "\nValue: " + selectedItem.Value +
                         "\nAmount: " + selectedItem.Amount);

        if (GUI.Button(new Rect(4.5f * scr.x, 6.5f * scr.y,
                scr.x, 0.25f * scr.y), "Deposite"))
        {
            //Try adding item to chest
            if (chest.AddItemAttempt(selectedItem))
            {
                //If successful, reduce amount of item
                selectedItem.Amount--;

                if (selectedItem.Amount <= 0)
                {
                    inventory.Remove(selectedItem);
                    selectedItem = null;
                }
            }
        }
    }
    #endregion

    /// <summary>
    /// A button for selling items to a shop.
    /// </summary>
    private void SellItemOnGUI()
    {
        //Description, value, amount
        GUI.Box(new Rect(4.25f * scr.x, 4 * scr.y,
                         3 * scr.x,
                         3 * scr.y), 
                         selectedItem.Description +
                         "\nAmount: " + selectedItem.Amount +
                         "\nTrade value: " + (int)(selectedItem.Value * (1f - shop.profitMarginHalved)));

        
        if (selectedItem.Type != ItemType.Quest) // can't sell quest items.
        {
            if (GUI.Button(new Rect(4.5f * scr.x, 6.5f * scr.y, scr.x, 0.25f * scr.y), "Sell"))
            {
                if (IsEquipt())
                {
                    Debug.Log("You must unequip first");
                }
                else
                {
                    //sell item.

                    selectedItem.Amount--;
                    shop.AddItem(selectedItem);

                    money += (int)(selectedItem.Value * (1f - shop.profitMarginHalved));
                    shop.Profit += selectedItem.Value - (int)(selectedItem.Value * (1f - shop.profitMarginHalved)); //shop profit determines your discount

                    if (selectedItem.Amount <= 0)
                    {
                        inventory.Remove(selectedItem);
                        selectedItem = null;
                    }
                }
            }
        }
    }

    private void OnGUI()
    {
        if (showOnGUI)
        {
            //even square boxes
            scr.x = Screen.width / 16;
            scr.y = Screen.height / 9;

            if (showInventory)
            {
                //full screen backdrop
                GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

                //player money
                GUI.Box(new Rect(250, 20, 170, 40), "Money: " + money);

                //get count of all item types
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

                DisplayItemsOnGUI();
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
                        UseItemOnGUI();
                    }
                    if (state == State.Chest)
                    {
                        DepositeItemOnGUI();
                    }
                    if (state == State.Shop)
                    {
                        SellItemOnGUI();
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
}
