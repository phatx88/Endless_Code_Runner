using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Enumeration for player direction
    enum enDirection { North, East, West };

    // Components and player properties
    CharacterController characterController;
    Vector3 playerVector; // Player movement vector
    enDirection playerDirection = enDirection.North; // Default direction (North)
    enDirection playerNextDirection = enDirection.North; // Next direction for the player
    Animator anim;

    // Player speed and movement-related variables
    public float playerStartSpeed = 10.0f;
    float playerSpeed;
    float gValue = 20.0f; // Gravity value
    float translationFactor = 10.0f; // Smoothens the turning of direction
    float jumpForce = 1.5f;

    // Additional variable for vertical velocity
    float verticalVelocity = 0f;

    // Start is called before the first frame update
    void Start()
    {
        playerSpeed = playerStartSpeed; // Initialize player speed
        characterController = GetComponent<CharacterController>(); // Get CharacterController component
        anim = GetComponent<Animator>(); // Get Animator component
        playerVector = new Vector3(0, 0, 1) * playerSpeed * Time.deltaTime; // Initialize player movement vector
    }

    // Update is called once per frame
    void Update()
    {
        PlayerLogic(); // Call the logic function each frame
    }

    void PlayerLogic()
    {
        playerSpeed += 0.005f * Time.deltaTime; // Gradually increase player speed over time

        #region Horizontal Direction 
        // Handles player direction change based on key inputs 'G' and 'F'
        if (Input.GetKeyDown(KeyCode.E)) // When 'G' key is pressed
        {
            switch (playerDirection)
            {
                case enDirection.North:
                    playerNextDirection = enDirection.East;
                    transform.rotation = Quaternion.Euler(0, 90, 0);  // Rotate player to face East
                    break;
                case enDirection.West:
                    playerNextDirection = enDirection.North;
                    transform.rotation = Quaternion.Euler(0, 0, 0);  // Rotate player to face North
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q)) // When 'F' key is pressed
        {
            switch (playerDirection)
            {
                case enDirection.North:
                    playerNextDirection = enDirection.West;
                    transform.rotation = Quaternion.Euler(0, -90, 0);  // Rotate player to face West
                    break;
                case enDirection.East:
                    playerNextDirection = enDirection.North;
                    transform.rotation = Quaternion.Euler(0, 0, 0);  // Rotate player to face North
                    break;
            }
        }

        playerDirection = playerNextDirection; // Update player direction

        // Update player movement vector based on direction
        if (playerDirection == enDirection.North)
        {
            playerVector = Vector3.forward * playerSpeed * Time.deltaTime;
        }
        else if (playerDirection == enDirection.East)
        {
            playerVector = Vector3.right * playerSpeed * Time.deltaTime;
        }
        else if (playerDirection == enDirection.West)
        {
            playerVector = Vector3.left * playerSpeed * Time.deltaTime;
        }
        #endregion

        // Set horizontal movement based on direction and input
        switch (playerDirection)
        {
            case enDirection.North:
                playerVector.x = Input.GetAxisRaw("Horizontal") * translationFactor * Time.deltaTime;
                break;
            case enDirection.East:
                playerVector.z = -Input.GetAxisRaw("Horizontal") * translationFactor * Time.deltaTime;
                break;
            case enDirection.West:
                playerVector.z = Input.GetAxisRaw("Horizontal") * translationFactor * Time.deltaTime;
                break;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            DoSliding(); // Trigger sliding when the down arrow key is pressed
        }
        #region vertical movement logic 
        //// Vertical movement logic
        //if (characterController.isGrounded)
        //{
        //    playerVector.y = -0.2f; // Ensure player stays grounded
        //}
        //else
        //{
        //    playerVector.y -= gValue * Time.deltaTime; // Apply gravity if not grounded
        //}

        //// Jumping logic
        //if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
        //{
        //    anim.SetTrigger("isJumping"); // Trigger jump animation
        //    playerVector.y = Mathf.Sqrt(jumpForce * gValue); // Calculate jump force
        //}
        #endregion

        #region Adding Vertical Velocity to vertical movement logic 
        

        // Vertical movement logic
        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0)
            {
                verticalVelocity = -0.2f; // Ensure player stays grounded
            }

            // Jumping logic
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetTrigger("isJumping"); // Trigger jump animation
                verticalVelocity = Mathf.Sqrt(jumpForce * gValue); // Calculate jump force
            }
        }
        else
        {
            verticalVelocity -= gValue * Time.deltaTime; // Apply gravity if not grounded
        }

        // Apply vertical velocity to player vector
        playerVector.y = verticalVelocity * Time.deltaTime;
        #endregion


        characterController.Move(playerVector); // Move the character controller
    }

    void DoSliding()
    {
        // Adjust CharacterController for sliding
        characterController.height = 1f;
        characterController.center = new Vector3(0, 0.5f, 0);
        characterController.radius = 0;
        StartCoroutine(ReEnableCC()); // Reset CharacterController after sliding
        anim.SetTrigger("isSliding"); // Trigger sliding animation
    }

    IEnumerator ReEnableCC()
    {
        yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds

        // Reset CharacterController dimensions
        characterController.height = 2f;
        characterController.center = new Vector3(0, 1f, 0);
        characterController.radius = 0.2f;
    }
}
