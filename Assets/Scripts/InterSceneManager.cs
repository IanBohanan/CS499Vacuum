using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InterSceneManager
{
    // Main Menu Import File:
    public static string fileSelection = "";

    public static List<GameObject> wallList = new List<GameObject>();

    public static List<GameObject> flagList = new List<GameObject>();

    // House Builder UI -> ClickDrop
    public static bool deleteMode = false;

    //House builder tiles that represent the rooms of the house
    //Represents the tiles that were explored by the flood fill algorithm of the house
    //Note: these are TILEMAP positions, not worldspace positions. Use the tilemap!
    public static List<Vector3Int> houseTiles;

    // Simulation Setup Settings:
    private static bool whiskersEnabled;
    private static string floorCovering;
    private static int batteryLife;
    private static bool randomAlgEnabled = false;
    private static bool spiralAlgEnabled = false;
    private static bool snakingAlgEnabled = false;
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

    public static void setAlgorithm(string algName, bool enabled = false)
    {
        if (algName == "random")
        {
            randomAlgEnabled = enabled;
        }
        else if (algName == "spiral")
        {
            spiralAlgEnabled = enabled;
        }
        else if (algName == "snaking")
        {
            snakingAlgEnabled = enabled;
        }
        else if (algName == "wallFollow")
        {
            wallFollowAlgEnabled = enabled;
        }
        else
        {
            Debug.Log("Invalid algorithm name given when setting algorithm state. What happened, George?");
        }
        return;
    }
}
