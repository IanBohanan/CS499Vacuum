using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BasePathingAlg
{
    // Abstract Method to be implemented by pathing algs
    void nextMove(float vacuumSpeed, Transform robot, Transform vacuum, Transform whiskers);
}
