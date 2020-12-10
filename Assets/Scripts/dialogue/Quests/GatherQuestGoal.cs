using UnityEngine;

[System.Serializable]
public class GatherQuestGoal : QuestGoal
{
    #region Gather Variables
    private Inventory playerInventory;

    public string itemName;
    public int requiredAmount;
    #endregion

    private void Start()
    {
        playerInventory = (Inventory)GameObject.FindObjectOfType<Inventory>();
        if (playerInventory == null)
        {
            Debug.LogError("There is no pllayer with an inventory in the scene");
        }
    }

    public override bool isCompleted() //if more than 1 inventory, have parameter of Inventory to pass in the inventory
    {
        Item item = playerInventory.FindItem(itemName);
        if (item == null)
        {
            return false;
        }

        if (item.Amount >= requiredAmount)
        {
            return true;
        }

        return false;
    }
}
