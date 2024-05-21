using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowing : MonoBehaviour
{
    public Transform playerTranform;
    // Start is called before the first frame update
    public float fixedDistance;
    public float height;
    public float heighFactor;
    public float rotationFactor;

    float startRotationAngle; // This camera's rotation angle around the y-axis
    float endRotationAngle; //The player's rotation angle around the y-aixs
    float finalRotationAngle; //The final smoothed out(interpolated) rotation angle of the camera around the y-axis

    float currentHeight;
    float targetHeight;

    // Update is called once per frame
    void LateUpdate()
    {
        //Set Camera Height
        currentHeight = transform.position.y;
        targetHeight = playerTranform.position.y + height;
        currentHeight = Mathf.Lerp(currentHeight, targetHeight, heighFactor * Time.deltaTime);

        //Set Camera Behind
        startRotationAngle = transform.eulerAngles.y;
        endRotationAngle = playerTranform.eulerAngles.y;
        finalRotationAngle = Mathf.LerpAngle(startRotationAngle, endRotationAngle, Time.deltaTime * rotationFactor);

        Quaternion finalRotation = Quaternion.Euler(0, finalRotationAngle, 0); //Convert angle value into actual rotation
        transform.position = playerTranform.position;
        transform.position -= finalRotation * Vector3.forward * fixedDistance; //Same as: new Vector3(sin(finalRotationEngle) * fixedDistance, 0, cos(finalRotationAngle) * fixedDistance)

        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
        transform.LookAt(playerTranform);
    }
}
