using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Image progressBar;
    public Text progressBarText;
    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex); //while cur scene still running, will load this scene.

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); //will fill the bar as it never typically reaches full since at that point its loaded

            progressBar.fillAmount = progress;
            progressBarText.text = progress.ToString("P0"); //converts to string as P percentage and 0 as number of decimals. E.G. "17%"
            yield return null;
        }
    }
}
