using UnityEngine;
using System.Collections;

// Trevor Walden, IGME 202 Section 2, December 14, 2015
// This script contains variables and logic for the lead Dalek
// This Dalek is the leader of the flock (leader following) and seeks the TARDIS 
public class Leader : Vehicle
{
    public float seekWeight; // Weight for seeking the TARDIS
    private Vector3 steeringForce; // Force for steering
    public float avoidWeight = 100.0f; // Force for obstacle avoidance
    public float safeDistance = 100.0f; // Safe area and square for obstacle avoidance
    private float safeDistanceSq;
    private Vector3 followPoint; // Point behind the leader that flockers follow
    public Vector3 FollowPoint
    {
        get { return followPoint; }
    }
    public float followDist; // Distance behind the leader the point is at

    override public void Start() // Call Inherited Start and then do our own and initialize the object
    {
        base.Start(); // Call parent's start
        autoRotate = true; // Always face direction of travel
        steeringForce = Vector3.zero; // Initialize the steering force
        safeDistanceSq = Mathf.Pow(safeDistance, 2f); // Calculate the square once and never do it again
	}

    protected override void CalcSteeringForces() // Calculate the forces necessary to steer the Dalek to its desired destination (seek TARDIS, avoid obstacles)
    {
        steeringForce = Vector3.zero; // Reset the steering force
        steeringForce += seekWeight * Seek(gm.tardis.transform.position); // Seek the TARDIS

        foreach (GameObject obst in gm.Obstacles) // Avoid obstacles
        {
            steeringForce += avoidWeight * AvoidObstacle(obst, safeDistanceSq);
        }

        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce); // Limit the 1 steering force (ultimate force)
        ApplyForce(steeringForce); // Apply all forces to the acceleration as 1 force (ultimate force) in ApplyForce()

        followPoint = (followDist * Vector3.Normalize(-velocity)) + transform.position; // Get the point behind the leader for the flockers to follow
    }
}
