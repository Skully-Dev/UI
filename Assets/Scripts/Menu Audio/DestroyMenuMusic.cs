using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMenuMusic : MonoBehaviour
{
    void Start()
    {
        Destroy(GameObject.Find("MenuMusic"));

    }
}
