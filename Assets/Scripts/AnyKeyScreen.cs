using UnityEngine;

public class AnyKeyScreen : MonoBehaviour
{
    public GameObject anyKeyScreen;
    public GameObject mainMenu;

    private void OnGUI()
    {
        Event e = Event.current; //e is now the current GUI event

        if (e.isKey || e.isMouse) //if what happened is a key was pressed or mouse was clicked
        {
            //if any key or mouse is pressed
            anyKeyScreen.SetActive(false);
            mainMenu.SetActive(true);
        }
    }
}
