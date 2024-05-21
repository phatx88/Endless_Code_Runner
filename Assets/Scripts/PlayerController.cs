using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum enDirection { North, East, West}; //a numerator that can enumerate player direction
    
    CharacterController characterController;
    Vector3 playerVector; //Player direction 
    enDirection playerDirection = enDirection.North; //set default direction (north = 0)
    enDirection playerNextDirection = enDirection.North;

    public float playerStartSpeed = 10.0f;
    float playerSpeed;
    float gValue = 10.0f; //set gravity value
    float translationFactor = 10.0f; // Smoothen the turning of direction

    // Start is called before the first frame update
    void Start()
    {
        playerSpeed = playerStartSpeed;
        characterController = GetComponent<CharacterController>();
        playerVector = new Vector3(0 , 0, 1) * playerSpeed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()   
    {
        PlayerLogic();
    }

    void PlayerLogic()
    {
        playerSpeed += 0.005f * Time.deltaTime; // increasing playerspeed as player traverse through the game

        #region Horizontal Direction 
        //Set Logic to Horizontal direction the player when press Key 'G'  or 'F' on keyboard, left and right direction
        //Making sure that Player will not turning more than 180 degree toward the South diraction (make sure only move N, E and W)
        //For Example if press G first time will turn right, press again will turn North.
        if (Input.GetKeyDown(KeyCode.G)) //check first condition if press G key
        {
            switch (playerDirection)
            {
                case enDirection.North:
                    playerNextDirection = enDirection.East;
                    transform.rotation = Quaternion.Euler(0, 90, 0);  //If player facing North and turning East, player move 90 degree right
                    break;
                case enDirection.West:
                    playerNextDirection = enDirection.North;
                    transform.rotation = Quaternion.Euler(0, 0, 0);  //If player facing West and turning North, player move straight
                    break;
            }
        }
        else if (Input.GetKeyDown(KeyCode.F)) //check second condition if press F key
        {
            switch (playerDirection)
            {
                case enDirection.North:
                    playerNextDirection = enDirection.West;
                    transform.rotation = Quaternion.Euler(0, -90, 0);  
                    break;
                case enDirection.East:
                    playerNextDirection = enDirection.North;
                    transform.rotation = Quaternion.Euler(0, 0, 0); 
                    break;
            }
        }

        playerDirection = playerNextDirection;

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

        //Set Logic Horizontal movement of the player
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
        //Set Logic for Vertical Movement of the Player
        if (characterController.isGrounded)
        {
            playerVector.y = -0.2f; //push against the ground
        }
        else playerVector.y = playerVector.y - (gValue * Time.deltaTime); //if not touch ground fall to gravity
        characterController.Move(playerVector);

    }
}
