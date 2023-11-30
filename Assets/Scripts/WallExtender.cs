using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallExtender : MonoBehaviour
{
    public WallPlacer wall; // Reference to the WallPlacer script on the prefab wall object.

    public bool connectedToWall = false; // Indicates whether this extender is touching another wall.

    private void Start()
    {
        connectedToWall = false; // Initialize the connectedToWall flag as false when the object is created.
    }

    private void OnEnable()
    {
        connectedToWall = false; // Reset the connectedToWall flag to false when the object is enabled.
    }

    void OnMouseDown()
    {
        // When the object is clicked, call the extendWall method of the associated WallPlacer script.
        wall.extendWall(this.transform.position);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        try
        {
            // Check if a collision occurred with an object named "WallEndpoint".
            if (collision.gameObject.name.Equals("WallEndpoint"))
                connectedToWall = true; // Set connectedToWall to true if the collision happened with a wall endpoint.
        }
        catch (Exception e)
        {
            // Handle any exceptions that may occur during collision checking.
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        connectedToWall = false; // When exiting a trigger zone, set connectedToWall to false to indicate no longer connected to a wall.
    }
}
