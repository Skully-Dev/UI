using System;
using UnityEngine;
using UnityEngine.UI;

public class ConsumablesBar : MonoBehaviour
{
    public bool showOnGUI;

    [SerializeField, Tooltip("Reference to player for stats.")]
    private Player player;
    [SerializeField, Tooltip("Reference to player inventory to access items")]
    private Inventory playerInventory;

    [Tooltip("Screen width and height divided by 16 and 9 (ratio 120:1)")] private Vector2 scr;

    /// <summary>
    /// Quick button settings
    /// </summary>
    [Serializable]
    public struct QuickItem
    {
        [Tooltip("1,2,3,4...")]
        public string buttonText;
        [Tooltip("Alpha1,Alpha2...")]
        public KeyCode keyCode;
        [Tooltip("a Reference of the bound item")]
        /*[NonSerialized]*/ public Item item;
    };
    [Tooltip("All Quick buttons information")]
    public QuickItem[] hotbar;

    #region Canvas References and Variables
    [SerializeField]
    private Button[] buttons;
    [SerializeField]
    private GameObject[] buttonIcons;
    [SerializeField]
    private GameObject[] CooldownObjects;
    [SerializeField]
    private Text[] CooldownTexts;
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

    }

    private void Update()
    {
        //Redundant
        /*
        //run item cooldowns.
        for (int i = 0; i < hotbar.Length; i++)
        {
            if (hotbar[i].item.Timer > 0)
            {
                hotbar[i].item.Timer -= Time.deltaTime;
            }
        }
        */
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