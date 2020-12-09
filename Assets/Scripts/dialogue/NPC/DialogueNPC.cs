using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNPC : NPC
{
    [SerializeField]
    private Dialogue dialogue;

    private void Start()
    {
        if (dialogue == null)
        {
            Debug.LogError("Don't forget to include Dialoge");
        }
    }

    public override void Interact()
    {
        // set up dialogue
        dialogue.showDialogue = true;
        dialogue.ShowDialogue();
    }
}
