using UnityEngine;

public class AnyKeyScreen : MonoBehaviour
{
    #region References
    [Header("References")]
    [SerializeField]
    [Tooltip("Reference the 'Any Key Screen' game objects")]
    private GameObject anyKeyScreen;
    [SerializeField]
    [Tooltip("Reference any 'Main Menu' Game Objects")]
    private GameObject[] mainMenuItems;
    #endregion
    #region Any Key Event
    private void OnGUI()
    {
        Event e = Event.current; //e is now the current GUI event

        if (e.isKey || e.isMouse) //if what happened is a key was pressed or mouse was clicked
        {
            anyKeyScreen.SetActive(false); //deactivate AnyKey reference
            foreach (GameObject item in mainMenuItems) //goes through the array of Main Menu references
            {
                item.SetActive(true); //activate all Main Menu references
            }
        }
    }
    #endregion
}
