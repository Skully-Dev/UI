using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnyKeyScreen : MonoBehaviour
{
    public GameObject anyKeyScreen;
    public GameObject mainMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        Event e = Event.current; //e is now the current GUI event

        if (e.isKey || e.isMouse) //if what happened is a key was pressed
        {
            //if any key or mouse is pressed
            anyKeyScreen.SetActive(false);
            mainMenu.SetActive(true);
        }
    }
}
