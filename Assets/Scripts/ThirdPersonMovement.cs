using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Third person movement
/// </summary>

public class ThirdPersonMovement : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cam;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float gravity = -9.81f;
    private float turnSmoothVelocity;
    private Vector3 playerVelocity;

    private bool isGrounded;
    private void FixedUpdate()
    {
        isGrounded = IsGrounded();
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        Movement();
    }

    private void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y; //finds the angle we need, gives us angle of stinck in relation to the camera
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime); //smooths out current to target angle movement over time, slows down and smooths angle change
            transform.rotation = Quaternion.Euler(0f, angle, 0f); //sets rotation to that angle

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward; //direction in relation to the forward vector

            float movementSpeed = player.playerStats.speed;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                movementSpeed = player.playerStats.sprintSpeed;
            }
            else if (Input.GetKey(KeyCode.C))
            {
                movementSpeed = player.playerStats.crouchSpeed;
            }
            controller.Move(moveDir * movementSpeed * Time.deltaTime);
        }
    }

    void Jump()
    {
        if (isGrounded && playerVelocity.y <0)
        {
            playerVelocity.y = 0f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded) //&& groundedPlayer
        {
            playerVelocity.y += Mathf.Sqrt(player.playerStats.jumpHeight * -3.0f * gravity);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + (-Vector3.up * (controller.bounds.extents.y + 0.1f - controller.bounds.extents.x)), controller.radius);
    }
}
