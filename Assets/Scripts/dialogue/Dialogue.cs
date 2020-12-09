using UnityEngine;

public class Dialogue : MonoBehaviour
{
    protected GameManager gameManager;

    #region Variables
    [Header("References")]
    [System.NonSerialized]
    public bool showDialogue;
    //index for the current line of dialogue.
    protected int currentLineIndex;

    //screen
    protected Vector2 scr;

    [Header("NPC Name and Dialogue")]
    //used to get name of the specific NPC talking
    public NPC npc;

    //array for text of the dialogue
    [SerializeField]
    protected string[] dialogueText;
    #endregion

    protected virtual void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("There is no game manager in scene");
        }
    }

    protected virtual void OnGUI() //virtual allows us to override OnGUI in derived classes
    {
        if (showDialogue)
        {
            gameManager.DisableControls(false);
            //gameManager.DisableControls(true);

            //set up our ratio for 16:9
            scr.x = Screen.width / 16;
            scr.y = Screen.height / 9;

            //The dialigue box takes up the whole bottom 3rd of the screen and displays the NPC's name and current dialogue line
            GUI.Box(new Rect(0, 6 * scr.y,
                             Screen.width, scr.y * 3),
                             npc.name + " : " + dialogueText[currentLineIndex]);

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
