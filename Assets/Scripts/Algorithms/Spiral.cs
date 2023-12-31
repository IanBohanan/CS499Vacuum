// The "Spiral" class is responsible for generating movement directions and rotations for an entity
// following a spiral pattern. It provides methods for calculating new direction vectors, both
// when initially starting and when encountering a collision, as well as for rotating the entity as it spirals.
using System;
using UnityEngine;

public class Spiral
{
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
    public Vector2 getNewRandomDirectionVec(Vector2 collisionDirection)
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

    public (Vector2, Quaternion) getNewSpiralVec(Vector2 currentDirectionVec, Quaternion currentEulerRotation)
    {
        // Rotate vacuum:
        Quaternion newRotation = Quaternion.Euler(currentEulerRotation.x, currentEulerRotation.y, currentEulerRotation.z + 2);

        // Rotate direction vector:
        float radians = 2 * Mathf.Deg2Rad;
        float cosTheta = Mathf.Cos(radians);
        float sinTheta = Mathf.Sin(radians);
        float x = currentDirectionVec.x * cosTheta - currentDirectionVec.y * sinTheta;
        float y = currentDirectionVec.x * sinTheta - currentDirectionVec.y * cosTheta;
        Vector2 newDirectionVec = new Vector2(x,y);

        return (newDirectionVec, newRotation);
    }
}
