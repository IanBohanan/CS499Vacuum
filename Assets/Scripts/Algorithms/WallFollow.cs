using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
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

    public Vector2 getFirstCollisionVec(Vector2 currentDirection, bool horizontalWall)
    {
/*        if ((currentDirection.x < 0) && (currentDirection.y < 0))
        {
            return new Vector2(-1, 0);
        }
        else if ((currentDirection.x < 0) && (currentDirection.y > 0))
        {
            return new Vector2(0, 1);
        }
        else if ((currentDirection.x > 0) && (currentDirection.y < 0))
        {
            return new Vector2(0, -1);
        }
        else if ((currentDirection.x > 0) && (currentDirection.y > 0))
        {
            return new Vector2(1, 0);
        }
        else return new Vector2(0, 0); // Should never run*/

        if (horizontalWall)
        {
            return new Vector2(1,0);
        }
        else
        {
            return new Vector2(0, -1);
        }
    }

    private bool IsCardinalDirection(Vector2 vector)
    {
        // Check if the vector is pointing in one of the cardinal directions.
        float angle = MathF.Atan2(vector.y, vector.x);
        angle = MathF.Abs(angle); // Make sure we're checking the absolute angle.
        
        bool result = (angle <= MathF.PI / 4.0f || angle >= 7.0f * MathF.PI / 4.0f);
        return result;
    }

    public Vector2 getNewDirectionVec(Vector2 currentDirection, bool collision, bool wallToLeft)
    {
        if (collision) // If we hit something, turn right
        {
            return turnRight(currentDirection);
        }
        else if (!wallToLeft) // If the wall to out left disappears, turn left
        {
            return turnLeft(currentDirection);
        }
        else // Otherwise, just keep going where we've been going
        {
            return currentDirection;
        }
    }

    // Rotates given vector 90 degrees to the right
    public Vector2 turnRight(Vector2 currentVec)
    {
        Debug.Log("old: "+currentVec);
        Vector2 newDirectionVec = new Vector2(currentVec.y, -currentVec.x);
        Debug.Log("new: "+newDirectionVec);
        return (newDirectionVec);
    }

    // This will slowly rotate the vacuum 90 degrees to the left and stop the vacuum from moving while doing so
    public Vector2 turnLeft(Vector2 currentVec)
    {
        Vector2 newDirectionVec = new Vector2(-currentVec.y, currentVec.x);

        return (newDirectionVec);
    }
}

