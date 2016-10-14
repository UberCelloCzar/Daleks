using UnityEngine;
using System.Collections;

//use the Generic system here to make use of a Flocker array later on
using System.Collections.Generic;

// Trevor Walden, IGME 202 Section 2, December 14, 2015
// This script contains operations and variables that all the scene's vehicles share

[RequireComponent(typeof(CharacterController))] // Make sure inheriting classes have the necessary components for the scripted operations

// The vehicle class is a parent class for all vehicle objects
abstract public class Vehicle : MonoBehaviour 
{
    CharacterController charControl; // Accessor for the Character Controller component

    protected Vector3 acceleration; // Create the variables for movement (v,a,vd,F)
    protected Vector3 velocity;
    public Vector3 Velocity // Property so that vehicles can make decisions based on each others' velocity
    {
        get { return velocity; }
    }
    protected Vector3 desired;
    protected Vector3 steer;
    private Vector3 vecToCenter; // Vector for distance in obstacle avoidance calculation

    public float maxSpeed; // Limiting variables, should be initialized in each vehicle child
    public float maxForce;
    public float radius;
    public float mass;
    protected bool autoRotate; // Variable to tell if vehicle should automatically rotate towards its direction of travel, set in child classes
    // No relevant gravitational force in the section of space the scene takes place in

    protected GameManager gm; // Accessor for the GameManager script
    abstract protected void CalcSteeringForces(); // Require vehicle children to implement a steering method


	virtual public void Start() // Initialize the basic variables for every vehicle
    {
        acceleration = Vector3.zero; // No initial movement
        velocity = transform.forward;
        charControl = GetComponent<CharacterController>(); // Get access to the CharacterController
        gm = GameObject.Find("GameManagerGO").GetComponent<GameManager>(); // Get access to the GameManager script
	}

	protected void Update() // Update movement variables based on forces, called once per frame
    {
        CalcSteeringForces(); // Get the overall acceleration needed to do what the vehicle wants

        velocity += acceleration * Time.deltaTime; // Apply the acceleration to the velocity, correcting for frame rate
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed); // Limit the velocity

        if(autoRotate && velocity != Vector3.zero) // This rotates the vehicle to face its new direction (if the vehicle should do this)
        {
            transform.forward = velocity.normalized;
        }

       charControl.Move(velocity * Time.deltaTime); // Move the character in the scene
       acceleration = Vector3.zero; // Reset acceleration when done for the frame
	}

    protected void ApplyForce(Vector3 steeringForce) // Takes in a force and applies it to the acceleration
    {
        acceleration += steeringForce/mass; // Apply the force, accounting for mass
    }

    protected Vector3 Seek(Vector3 targetPos) // Returns a basic seeking force towards the inputted target
    {
        desired = targetPos - transform.position; // Get the vector between there and here
        desired.Normalize();
        desired = desired * maxSpeed; // Scale the vector to maxSpeed
        steer = desired - velocity; // Take the desired velocity we just calculated and subtract the current velocity
        return steer; // Return this calculated steering force
    }

    protected Vector3 AvoidObstacle(GameObject ob, float safeSq) // Takes an obstacle and determines what force is needed to avoid it
    {
        desired = Vector3.zero; // Reset desired velocity
        
        float obRad = ob.GetComponent<ObstacleScript>().Radius; // Get radius from obstacle's script
        vecToCenter = ob.transform.position - transform.position; // Get vector from vehicle to obstacle
        if (vecToCenter.sqrMagnitude > safeSq) // If object is out of my safe zone, ignore it
        {
            return Vector3.zero;
        }
        if (Vector3.Dot(vecToCenter, transform.forward) < 0) // If object is behind me, ignore it
        {
            return Vector3.zero;
        }
        if (Mathf.Abs(Vector3.Dot(vecToCenter, transform.right)) > obRad + radius) // If object is not in my forward path, ignore it
        {
            return Vector3.zero;
        }

        // If we get this far, we will collide with an obstacle!
        
        if (Vector3.Dot(vecToCenter, transform.right) < 0) // Object on left, steer right
        {
            desired = transform.right * maxSpeed; 

            if (gm.dbug) // Debug line to see if the vehicle is avoiding to the right
            {
                Debug.DrawLine(transform.position, ob.transform.position, Color.red);
            }
        }
        else // Object on left, steer left
        {
            desired = transform.right * -maxSpeed;

            if (gm.dbug) // Debug line to see if the vehicle is avoiding to the left
            {
                Debug.DrawLine(transform.position, ob.transform.position, Color.green);
            }
        }

        desired -= velocity; // Calculate the change in acceleration needed to avoid the obstacle
        return desired; // Return the acceleration
    }
}
