using UnityEngine;

public class BackgroundAnim : MonoBehaviour
{
    #region Reference
    [Tooltip("Reference to Background Animator")]
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>(); //get reference
    }
    #endregion
    #region Animator Methods
    /// <summary>
    /// on click event for opening options menu.
    /// Sets options bool to true.
    /// In Main Menu, sets the BG green.
    /// In Pause Menu, increases the backdrop size to fit options menu UI.
    /// </summary>
    public void OptionsEnter()
    {
        anim.SetBool("Options", true);
    }
    /// <summary>
    /// on click event for closing options menu.
    /// Sets options bool to false.
    /// In Main Menu, sets the BG brown.
    /// In Pause Menu, shinks the backdrop size to orignal pause menu size.
    /// </summary>
    public void OptionsExit()
    {
        anim.SetBool("Options", false);
    }
    #endregion
}
