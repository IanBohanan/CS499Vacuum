using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InterSceneManager
{
    // Main Menu Import File:
    public static string fileSelection = "";

    // House Builder UI -> ClickDrop
    public static bool deleteMode = false;

    // Simulation Setup Settings:
    private static bool whiskersEnabled;
    private static string floorCovering;
    private static int batteryLife;
    private static bool randomAlgEnabled = false;
    private static bool spiralAlgEnabled = false;
    private static bool snakingAlgEnabled = true;
    private static bool wallFollowAlgEnabled = false;

    // Simulation Speed, used by Vacuum to multiply speed:
    public static int speedMultiplier = 1;
        
    public static void setSimulationSettings(bool whiskers, string floorCov, int battery, bool randomAlg, bool spiralAlg, bool snakingAlg, bool wallFollowAlg)
    {
        whiskersEnabled = whiskers;
        floorCovering = floorCov;
        batteryLife = battery; 
        randomAlgEnabled = randomAlg;
        spiralAlgEnabled = spiralAlg;
        snakingAlgEnabled = snakingAlg;
        wallFollowAlgEnabled = wallFollowAlg;
        return;
    }

    public static (bool, bool, bool, bool) getPathAlgs()
    {
        return (randomAlgEnabled, spiralAlgEnabled, snakingAlgEnabled, wallFollowAlgEnabled);
    }

    public static (bool, string, int) getSimulationSettings()
    {
        return (whiskersEnabled, floorCovering, batteryLife);
    }
}
