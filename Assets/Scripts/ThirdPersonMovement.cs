using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("Reference to player")] private Player player;
    [SerializeField, Tooltip("Reference to Character Controller on Player")] private CharacterController controller;
    [SerializeField, Tooltip("Reference to camera")] private Transform cam;
    [Header("Variables")]
    [SerializeField, Tooltip("How fast smoothing attempts to meet target")] private float turnSmoothTime = 0.1f;
    [SerializeField, Tooltip("Simulate gravity for player velocity")] private float gravity = -9.81f;
    [Tooltip("The current velocity of the function, auto updated")]
    private float turnSmoothVelocity;
    [Tooltip("Completely taken care of in script, this is the players velocity")]
    private Vector3 playerVelocity;

    [Tooltip("Is the player currently grounded. bool var")]
    private bool isGrounded;

    private void FixedUpdate()
    {
        //update isGrounded value every physics update.
        isGrounded = IsGrounded();
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        Movement();
    }


    /// <summary>
    /// Player directional movement.
    /// </summary>
    private void Movement()
    {
        #region Input Axis Horizontal and Vertical
        //float horizontal = Input.GetAxisRaw("Horizontal");
        //float vertical = Input.GetAxisRaw("Vertical");
        #endregion

        #region Input Keybinds Horizontal and Vertical
        //Direction based on user input of KeyBinds
        float vertical = 0;
        float horizontal = 0;
        //instead of Input.GetKeyDown(KeyCode.W), we use the dictonary 'keys' with value "Up" to call what KeyCode is required to be pressed for said action
        if (Input.GetKey(KeyBindScript.keys["Up"]))//default (Keycode.W)
        {
            vertical = 1;
        }
        if (Input.GetKey(KeyBindScript.keys["Down"]))//default (Keycode.S)
        {
            vertical += -1;
        }

        if (Input.GetKey(KeyBindScript.keys["Left"]))//default (Keycode.A)
        {
            horizontal = -1;
        }
        if (Input.GetKey(KeyBindScript.keys["Right"]))//default (Keycode.D)
        {
            horizontal += 1;
        }

        #region Controller input
        if (vertical == 0)
        {
            if ((Input.GetAxis("Vertical") != 0))
            {
                vertical = Input.GetAxis("Vertical");
            }
        }
        if (horizontal == 0)
        {
            if ((Input.GetAxis("Horizontal") != 0))
            {
                horizontal = Input.GetAxis("Horizontal");
            }
        }
        #endregion
        #endregion

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized; //brings the combined direction value to a magnitude/length of 1.

        if (direction.magnitude >= 0.1f) //if any directional movement
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y; //finds the angle we need, gives us angle of stick in relation to the camera
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); //smooths out current to target angle movement over time, slows down and smooths angle change
            transform.rotation = Quaternion.Euler(0f, angle, 0f); //sets rotation to that angle

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward; //direction in relation to the forward vector

            float movementSpeed = player.playerStats.stats.speed;
            if (player.playerStats.stats.currentStamina > 0 && (Input.GetKey(KeyBindScript.keys["Sprint"]) || Input.GetButton("Sprint") /*LStick button*/)/*Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)*/)
            {
                player.disableStaminaRegenTime = Time.time;
                player.playerStats.CurrrentStamina -= player.StaminaDegen * Time.deltaTime; //set property to clamp and update ui etc.
                movementSpeed = player.playerStats.stats.sprintSpeed;
            }
            else if (Input.GetKey(KeyBindScript.keys["Crouch"]) || Input.GetButton("Crouch")/*B button*/)
            {
                movementSpeed = player.playerStats.stats.crouchSpeed;
            }
            controller.Move(moveDir * movementSpeed * Time.deltaTime); //applies the movement.
        }
    }

    /// <summary>
    /// Applies gravity and if player jumps, applies jump too.
    /// </summary>
    void Jump()
    {
        if (isGrounded && playerVelocity.y <0)
        {
            playerVelocity.y = 0f;
        }

        if ((Input.GetKeyDown(KeyBindScript.keys["Jump"]) || Input.GetButtonDown("Jump")/*Y button*/) && isGrounded) //if jump is pressed and character is grounded
        {
            playerVelocity.y += Mathf.Sqrt(player.playerStats.stats.jumpHeight * -3.0f * gravity); //calculate jump
        }

        playerVelocity.y += gravity * Time.deltaTime; //calculate gravity
        controller.Move(playerVelocity * Time.deltaTime); //applies the movement.
    }


    /// <summary>
    /// Determine if player is touching ground.
    /// </summary>
    /// <returns></returns>
    bool IsGrounded()
    {
        Debug.DrawRay(transform.position, -Vector3.up * ((controller.height * 0.5f) * 1.1f), Color.red);


        //Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8; //gets the value of 1 in binary and shifts that value across 8 times

        //This would cast rays only against colliders in layer 8
        //but instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask
        layerMask = ~layerMask;

        RaycastHit hit;

        //we are ignoring layer 8 (the player layer)
        /*if (Physics.Raycast(transform.position, -Vector3.up, out hit, (controller.height * 0.5f) * 1.1f, layerMask))//ray cast from current position to negative y
        {
            return true; //if we hit anything we know were on the ground
        }*/

        if (Physics.SphereCast(transform.position, controller.radius, -Vector3.up, out hit, controller.bounds.extents.y + 0.1f - controller.bounds.extents.x, layerMask))
        {
            //can use hit to get information too, for example, when jumping, spawn dirt particles at point of grounded.
            ////Vector3 spawnZone = hit.point;
            ////Instantiate(spawnable, spawnZone, Quaternion.identity);

            return true; //if we hit anything we know were on the ground
        }
        return false;
    }

    /// <summary>
    /// visualize the IsGrounded SphereCast
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + (-Vector3.up * (controller.bounds.extents.y + 0.1f - controller.bounds.extents.x)), controller.radius);
    }
}
