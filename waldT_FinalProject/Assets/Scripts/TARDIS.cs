using UnityEngine;
using System.Collections;

// Trevor Walden, IGME 202 Section 2, December 14, 2015
// This script has 2 classes: 1 for the TARDIS's movement and one for data about individual path segments on the TARDIS's path
// This first class contains all the variables and logic for the TARDIS's movement (complex path following, obstacle avoidance)

public class TARDIS : Vehicle
{
    public float pathRadius; // Radius and square of the path the TARDIS follows
    private PathSegment[] path; // Array holding all segments of the path
    private Vector3 futurePos; // Future position of the TARDIS
    public float futureDist; // Frames ahead the future position is calculated to
    public float seekWeight; // Weight for seeking the path
    private Vector3 steeringForce; // Force for steering
    public float avoidWeight = 100.0f; // Force for obstacle avoidance
    public float safeDistance = 100.0f; // Safe area and square for obstacle avoidance
    private float safeDistanceSq;
    private Vector3 target; // Closest normal on the path, seek target
    private float closestDist = 10000f; // Distance to the closest normal point
    private Vector3 normalPoint; // Used to hold normal points temporarily while finding the closest one
    private float normalDist; // Distance between futurePos and normalPoint
    /*private int lowCap; // Hold the indexes in path[] of the segments behind and in front of the current segment to speed up looping
    private int highCap;
    private int newHighCap; // Can't update the cap during the loop!*/

    override public void Start() // Call Inherited Start and then do our own and initialize the object
    {
        base.Start(); // Call parent's start
        autoRotate = false; // TARDIS has its own rotation method
        steeringForce = Vector3.zero; // Initialize the steering force
        safeDistanceSq = Mathf.Pow(safeDistance, 2f); // Calculate the square once and never do it again
        target = Vector3.zero;

        path = new PathSegment[7]; // Create the path by looping through the waypoints and grabbing their positions
        GameObject[] wp = new GameObject[7];
        for (int i = 0; i < 7; i++)
        {
            wp[i] = GameObject.Find("wp" + i); // Not very efficient, but I didn't see a better way

            if (i != 0) // Loop to create a segment between every 2 points
            {
                path[i - 1] = new PathSegment(wp[i - 1].transform.position, wp[i].transform.position);
            }
        }
        path[6] = new PathSegment(wp[6].transform.position, wp[0].transform.position);
        /*lowCap = 5; // Set the initial bounds (otherwise it won't move!)
        highCap = 2;
        newHighCap = 2; // Can't update the loop regulator in the loop!*/
	}

    protected override void CalcSteeringForces() // Calculate the forces necessary to steer the TARDIS along its path (path following)
    {
        steeringForce = Vector3.zero; // Reset the steering force

        futurePos = transform.position + (Vector3.Normalize(velocity) * (futureDist * Time.deltaTime)); // Calculate future position

        closestDist = 10000f; // Reset the normal distance to ensure closest normal is found
        /*for (int i = lowCap; i != highCap; i++) // I improved the loop speed/efficiency by only looping through relevant segments, but that's still several square roots*/
        for (int i = 0; i < 7; i++) // I couldn't figure out why the lowCap/highCap was causing Unity to freeze on the 5th waypoint, so I commented it out and hardcoded in the values
        {
            /*if (i > path.Length - 1) // Loop the path
            {
                i -= path.Length;
            }*/

            normalPoint = path[i].ClosestPoint(futurePos); // Get the closest point on the segment

            normalDist = Vector3.Magnitude(normalPoint - futurePos); // Get the distance to the normal point
            if (normalDist <= closestDist) // If the normal's distance is shorter than the current (or equal, indicating segment change),
            {
                closestDist = normalDist; // Then set up the variables to reflect the new normal point
                target = normalPoint + (path[i].UnitSegment * futureDist); // Seek a point further ahead on the path

                /*lowCap = i - 2; // Set the new bounds for looping
                if (lowCap < 0)
                {
                    lowCap += path.Length; // Adjust for looping path
                }
                newHighCap = i + 2; // Set the next high cap
                if (newHighCap > path.Length - 1)
                {
                    newHighCap -= path.Length; // Adjust for looping path
                }*/
            }
        }
        /*highCap = newHighCap; // Update the loop regulating variable*/

        if (closestDist > pathRadius)
        {
            steeringForce += seekWeight * Seek(target); // Seek the closest normal on the path if straying from path
        }

        foreach (GameObject obst in gm.Obstacles) // Avoid obstacles
        {
            steeringForce += avoidWeight * AvoidObstacle(obst, safeDistanceSq);
        }

        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce); // Limit the 1 steering force (ultimate force)
        ApplyForce(steeringForce); // Apply all forces to the acceleration as 1 force (ultimate force) in ApplyForce()

        transform.Rotate(transform.up, -90 * Time.deltaTime); // Make the TARDIS spin
    }
}

// I initially wrote this and the class above before incorporating the waypoint GameObjects, so it was easier to just leave this class in this script (rather than lots of GetComponents)
// This class contains data for individual segments of the TARDIS's path

public class PathSegment // Does not inherit from MonoBehaviour so the segments can be created as standalone class objects using the 'new' keyword
{
    private Vector3 startP; // Start and end points of the segment
    private Vector3 endP;
    private Vector3 segment; // Vector and normalized vector representing the segment
    private Vector3 unitSegment;
    public Vector3 UnitSegment
    {
        get { return unitSegment; }
    }
    private float segmentMag; // Magnitude of the segment
    private Vector3 ap; // The vector between start point and point for which the normal is being calculated
    private float segProj; // Holds the projection of a point onto the segment
    private GameManager gm; // Accessor for the GameManager script

    public PathSegment(Vector3 pStart, Vector3 pEnd) // Initialize the segment using passed in parameters
    {
        startP = pStart;
        endP = pEnd;
        segment = endP - startP; // Get the segment
        unitSegment = Vector3.Normalize(segment);
        segmentMag = segment.magnitude; // Calculate once to avoid extra square roots
        gm = GameObject.Find("GameManagerGO").GetComponent<GameManager>(); // Get access to the GameManager script
        
    }

    public Vector3 ClosestPoint(Vector3 p) // Determines the closest point on the segment to the passed in point p
    {
        if (gm.dbug) // Draw debug line
        {
            Debug.DrawLine(startP, endP, Color.green);
        }

        ap = p - startP; // Get the vector for projection onto the segment
        segProj = Vector3.Dot(ap, unitSegment); // Get the projection onto the segment

        if (segProj < 0 || segProj > segmentMag) // If the point is behind or past the segment,
        {
            return startP; // Then return the closest/furthest point on the segment (past=furthest, no going backwards plz)
        }

        return (startP + (unitSegment * segProj)); // Return the scalar projection of ap onto the segment (i.e. the "normal" point)
    }
}
