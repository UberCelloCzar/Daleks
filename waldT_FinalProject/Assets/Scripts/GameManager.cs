
using UnityEngine;
using System.Collections;

//add using System.Collections.Generic; to use the generic array format
using System.Collections.Generic;

// Trevor Walden, IGME 202 Section 2, December 14, 2015
// This script manages overall variables and operations otherwise outside the scope of the vehicle script
// NOTE TO SELF: change random vehicle spawns to placed by making public arrays to be set in Inspector


public class GameManager : MonoBehaviour 
{
    public GameObject[] obstPrefabs; // Prefabs for spawning

    public GameObject tardis; // Seek target for the leader
    public GameObject leader; // Leader of the flock

    private GameObject[] obstacles; // Array and property for the array of asteroids
    public GameObject[] Obstacles
    {
        get { return obstacles; }
    }

    private GameObject centroid; // Vector and property for the position of the center point of the flock
    public GameObject Centroid
    {
        get { return centroid; }
    }

    private GameObject[] flock; // Variable and property for the array of daleks that makes up the flock
    public GameObject[] Flock
    {
        get { return flock; }
    }

    public bool dbug = false; // Variable for debug lines

    public Camera[] cameras; // Game cameras for use, one is free move, other is Centroid follow
    private int currentCameraIndex; // Which camera is being used
	
	void Start() // Initialize the scene
    {
        centroid = GameObject.FindGameObjectWithTag("Centroid"); // Get the centroid

        tardis = GameObject.FindGameObjectWithTag("TARDIS"); // Spawn game objects into the scene
        leader = GameObject.FindGameObjectWithTag("Leader"); // Get the Lead Dalek from the scene

        flock = GameObject.FindGameObjectsWithTag("Dalek"); // Get the Daleks from the scene. Order doesn't matter.
        
        for (int i = 0; i < 50; i++) // Create obstacles and place them in the obstacles array
        {
            Vector3 pos = new Vector3(Random.Range(-1200, 1200), Random.Range(100, 2400), Random.Range(-1200, 1200)); // Random position
            Quaternion rot = Quaternion.Euler(new Vector3(Random.Range(0,360), Random.Range(0, 360), Random.Range(0,360))); // Random Rotation
            GameObject.Instantiate(obstPrefabs[Random.Range(0,2)], pos, rot); // Also, randomize prefab (of the 3)
        }
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        currentCameraIndex = 0;
        cameras[1].gameObject.SetActive(false); // Default camera is Centroid
        cameras[0].gameObject.SetActive(true);
        cameras[0].GetComponent<SmoothFollow>().target = centroid.transform; // Set default camera to follow the centroid
	}

	void Update() // Update the scene variables, called once per frame
    {
        if (Input.GetKeyDown(KeyCode.C)) // Switch cameras when C key is pressed
        {
            currentCameraIndex++;
            if (currentCameraIndex < cameras.Length) // Loop the array accessor
            {
                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                cameras[currentCameraIndex].gameObject.SetActive(true);
            }
            else
            {
                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                currentCameraIndex = 0;
                cameras[currentCameraIndex].gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < flock.Length; i++) // Update the flocking variables
        {
            centroid.transform.forward += flock[i].transform.forward; // Add all the Daleks' directions and positions,
            centroid.transform.position += flock[i].transform.position;
        }
        centroid.transform.forward = centroid.transform.forward / flock.Length; // Then divide them by the number of Daleks to compute the average
        centroid.transform.position = centroid.transform.position / flock.Length;
	}
}
