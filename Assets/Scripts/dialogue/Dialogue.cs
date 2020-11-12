using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    #region Variables
    [Header("References")]
    public bool showDialogue;
    //index for the current line of dialogue.
    public int currentLineIndex;

    //To disable player movement during dialogue
    public ThirdPersonMovement playerMovement;

    //screen
    public Vector2 scr;

    [Header("NPC Name and Dialogue")]
    //name of the specific NPC talking
    public string name;
    //array for text of the dialogue
    public string[] dialogueText;
    #endregion

    private void OnGUI()
    {
        if (showDialogue)
        {
            playerMovement.enabled = false;
            //also maybe mouse/screen aim control enable/disable


            //set up our ratio for 16:9
            scr.x = Screen.width / 16;
            scr.y = Screen.height / 9;

            //The dialigue box takes up the whole bottom 3rd of the screen and displays the NPC's name and current dialogue line
            GUI.Box(new Rect(0, 6 * scr.y,
                             Screen.width, scr.y * 3),
                             name + " : " + dialogueText[currentLineIndex]);

            //if not at the end of the dialogue
            if (currentLineIndex < dialogueText.Length - 1)
            {
                //next button allows us to skip forward to the next line of dialogue
                if (GUI.Button(new Rect(15 * scr.x, 8.5f * scr.y,
                                        scr.x, scr.y * 0.5f), "Next"))
                {
                    currentLineIndex++;
                }
            }
            else
            {
                if (GUI.Button(new Rect(15 * scr.x, 8.5f * scr.y,
                                        scr.x, scr.y * 0.5f), "Bye"))
                {
                    showDialogue = false;
                    currentLineIndex = 0;

                    playerMovement.enabled = true;
                    //also maybe mouse/screen aim control enable/disable

                    //enable and disable
                    //Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }
    }
}
