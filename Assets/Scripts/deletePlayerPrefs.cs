using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deletePlayerPrefs : MonoBehaviour
{
    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();

    }
}
