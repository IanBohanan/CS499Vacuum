using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// WallExtender class is responsible for detecting interactions between wall endpoints and triggering wall extension.
public class WallExtender : MonoBehaviour
{
    public WallPlacer wall; // Reference to the WallPlacer script on the wall prefab.

    public bool connectedToWall = false; // Flag to check if this extender is touching another wall.

    // Initialize the connectedToWall flag when the script starts.
    private void Start()
    {
        connectedToWall = false;
    }

    // Reset the connectedToWall flag when the GameObject is enabled.
    private void OnEnable()
    {
        connectedToWall = false;
    }

    // OnMouseDown is called when the user clicks on the GameObject.
    void OnMouseDown()
    {
        // Trigger wall extension from the current position.
        wall.extendWall(this.transform.position);
    }

    // OnTriggerEnter2D is called when another collider enters the trigger (2D physics only).
    void OnTriggerEnter2D(Collider2D collision)
    {
        try
        {
            // Check if the collision is with a wall endpoint and update the connectedToWall flag accordingly.
            if (collision.gameObject.name.Equals("WallEndpoint"))
                connectedToWall = true;
            else
            {
                connectedToWall = false;
            }
        }
        catch (Exception e)
        {
            // If an exception occurs, ensure connectedToWall is set to false.
            connectedToWall = false;
        }
    }

    // OnTriggerExit2D is called when another collider leaves the trigger (2D physics only).
    void OnTriggerExit2D(Collider2D other)
    {
        // Reset the connectedToWall flag when the collider exits the trigger.
        connectedToWall = false;
    }
}