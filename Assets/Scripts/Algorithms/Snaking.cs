using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalk
{
    public Vector2 getNewDirectionVec(Vector2 collisionDirection)
    {
        // Generate randomized angle to be used:
        System.Random random = new System.Random();
        double newAngle = random.Next(0, 180); // Collisions only possible with flat object, meaning 180 degrees of valid range for new movement direction

        newAngle = newAngle * Math.PI / 180; // Convert degrees to radians

        Vector2 directionVec = new Vector2(); // Initialize new 2D direction vector

        if (collisionDirection.x == -1) // Collision to right of us
        {
            directionVec.x = (float)Math.Sin(newAngle);
            directionVec.y = (float)Math.Cos(newAngle);
        }
        else if (collisionDirection.y == -1) // Collision above us
        {
            directionVec.x = (float)Math.Cos(newAngle);
            directionVec.y = (float)Math.Sin(newAngle);
        }
        else if (collisionDirection.x == 1) // Collision to left of us
        {
            directionVec.x = (float)-Math.Sin(newAngle);
            directionVec.y = (float)Math.Cos(newAngle);
        }
        else if (collisionDirection.y == 1) // Collision below us
        {
            directionVec.x = (float)Math.Cos(newAngle);
            directionVec.y = (float)-Math.Sin(newAngle);
        }
        else // Invalid direction vector
        {
            Debug.Log("Invalid direction vector given to Random Algorithm!");
        }

        directionVec.Normalize(); // Normalize so that magnitude can be applied later on

        return (directionVec);
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
