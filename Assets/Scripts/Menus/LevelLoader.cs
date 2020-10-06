using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference an Image with Image Type: Filled and Fill Method: Horizontal for visual loading progress")]
    private Image progressBar;
    [SerializeField]
    [Tooltip("Reference a Text to display load percentage as e.g. 35%")]
    private Text progressBarText;

    /// <summary>
    /// Calls stack to load level Asynchronously, what scene do you want to load?
    /// </summary>
    /// <param name="sceneIndex">The index of the scene in project settings that you want to load</param>
    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    /// <summary>
    /// A coroutine to load scene Asynchronously.
    /// </summary>
    /// <param name="sceneIndex">The index of the scene in project settings that you want to load</param>
    /// <returns></returns>
    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex); //while cur scene still running, will load this scene.

        while (!operation.isDone) //runs until scene is loaded.
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); //will fill the bar as it never typically reaches full since at that point its loaded

            progressBar.fillAmount = progress; // fills the bar proportionally to the load percentage
            progressBarText.text = progress.ToString("P0"); //converts to string as P percentage and 0 as number of decimals. E.G. "17%"
            yield return null;
        }
    }
}
