using UnityEngine;
using UnityEngine.UI;

public class ConsumablesBar : MonoBehaviour
{
    [Header("GUI Variabels")]

    public bool showOnGUI;
    [Tooltip("Screen width and height divided by 16 and 9 (ratio 120:1)")]
    private Vector2 scr;

    [Header("External References")]

    [SerializeField, Tooltip("Reference to player for stats.")]
    private Player player;
    [SerializeField, Tooltip("Reference to player inventory to access items")]
    private Inventory playerInventory;

    /// <summary>
    /// Quick button settings
    /// </summary>
    [System.Serializable]
    public struct QuickItem
    {
        [Tooltip("1,2,3,4...")]
        public string buttonText;
        [Tooltip("Alpha1,Alpha2...")]
        public KeyCode keyCode;
        [Tooltip("a Reference of the bound item")]
        /*[NonSerialized]*/ public Item item;
    };
    [Header("Button Settings"), Tooltip("All Quick buttons information")]
    public QuickItem[] hotbar;

    #region Canvas References and Variables
    [Header("Canvas UI Button References")]
    [SerializeField, Tooltip("Reference the button groups, used to automatically Set the other required references")] private GameObject[] buttonGroups;
    [Header("Set Automatically in Script")]
    private Button[] buttons = new Button[6];
    private Image[] buttonIcons = new Image[6]; //for icon of bound item
    private GameObject[] cooldownObjects = new GameObject[6]; //for cooldown visuals
    private Text[] cooldownTexts = new Text[6]; //for cooldown timer
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //initalize text and keycodes, automated.
        for (int i = 0; i < hotbar.Length; i++)
        {
            hotbar[i].buttonText = (i + 1).ToString();
            hotbar[i].keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + hotbar[i].buttonText);
        }

        for (int i = 0; i < buttonGroups.Length; i++)
        {
            #region Get References
            //GET REFERENCES
            //Initialize button references
            buttons[i] = buttonGroups[i].GetComponentInChildren<Button>();
            //Initialize Button Icon References
            buttonIcons[i] = buttons[i].transform.GetChild(1).GetComponent<Image>();
            //Initialize Cooldown game objects References
            cooldownObjects[i] = buttonGroups[i].transform.GetChild(1).gameObject;
            //Initialize Cooldown text References
            cooldownTexts[i] = cooldownObjects[i].GetComponentInChildren<Text>();
            #endregion

            #region Set Reference Values
            //rename button text to button numbers
            buttons[i].GetComponentInChildren<Text>().text = (i + 1).ToString();

            //SET VISUALS
            //Icon
            if (hotbar[i].item.Icon != null)
            {
                buttonIcons[i].gameObject.SetActive(true);

                Sprite sprite = Sprite.Create(hotbar[i].item.Icon, buttonIcons[i].sprite.rect, buttonIcons[i].sprite.pivot);
                buttonIcons[i].sprite = sprite;
            }
            else
            {
                buttonIcons[i].gameObject.SetActive(false);
            }
            
            //Cooldowns off
            cooldownObjects[i].SetActive(false);
            #endregion
        }
    }

    private void Update()
    {
        for (int i = 0; i < hotbar.Length; i++)
        {
            if (Input.GetKeyDown(hotbar[i].keyCode))
            {
                UseButton(i);
            }

            CooldownUpdate(i);
        }
    }

    public void UseButton(int index)
    {
        //if inventory open
        if (playerInventory.showInventory)
        {
            if (playerInventory.selectedItem != null) //if item is selected
            {
                //if selected item is consumable
                if (playerInventory.selectedItem.Type == ItemType.Food || playerInventory.selectedItem.Type == ItemType.Potions)
                {
                    //add selected item as item to use, for hotkey
                    hotbar[index].item = playerInventory.selectedItem;

                    //apply icon to associated button
                    buttonIcons[index].gameObject.SetActive(true);
                    Sprite sprite = Sprite.Create(hotbar[index].item.Icon, buttonIcons[index].sprite.rect, buttonIcons[index].sprite.pivot);
                    buttonIcons[index].sprite = sprite;

                    if (hotbar[index].item.CooldownTermination > Time.time) //if a cooldown was active
                    {
                        //display cooldown information visuals
                        cooldownObjects[index].SetActive(true);
                        cooldownTexts[index].text = (hotbar[index].item.CooldownTermination - Time.time).ToString("0");
                    }

                    playerInventory.gameManager.PlayButtonSound();
                }
            }
        }
        else if (!GameManager.isDisplay && hotbar[index].item != null && hotbar[index].item.Amount > 0 && hotbar[index].item.CooldownTermination < Time.time) //if no display windows open AND item isnt null AND have items in stock AND timer is up
        {
            //determine how to use the item
            switch (hotbar[index].item.Type)
            {
                case ItemType.Food:
                    player.Heal(hotbar[index].item.Heal);
                    UseConsumableGenerics(index);
                    break;
                case ItemType.Potions:
                    player.RefillStat(hotbar[index].item.Heal, hotbar[index].item.Mana, hotbar[index].item.Stamina);
                    UseConsumableGenerics(index);
                    break;
                default:
                    break;
            }
        }
    }

    private void UseConsumableGenerics(int index)
    {
        //reduce quantity of item ledft
        hotbar[index].item.Amount--;

        if (hotbar[index].item.Amount <= 0) //if no more of that item left
        {
            //remove from player inventory
            playerInventory.inventory.Remove(hotbar[index].item);

            //Unbind from button
            buttonIcons[index].gameObject.SetActive(false);
            cooldownObjects[index].SetActive(false);
            hotbar[index].item = null;
        }
        else //If more of that item left
        {
            //Set Cooldown stuff
            hotbar[index].item.CooldownTermination = Time.time + hotbar[index].item.Cooldown;

            //Takes into account EVERY consumable UI button
            for (int ii = 0; ii < hotbar.Length; ii++)
            {
                if (hotbar[ii].item != null && hotbar[ii].item.Name == hotbar[index].item.Name)
                {
                    cooldownObjects[ii].SetActive(true);
                }
            }
        }
    }

    public void CooldownUpdate(int index)
    {
        if (hotbar[index].item != null) //if item bound
        {
            if (hotbar[index].item.Amount > 0) //if stock of item
            {
                if (cooldownObjects[index].activeSelf) //if cooldown was still active
                {
                    if (Time.time < hotbar[index].item.CooldownTermination) //if countdown still not up
                    {
                        //update countdown timer text
                        cooldownTexts[index].text = (hotbar[index].item.CooldownTermination - Time.time).ToString("0");
                    }
                    else
                    {
                        //Otherwise timer up, make visuals normal again.
                        cooldownObjects[index].SetActive(false);
                    }
                }
            }
            else //if out of stock
            {
                //Unbind item

                //hide icon
                buttonIcons[index].gameObject.SetActive(false);
                //hide cooldown visuals
                cooldownObjects[index].SetActive(false);
                //remove reference
                hotbar[index].item = null;
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

            for (int i = 0; i < hotbar.Length; i++)
            {
                //if button press or related hotkey keycode pressed
                if (GUI.Button(new Rect(5 * scr.x + i * scr.x, 8 * scr.y, scr.x, scr.y), hotbar[i].item.Icon) || Input.GetKeyDown(hotbar[i].keyCode))
                {
                    Debug.Log(hotbar[i].buttonText);

                    //if in inventory
                    if (playerInventory.showInventory)
                    {
                        if (playerInventory.selectedItem != null)
                        {
                            //if selected item is consumable
                            if (playerInventory.selectedItem.Type == ItemType.Food || playerInventory.selectedItem.Type == ItemType.Potions)
                            {
                                //add selected item as item to use for hotkey
                                hotbar[i].item = playerInventory.selectedItem;
                            }
                        }
                    }
                    else if (!GameManager.isDisplay && hotbar[i].item.Amount > 0 && hotbar[i].item.CooldownTermination < Time.time) //if no display windows open and have items in stock && timer is up
                    {
                        //determine how to use the item
                        switch (hotbar[i].item.Type)
                        {
                            case ItemType.Food:
                                hotbar[i].item.Amount--;

                                player.Heal(hotbar[i].item.Heal);

                                hotbar[i].item.CooldownTermination = Time.time + hotbar[i].item.Cooldown;

                                if (hotbar[i].item.Amount <= 0)
                                {
                                    playerInventory.inventory.Remove(hotbar[i].item);
                                    break;
                                }
                                break;
                            case ItemType.Potions:
                                hotbar[i].item.Amount--;

                                player.RefillStat(hotbar[i].item.Heal, hotbar[i].item.Mana, hotbar[i].item.Stamina);

                                hotbar[i].item.CooldownTermination = Time.time + hotbar[i].item.Cooldown;

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

                if (hotbar[i].item.CooldownTermination > Time.time)
                {
                    GUI.Box(new Rect(5 * scr.x + i * scr.x, 8 * scr.y, scr.x, scr.y), (hotbar[i].item.CooldownTermination - Time.time).ToString("0"));
                }
                else if (hotbar[i].item.Amount <= 0)
                {
                    GUI.Box(new Rect(5 * scr.x + i * scr.x, 8 * scr.y, scr.x, scr.y), "");
                }


                //hotkey Number in top left corner
                GUI.Box(new Rect(5 * scr.x + i * scr.x, 8 * scr.y, 0.2f * scr.x, 0.2f * scr.y), hotbar[i].buttonText);
            }
        }
    }
        
}