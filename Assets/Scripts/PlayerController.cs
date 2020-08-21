using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))] //When script is added as a component of a game object, this also adds a Component CharacterController if not already added
public class PlayerController : MonoBehaviour
{
    //mouse input
    ////rotate camera up and down
    ////rotate the character left and right
    //Keyboard input
    ////move the character
    #region Variables
    public CharacterController controller; ///set in Start, probs dont need to be public

    public float speed = 12f; //moevement speed of player
    public float gravity = -9.81f;
    private Vector3 velocity;

    public float mouseSensitivity = 100f; //A public variable to adjust sensitivity in the inspector 
    private float xRotation = 0f;

    private Camera cam;//cam instead of camera, because it clashed with another thing in Unity called camera
    #endregion

    void Start()//Executed when script instance is enabled, before any Update
    {
        controller = GetComponent<CharacterController>(); //reference Character Controller of the Game Object the script is attached to.

        cam = Camera.main; //refernce the main camera, its child to player

        Cursor.lockState = CursorLockMode.Locked; //Locks cursor position to middle of screen
    }

    //FixedUpdate is called on  a regular timeline and has the SAME TIME BETWEEN CALLS, immediately after FixedUpdate any necessary physics calculations are made, anything applied to a RigidBody (i.e. physics related functions) should be executed in FixedUpdate, runs every 0.2 seconds
    void FixedUpdate()
    {
        MouseLook();
        Move();
        //calling the below methods
    }

    private void MouseLook() ///Ask ANDREW about
    {
        //Mouse x/y (-1 to 1) multiplied by mouse sensitivity and relative to frame rate.
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY; //Cameras rotation x is up/down and inverted, so this converts to those values.
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);//clamps rotation values to between -90 and 90 (straight down/up)
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);//Since local rotation is a Quarternion, applies up/down rotation by Euler
        transform.Rotate(Vector3.up * mouseX);//GameObject y rotation (look left/right) is y * MouseX (left/right), applies left/right rotation
    }

    void Move()
    {
        //x = -1 to 1
        float x = Input.GetAxis("Horizontal");
        //z = -1 to 1
        float z = Input.GetAxis("Vertical");


        //we want to move in this direction, right/red is x axis, blue/forward is z axis
        Vector3 move = (transform.right * x) + (transform.forward * z);

        velocity.y += gravity * Time.deltaTime;//player up is bound by gravity relative to frame rate

        controller.Move((velocity + move) * speed * Time.deltaTime);//velocity makes player move direction movement physics based increased by speed and relative to frame rate
    }
}
