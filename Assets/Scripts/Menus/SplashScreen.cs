using UnityEngine;

public class SplashScreen : MonoBehaviour //attached to Manual Splash Screen Game Object
{
    [SerializeField]
    [Tooltip("How long you want the manual splash screen to be visable")]
    private float disableTime = 0.5f;

    // Update is called once per frame
    void Update()
    {
        if(Time.time > disableTime) //once its been that long.
        {
            gameObject.SetActive(false); //disable splash screen.
        }
    }
}
//If we pass 0.5 seconds it will disable