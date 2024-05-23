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
    BridgeSpawner bridgeSpawner;

    int coinCollected = 0;
    int coinCollectedBest;
    int distanceRun = 0;
    int distanceRunBest;
    // Player speed and movement-related variables
    public float playerStartSpeed = 10.0f;
    float playerSpeed;
    float gValue = 20.0f; // Gravity value
    float translationFactor = 10.0f; // Smoothens the turning of direction
    float jumpForce = 1.5f;
    float timer = 0;
    float distance = 0;
    // Additional variable for vertical velocity
    float verticalVelocity = 0f;
    bool canTurnRight = false;
    bool canTurnLeft = false;
    bool isDead = false;
    bool isMoving = false;

    Vector3 previousPosition;
    Vector3 lastPosition;


    // Start is called before the first frame update
    void Start()
    {
        playerSpeed = playerStartSpeed; // Initialize player speed
        characterController = GetComponent<CharacterController>(); // Get CharacterController component
        anim = GetComponent<Animator>(); // Get Animator component
        bridgeSpawner = GameObject.Find("BridgeManager").GetComponent<BridgeSpawner>();
        playerVector = Vector3.forward * playerSpeed * Time.deltaTime; // Initialize player movement vector
        playerDirection = enDirection.North; // Set initial direction to North
        playerNextDirection = enDirection.North; // Ensure next direction is also North
        AudioController.Instance.PlayMusic("TikiDrum");

        lastPosition = transform.position; // Initialize lastPosition
    }

    // Update is called once per frame
    void Update()
    {
        PlayerLogic(); // Call the logic function each frame
    }

    void PlayerLogic()
    {
        if (isDead)
            return;

        if (!characterController.enabled)
        {
            characterController.enabled = true;
        }

        isMoving = false; // Reset isMoving flag

        //timer += Time.deltaTime;
        //playerSpeed += 0.05f * Time.deltaTime; // Gradually increase player speed over time

        #region Horizontal Direction 
        // Handles player direction change based on key inputs 'G' and 'F'
        if (Input.GetKeyDown(KeyCode.E) && canTurnRight) // When 'G' key is pressed
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

            AudioController.Instance.PlaySFX("turn");
        }
        else if (Input.GetKeyDown(KeyCode.Q) && canTurnLeft) // When 'F' key is pressed
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

            AudioController.Instance.PlaySFX("turn");

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
                AudioController.Instance.PlaySFX("gruntJump", 0.4f);
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


        //Death Flag On if Fall into bottom
        if (transform.position.y < -1f)
        {
            isDead = true;
            AudioController.Instance.PlaySFX("scream", 2f);
            anim.SetTrigger("isTripping");
            SaveScore();
        }

        previousPosition = transform.position; // Store previous position before moving

        characterController.Move(playerVector); // Move the character controller

        lastPosition = transform.position; // Store previous position after moving

        // Calculate distance based on position change
        if (Vector3.Distance(previousPosition, transform.position) > 0.01f) // Check if player has moved
        {
            isMoving = true; // Set isMoving to true if player has moved
        }

        if (isMoving)
        {
            timer += Time.deltaTime; // Update timer only if player is moving
            playerSpeed += 0.05f * Time.deltaTime; // Gradually increase player speed over time
        }

        //Calculate Distrance Travelled
        distance = playerSpeed * timer;
        distanceRun = (int)distance;
    }

    void DoSliding()
    {
        // Adjust CharacterController for sliding
        characterController.height = 1f;
        characterController.center = new Vector3(0, 0.5f, 0);
        characterController.radius = 0;
        StartCoroutine(ReEnableCC()); // Reset CharacterController after sliding
        AudioController.Instance.PlaySFX("slide");
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

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "LCorner")
        {
            canTurnLeft = true;
        }
        else if (hit.gameObject.tag == "RCorner")
        {
            canTurnRight = true;
        }
        else
        {
            canTurnLeft = false;
            canTurnRight = false;
        }


        //Death Flag On if Hit Obstacle
        if (hit.gameObject.tag == "Obstacle")
        {
            isDead = true;
            AudioController.Instance.PlaySFX("splat", 0.4f);
            anim.SetTrigger("isTripping");
            SaveScore();
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), $"Coins: {coinCollected}");
        GUI.Label(new Rect(10, 40, 150, 20), $"Coins Collect Best: {PlayerPrefs.GetInt("highScoreC")}");
        GUI.Label(new Rect(10, 70, 100, 20), $"Distance: {distanceRun}");
        GUI.Label(new Rect(10, 100, 150, 20), $"Distance Run Best: {PlayerPrefs.GetInt("highScoreD")}");

        if (isDead)
        {
            if(GUI.Button(new Rect(0.4f * Screen.width, 0.6f * Screen.height, 0.2f * Screen.width, 0.1f* Screen.height), "RESPAWN"))
            {
                DeathEvent();
            }
        }

      
    }

    void DeathEvent()
    {
        characterController.enabled = false; // Disable the character controller
        isDead = true; // Set isDead to true to pause player logic

        // Reset player position and rotation
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Reset player direction and speed
        playerDirection = enDirection.North;
        playerNextDirection = enDirection.North;
        playerSpeed = playerStartSpeed;

        // Reset any residual velocities or movement vectors
        verticalVelocity = 0f;
        playerVector = Vector3.forward * playerSpeed * Time.deltaTime;

        // Clean the scene through the BridgeSpawner
        bridgeSpawner.CleanTheScene();

        //Reset Timer & score
        timer = 0;
        coinCollected = 0;

        // Trigger the spawn animation
        anim.SetTrigger("isSpawned");

        StartCoroutine(ReEnableCharacterController()); // Re-enable the character controller after a slight delay
    }

    IEnumerator ReEnableCharacterController()
    {
        yield return new WaitForSeconds(0.1f); // Wait for a short duration to ensure segments are initialized

        // Ensure position is reset correctly before re-enabling
        transform.position = Vector3.zero;
        playerVector = Vector3.forward * playerSpeed * Time.deltaTime; // Reset movement vector

        characterController.enabled = true; // Re-enable the character controller
        isDead = false; // Mark the player as not dead
    }

    void FootStepEventA()
    {
        AudioController.Instance.PlaySFX("footStep", 0.8f);
    }
    void FootStepEventB()
    {
        AudioController.Instance.PlaySFX("footStep", 0.8f);
    }
    void JumpLandEvent()
    {
        AudioController.Instance.PlaySFX("gruntJumpLand",2f);
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Coin")
        {
            Destroy(col.gameObject);
            AudioController.Instance.PlaySFX("coin");
            coinCollected += 1;
        }
    }

    void SaveScore()
    {
        if(coinCollected > coinCollectedBest)
        {
            coinCollectedBest = coinCollected;
            PlayerPrefs.SetInt("highScoreC", coinCollectedBest);
            PlayerPrefs.Save();
        }

        if (distanceRun > distanceRunBest)
        {
            distanceRunBest = distanceRun;
            PlayerPrefs.SetInt("highScoreD", distanceRunBest);
            PlayerPrefs.Save();
        }
    }
}
