using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static bool isDialogue = false;

    public GameObject dialogueWindow;

    public TextMeshProUGUI dialogueText;

    public Button[] buttons;
    [NonSerialized]
    public TextMeshProUGUI[] buttonsText;

    // Start is called before the first frame update
    void Start()
    {
        buttonsText = new TextMeshProUGUI[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            buttonsText[i] = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}
