using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class is used to manage data across different scenes in a Unity game
public static class InterSceneManager
{
    // Variable to store file selection from Main Menu
    public static string fileSelection = "";

    // Flag to toggle delete mode in House Builder UI
    public static bool deleteMode = false;

    // Private variables to store Simulation Setup Settings:
    private static bool whiskersEnabled; // Flag for enabling whiskers
    private static string floorCovering; // Type of floor covering
    private static int batteryLife; // Duration of battery life
    private static bool randomAlgEnabled; // Flag for enabling random algorithm
    private static bool spiralAlgEnabled; // Flag for enabling spiral algorithm
    private static bool snakingAlgEnabled; // Flag for enabling snaking algorithm
    private static bool wallFollowAlgEnabled; // Flag for enabling wall following algorithm

    // Public variable to adjust simulation speed
    public static int speedMultiplier = 1; // Multiplier for simulation speed
    
    // Method to set simulation settings
    public static void setSimulationSettings(bool whiskers, string floorCov, int battery, bool randomAlg, bool spiralAlg, bool snakingAlg, bool wallFollowAlg)
    {
        whiskersEnabled = whiskers; // Set whiskers flag
        floorCovering = floorCov; // Set floor covering type
        batteryLife = battery; // Set battery life
        randomAlgEnabled = randomAlg; // Set random algorithm flag
        spiralAlgEnabled = spiralAlg; // Set spiral algorithm flag
        snakingAlgEnabled = snakingAlg; // Set snaking algorithm flag
        wallFollowAlgEnabled = wallFollowAlg; // Set wall following algorithm flag
    }

    // Method to get the status of path algorithms
    public static (bool, bool, bool, bool) getPathAlgs()
    {
        // Return the status of each pathfinding algorithm
        return (randomAlgEnabled, spiralAlgEnabled, snakingAlgEnabled, wallFollowAlgEnabled);
    }

    // Method to get current simulation settings
    public static (bool, string, int) getSimulationSettings()
    {
        // Return the whiskers enabled flag, floor covering type, and battery life
        return (whiskersEnabled, floorCovering, batteryLife);
    }
}
