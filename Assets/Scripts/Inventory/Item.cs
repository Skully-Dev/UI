using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Food,
    Weapon,
    Apparel,
    Crafting,
    Ingredients,
    Potions,
    Scrolls,
    Quest,
    Money
}

/// <summary>
/// Determining the core requirements of an item
/// </summary>
[System.Serializable]//debug
public class Item
{
    //serialized for debug
    #region Private Variables
    private int id;
    [SerializeField]private string name;
    [SerializeField]private string description;
    [SerializeField]private int value;
    [SerializeField]private int amount;
    [SerializeField]private Texture2D icon;
    [SerializeField]private GameObject mesh;
    [SerializeField]private ItemType type;
    [SerializeField] private int damage;// only ever need one of the 3, so could be replaced with effect amount and apply based on item type
    [SerializeField] private int armour;// only ever need one of the 3, so could be replaced with effect amount and apply based on item type
    [SerializeField] private int heal;// only ever need one of the 3, so could be replaced with effect amount and apply based on item type
    [SerializeField] private int stamina;
    [SerializeField] private int mana;
    [SerializeField] private int effect; //USING FOR CUSTOM EFFECTS
    #endregion

    #region Public Properties
    public int ID
    {
        get { return id; }
        set { id = value; }
    }
    public string Name
    {
        get { return name; }
        set { name = value; }
    }
    public string Description
    {
        get { return description; }
        set { description = value; }
    }
    public int Value
    {
        get { return value; }
        set { this.value = value; }
    }
    public int Amount
    {
        get { return amount; }
        set { amount = value; }
    }
    public Texture2D Icon
    {
        get { return icon; }
        set { icon = value; }
    }
    public GameObject Mesh
    {
        get { return mesh; }
        set { mesh = value; }
    }
    public ItemType Type
    {
        get { return type; }
        set { type = value; }
    }
    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }
    public int Armour
    {
        get { return armour; }
        set { armour = value; }
    }
    public int Heal
    {
        get { return heal; }
        set { heal = value; }
    }
    public int Effect
    {
        get { return effect; }
        set { effect = value; }
    }
    public int Stamina
    {
        get { return stamina; }
        set { stamina = value; }
    }
    public int Mana
    {
        get { return mana; }
        set { mana = value; }
    }
    #endregion

    #region Contructors
    public Item()
    {

    }

    public Item(Item copyItem, int copyAmount)
    {
        name = copyItem.Name;
        description = copyItem.Description;
        value = copyItem.value;
        amount = copyAmount;
        icon = copyItem.Icon;
        mesh = copyItem.Mesh; //as they are prefabs, this is fine, but if it was an item in scene, instantiate would be a better solution
        type = copyItem.Type;
        damage = copyItem.Damage;
        armour = copyItem.Armour;
        heal = copyItem.Heal;
        effect = copyItem.Effect;
        stamina = copyItem.Mana;
        mana = copyItem.Stamina;
    }
    #endregion
}
