using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasFaceCamera : MonoBehaviour
{
    public Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform; //camera name clashed with another named thing in Unity, so named cam
    }

    // Update is called once per frame
    void LateUpdate() //since we want to make it face current camera position, late update runs after normal update
    {
        transform.LookAt(transform.position + cam.forward);
    }
}