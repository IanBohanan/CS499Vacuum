using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snaking
{
    public (Vector2, string) getNewDirectionVec(Vector2 currentDirection, float distanceUp,float distanceRight, float distanceDown, float distanceLeft)
    {
        Vector2 directionVec = new Vector2(); // New direction to be returned
        string offsetDirection = "right"; // The direction the vacuum needs to move in before snaking back the way it came

        if (currentDirection.x > 0) // Moving right
        {
            offsetDirection = "right";
            if (currentDirection.y > 0) // Moving up
            {
                if (distanceUp > distanceRight)
                {
                    // Want to offset Down
                }
                directionVec = -currentDirection;
            }
            else if (currentDirection.y < 0) // Moving down
            {
                // Want to offset Up
                directionVec = -currentDirection;
            }
            else // We're not moving somehow
            {
                directionVec = getStartingVec(); // Just get random vector, I guess
            }
        }
        else if (currentDirection.x < 0) // Moving left
        {

        }
        else // Moving perfectly vertically (very low chance, but technically possible)
        {
            offsetDirection = "right";
            if (currentDirection.y > 0) // Moving up
            {
                directionVec = -currentDirection;
            }
            else if (currentDirection.y < 0) // Moving down
            {
                directionVec = -currentDirection;
            }
            else // We're not moving somehow
            {
                directionVec = getStartingVec(); // Just get random vector, I guess
            }
        }

        directionVec.Normalize(); // Normalize so that magnitude can be applied later on

        return (directionVec, offsetDirection);
    }

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
        return (directionVec);
    }
}
