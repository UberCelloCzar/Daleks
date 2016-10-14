using UnityEngine;
using System.Collections;


// Trevor Walden, IGME 202 Section 2, December 14, 2015
// This camera smoothes out rotation around the y-axis and height.
// Horizontal Distance to the target is always fixed.
// For every of those smoothed values we calculate the wanted value and the current value.
// Then we smooth it using the Lerp function.
// Then we apply the smoothed values to the transform's position.

public class SmoothFollow : MonoBehaviour 
{
	public Transform target;
	public float distance = 3.0f;
	public float height = 1.50f;
	public float heightDamping = 2.0f;
	public float positionDamping =2.0f;
	public float rotationDamping = 2.0f;
	
	void LateUpdate () // Update called last once every frame
	{
		
		if (!target) // Early out if we don't have a target
			return;
		
		float wantedHeight = target.position.y + height;
		float currentHeight = transform.position.y;
		
		
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime); // Damp the height
		
		
		Vector3 wantedPosition = target.position - target.forward * distance; // Set the position of the camera 
		transform.position = Vector3.Lerp (transform.position, wantedPosition, Time.deltaTime * positionDamping);
		
		
		transform.position = new Vector3 (transform.position.x, currentHeight, transform.position.z); // Adjust the height of the camera
		
		
		
		transform.forward = Vector3.Lerp (transform.forward, target.forward , Time.deltaTime * rotationDamping); // Rotate the camera
		
	}
}