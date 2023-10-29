using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWalk
{
    public Vector2 getNewDirectionVec(bool movingPositively, bool horizontalObjCollision)
    {
        // Generate randomized angle to be used:
        System.Random random = new System.Random();
        double newAngle = random.Next(0, 180); // Collisions only possible with flat object, meaning 180 degrees of valid range for new movement direction

        newAngle = newAngle * Math.PI / 180; // Convert degrees to radians

        Vector2 directionVec = new Vector2(); // Initialize new 2D direction vector

        if (movingPositively) // Was moving upwards or rightwards
        {
            if (horizontalObjCollision) // Collided with horizontal object, therefore moving upwards
            {
                directionVec.x = (float)Math.Cos(newAngle);
                directionVec.y = (float)Math.Sin(newAngle);
            }
            else // Collided with vertical object, therefore moving rightwards
            {
                directionVec.x = (float)Math.Sin(newAngle);
                directionVec.y = (float)Math.Cos(newAngle);
            }
        }
        else // Was moving downwards or leftwards
        {
            if (horizontalObjCollision) // Collided with horizontal object, therefore moving downwards
            {
                directionVec.x = (float)Math.Cos(newAngle);
                directionVec.y = (float)-Math.Sin(newAngle);
            }
            else // Collided with vertical object, therefore moving leftwards
            {
                directionVec.x = (float)-Math.Sin(newAngle);
                directionVec.y = (float)Math.Cos(newAngle);
            }
        }

        directionVec.Normalize(); // Normalize so that magnitude can be applied later on

        return (directionVec);
    }
}
