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

    public Vector2 getFirstCollisionVec(Vector2 currentDirection)
    {
        if ((currentDirection.x < 0) && (currentDirection.y < 0))
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
        else return new Vector2(0, 0); // Should never run

        //Vector2 newDirectionVector = currentDirection;
        //double x = Math.Round(currentDirection.x, 2);
        //double y = Math.Round(currentDirection.y, 2);

        //int infiniteLoopStopper = 10000; // Just in case...

        //while (((Math.Abs(x) != 1) && (Math.Abs(y) != 1)) && (infiniteLoopStopper > 0))
        //{
        //    float radians = MathF.PI * 1 / 180.0f; // Turn 1 degree
        //    float cosTheta = MathF.Cos(radians);
        //    float sinTheta = MathF.Sin(radians);

        //    newDirectionVector.x = newDirectionVector.x * cosTheta - newDirectionVector.y * sinTheta;
        //    newDirectionVector.y = newDirectionVector.x * sinTheta + newDirectionVector.y * cosTheta;

        //    x = (double)Math.Round(newDirectionVector.x);
        //    y = (double)Math.Round(newDirectionVector.y);

        //    infiniteLoopStopper--;
        //}
        ////while (!IsCardinalDirection(newDirectionVector))
        ////{
        ////    float radians = MathF.PI * 1 / 180.0f; // Turn 1 degree
        ////    float cosTheta = MathF.Cos(radians);
        ////    float sinTheta = MathF.Sin(radians);

        ////    newDirectionVector = new Vector2(
        ////        currentDirection.x * cosTheta - currentDirection.y * sinTheta,
        ////        currentDirection.x * sinTheta + currentDirection.y * cosTheta
        ////    );
        ////}
        //newDirectionVector = new Vector2((float)x, (float)y);
        //newDirectionVector.Normalize();
        //return newDirectionVector;
    }

    private bool IsCardinalDirection(Vector2 vector)
    {
        // Check if the vector is pointing in one of the cardinal directions.
        float angle = MathF.Atan2(vector.y, vector.x);
        angle = MathF.Abs(angle); // Make sure we're checking the absolute angle.
        
        bool result = (angle <= MathF.PI / 4.0f || angle >= 7.0f * MathF.PI / 4.0f);
        return result;
    }

    public Vector2 getNewDirectionVec(Vector2 currentDirection, bool collision, bool horizontalCollision, bool wallToLeft)
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

    // This will slowly rotate the vacuum 90 degrees to the right and stop the vacuum from moving while doing so
    public Vector2 turnRight(Vector2 currentVec)
    {
        Vector2 newDirectionVec = new Vector2(currentVec.y, -currentVec.x);

        return (newDirectionVec);
    }

    // This will slowly rotate the vacuum 90 degrees to the left and stop the vacuum from moving while doing so
    public Vector2 turnLeft(Vector2 currentVec)
    {
        Vector2 newDirectionVec = new Vector2(-currentVec.y, currentVec.x);

        return (newDirectionVec);
    }
}

