using UnityEngine;
using Cinemachine;

/// <summary>
/// General code.
/// Currently has a few common bits of code relating to inventories
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Common Stuff for various game UI windows")]
    [Tooltip("is it currently on a display of sorts")]
    public static bool isDisplay = false;

    [SerializeField, Tooltip("To disable/enable player movements")]
    private ThirdPersonMovement playerMovement;

    [SerializeField, Tooltip("To change mouse position aim settings.")]
    private CinemachineFreeLook cineCam;
    
    /// <summary>
    /// Disable player movements, Enable Cursor, sets isDisplay as true.
    /// </summary>
    public void DisableControls(bool mouseAimOn)
    {
        //enable cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerMovement.enabled = false;

        if (mouseAimOn)
        {
            //mouse/screen aim control to 10% speed
            cineCam.m_XAxis.m_MaxSpeed = 60f;
            cineCam.m_YAxis.m_MaxSpeed = 0.4f;
        }
        else
        {
            Time.timeScale = 0f;
        }

        isDisplay = true;
    }

    /// <summary>
    /// Enable player movements, Disable Cursor, sets isDisplay as false.
    /// </summary>
    public void EnableControls()
    {
        //disable cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerMovement.enabled = true;

        //mouse/screen aim control to 100% speed
        Time.timeScale = 1f;
        cineCam.m_XAxis.m_MaxSpeed = 300;
        cineCam.m_YAxis.m_MaxSpeed = 2;

        isDisplay = false;
    }
}
