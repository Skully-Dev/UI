using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteButton : MonoBehaviour
{
    [SerializeField]
    private AudioSource buttonSound;
    [SerializeField]
    private GameObject wave;

    public void ToggleMute()
    {
        if (wave.activeSelf)
        {
            buttonSound.volume = 0f;
            wave.SetActive(false);
        }
        else
        {
            buttonSound.volume = 1f;
            buttonSound.Play();
            wave.SetActive(true);
        }
    }
}
