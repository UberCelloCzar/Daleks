using UnityEngine;
using System.Collections;

// Trevor Walden, IGME 202 Section 2, December 14, 2015
// This script contains the radius of the obstacles used for calculating obstacle avoidance

public class ObstacleScript : MonoBehaviour
{
    private float radius; // Radius for the vehicles to avoid around the obstacle
    public float Radius
    {
        get { return radius; }
    }

    public ObstacleScript(int type) // Set radius based on prefab size
    {
        switch (type)
        {
            case 0:
                radius = 24.05f; // Asteroid E
                break;
            case 1:
                radius = 22.5f; // Asteroid F
                break;
            case 2:
                radius = 25f; // Asteroid H
                break;
        }
    }
}

	