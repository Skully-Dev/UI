using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuarterHearts : MonoBehaviour
{
    [SerializeField] private Image[] heartSlots;
    [SerializeField] private Sprite[] hearts;

    private float currentHealth;
    private float maximumHealth;
    private float healthPerSection;

    #region UpdateHearts
    public void UpdateHearts(float curHealth, float maxHealth)
    {
        currentHealth = curHealth;
        maximumHealth = maxHealth;

        healthPerSection = maximumHealth / (heartSlots.Length * 4);
    }
    #endregion

    private void Update()
    {
        int heartSlotIndex = 0;
        
        foreach (Image image in heartSlots)
        {
            //if current health exceeds current heart's value
            if (currentHealth >= (healthPerSection * 4) + healthPerSection * 4 * heartSlotIndex)
            {
                //set hear to 4/4
                heartSlots[heartSlotIndex].sprite = hearts[0];
            }
            else if (currentHealth >= ((healthPerSection * 3)) + healthPerSection * 4 * heartSlotIndex)
            {
                //set heart to 3/4
                heartSlots[heartSlotIndex].sprite = hearts[1];
            }
            else if (currentHealth >= ((healthPerSection * 2)) + healthPerSection * 4 * heartSlotIndex)
            {
                //set heart to 2/4
                heartSlots[heartSlotIndex].sprite = hearts[2];
            }
            else if (currentHealth >= ((healthPerSection * 1)) + healthPerSection * 4 * heartSlotIndex)
            {
                //set heart to 1/4
                heartSlots[heartSlotIndex].sprite = hearts[3];
            }
            else
            {
                //set heart to 0/4
                heartSlots[heartSlotIndex].sprite = hearts[4];
            }
            heartSlotIndex++;
        }
    }
}
