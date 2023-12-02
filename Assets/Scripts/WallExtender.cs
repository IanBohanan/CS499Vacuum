// This script, WallExtender, is responsible for extending walls in a Unity game.
// It allows the player to click on the extender to extend a wall and detects whether
// it is connected to another wall or not.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallExtender : MonoBehaviour
{
    public WallPlacer wall; // Reference to the WallPlacer script on the prefab wall object

    public bool connectedToWall = false; // Indicates whether this extender is touching another wall

    private void Start()
    {
        connectedToWall = false;
    }

    private void OnEnable()
    {
        connectedToWall = false;
    }

    // Called when the mouse is clicked on the extender
    void OnMouseDown()
    {
        // Call the extendWall method in the WallPlacer script to extend the wall
        wall.extendWall(this.transform.position);
    }

    // Called when another collider enters the trigger zone of this extender
    void OnTriggerEnter2D(Collider2D collision)
    {
        try
        {
            // Check if the colliding object has the name "WallEndpoint," indicating it's a wall endpoint
            if (collision.gameObject.name.Equals("WallEndpoint"))
                connectedToWall = true;
        }
        catch (Exception e)
        {
            // Handle any exceptions that may occur
        }
    }

    // Called when another collider exits the trigger zone of this extender
    void OnTriggerExit2D(Collider2D other)
    {
        connectedToWall = false;
    }
}
