using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public GameManager gameManager;

    #region Variables
    [Header("References")]
    public bool showDialogue;
    //index for the current line of dialogue.
    public int currentLineIndex;

    //screen
    public Vector2 scr;

    [Header("NPC Name and Dialogue")]
    //name of the specific NPC talking
    public string npcName;
    //array for text of the dialogue
    public string[] dialogueText;
    #endregion

    protected virtual void OnGUI() //virtual allows us to override OnGUI
    {
        if (showDialogue)
        {
            gameManager.DisableControls(true);

            //set up our ratio for 16:9
            scr.x = Screen.width / 16;
            scr.y = Screen.height / 9;

            //The dialigue box takes up the whole bottom 3rd of the screen and displays the NPC's name and current dialogue line
            GUI.Box(new Rect(0, 6 * scr.y,
                             Screen.width, scr.y * 3),
                             npcName + " : " + dialogueText[currentLineIndex]);

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
            { //for quests instead have 1 accept, 1 reject
                EndDialogue();
            }
        }
    }

    protected virtual void EndDialogue()
    {
        if (GUI.Button(new Rect(15 * scr.x, 8.5f * scr.y,
                        scr.x, scr.y * 0.5f), "Bye"))
        {
            showDialogue = false;
            currentLineIndex = 0;

            gameManager.EnableControls();
        }
    }
}
