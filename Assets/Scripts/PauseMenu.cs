using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    //Don't tattach to the pause menu UI itself, this would end up with the script being inactive and no longer able to detect user input

    public bool gameIsPaused = false;
    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;//time resumes normal game time
        gameIsPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;//Make game time stand still
        gameIsPaused = true;
        Cursor.lockState = CursorLockMode.None; //unlocks the cursor to use
    }

    public void QuitGame() //called with exit button
    {
        Debug.Log("Quitting Game");

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode(); //If using unity, run this code, exit play mode
#endif
        Application.Quit(); //if not unity editor, run the quit application/exe
                            //quit exe game, i.e. once published, this will quit
    }
}