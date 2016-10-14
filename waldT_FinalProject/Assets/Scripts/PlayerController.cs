using UnityEngine;
using System.Collections;

// Trevor Walden, IGME 202 Section 2, December 14, 2015
// This script has the code for the free-moving camera that can be used to view the scene
public class PlayerController : MonoBehaviour 
{
    public float moveSpeed = 10.0f; // Movement and turn speeds for the camera SET IN THE INSPECTOR!
    public float turnSpeed = 5.0f;
    private float rotHoriz; // Floats for the rotation of the camera vertically and horizontally
    private float rotVert;
	
	void Update () // Turn and move the camera, called once per frame
    {
        float fwd = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime; // Move forward in faced direction and strafe (WASD)
        float side = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;

        transform.Translate(side, 0, fwd); // Move in the indicated direction

        float turn = Input.GetAxis("Mouse X") * turnSpeed;
        float turnV = Input.GetAxis("Mouse Y") * -turnSpeed; // Rotate vertically as well
        transform.Rotate(turnV, turn, 0);
	}
}
