using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallFollow
{
    bool turning = false;
    Vector2 currentDirectionVector; // Used to know what direction is "straight" if we're not turning

    public Vector2 getStartingVec()
    {
        // Generate randomized angle to be used:
        System.Random random = new System.Random();
        double newAngle = random.Next(0, 360);

        newAngle = newAngle * Math.PI / 180; // Convert degrees to radians

        Vector2 directionVec = new Vector2(); // Initialize new 2D direction vector

        directionVec.x = (float)Math.Cos(newAngle);
        directionVec.y = (float)Math.Sin(newAngle);

        directionVec.Normalize();

        currentDirectionVector = directionVec;
        return (directionVec);
    }

    public Vector2 getNewDirectionVec(bool collision)
    {
        if (collision) // If we hit something, turn right
        {
            return turnRight();
        }
        else if (!checkForWall()) // If the wall to out left disappears, turn left
        {
            return turnLeft();
        }
        else // Otherwise, just keep going where we've been going
        {
            return keepGoing();
        }
    }

    // This will check the raycaster distance to the nearest wall (must be given by vacuum) and set bool inside class to alert the movement algorithm if a wall stops being detected immediately next to us
    public bool checkForWall()
    {
        return true;
    }

    // This will slowly rotate the vacuum 90 degrees to the right and stop the vacuum from moving while doing so
    public Vector2 turnRight()
    {
        Vector2 newDirectionVec = new Vector2(0, 0);

        return (newDirectionVec);
    }

    // This will slowly rotate the vacuum 90 degrees to the left and stop the vacuum from moving while doing so
    public Vector2 turnLeft()
    {
        Vector2 newDirectionVec = new Vector2(0, 0);

        return (newDirectionVec);
    }

    public Vector2 keepGoing()
    {
        Vector2 newDirectionVec = new Vector2(0,0);

        return (newDirectionVec);
    }
}

