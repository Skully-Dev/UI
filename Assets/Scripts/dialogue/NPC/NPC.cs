using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//base class for NPCs
public abstract class NPC : MonoBehaviour
{   
    [SerializeField] protected string name;

    public abstract void Interact();

}
