using UnityEngine;

public class SplashScreen : MonoBehaviour //attached to Manual Splash Screen Game Object
{
    public float disableTime = 0.5f; //a float variable named disableTime, default value of 0.5f, settable from inspector

    // Update is called once per frame
    void Update()
    {
        if(Time.time > disableTime)
        {
            gameObject.SetActive(false);
        }
    }
}
//If we pass 0.5 seconds it will disable