using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LayoutManager;

public class CheckForAlgsController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // First, save this run's data to JSON:

        // Elapsed Seconds:
        int elapsedSeconds = InterSceneManager.simulationElapsedSeconds;
        int elapsedMinutes = 0;
        int elapsedHours = 0;
        while (elapsedSeconds >59) 
        {
            elapsedMinutes++;
            elapsedSeconds -= 60;
        }
        while (elapsedMinutes > 59)
        {
            elapsedHours++;
            elapsedMinutes -= 60;
        }
        string SecondsString = elapsedSeconds.ToString();
        string MinutesString = elapsedMinutes.ToString();
        string HoursString = elapsedHours.ToString();
        if (SecondsString.Length == 1) { SecondsString = "0" + SecondsString; }
        if (MinutesString.Length == 1) { MinutesString = "0" + MinutesString; }
        if (HoursString.Length == 1) { HoursString = "0" + HoursString; }
        string finalElapsedTime = (HoursString + ":" + MinutesString + ":" + SecondsString);

        // Battery Life:
        int batteryLifeSeconds = InterSceneManager.endingBatteryLifeSeconds;
        int batteryLifeMinutes = 0;
        int batteryLifeHours = 0;
        while (batteryLifeSeconds > 59)
        {
            batteryLifeMinutes++;
            batteryLifeSeconds -= 60;
        }
        while (batteryLifeMinutes > 59)
        {
            batteryLifeHours++;
            batteryLifeMinutes -= 60;
        }
        SecondsString = batteryLifeSeconds.ToString();
        MinutesString = batteryLifeMinutes.ToString();
        HoursString = batteryLifeHours.ToString();
        if (SecondsString.Length == 1) { SecondsString = "0" + SecondsString; }
        if (MinutesString.Length == 1) { MinutesString = "0" + MinutesString; }
        HoursString = "0" + HoursString;
        string finalBatteryLife = (HoursString + ":" + MinutesString + ":" + SecondsString);

        // Save to JSON:
        SerializableList<LayoutManager.Object> parsedJSON = new SerializableList<LayoutManager.Object>();
        try
        {
            string unparsedJSON = System.IO.File.ReadAllText(Application.dataPath + "/StreamingAssets/" + InterSceneManager.fileSelection + ".json");
            parsedJSON = JsonUtility.FromJson<SerializableList<LayoutManager.Object>>(unparsedJSON);
        }
        catch (Exception e)
        {
            Debug.Log("JSON Import Exception: " + e.Message);
        }
        SimulationEntry newEntry = new SimulationEntry();
        newEntry.Settings.dateTime = InterSceneManager.startDateTime;
        newEntry.Settings.whiskers = InterSceneManager.whiskersEnabled;
        newEntry.Settings.batteryLifeStart = InterSceneManager.batteryLife;

        // Search through tile data:
        int touchedTileCount = 0;
        int fullyCleanCount = 0;
        int untouchedTileCount = 0;
        int totalHits = 0;
        float totalEfficiency = 0;
        foreach (SerializableTile tile in InterSceneManager.cleanedTiles)
        {
            if (tile.hits == 0)
            {
                untouchedTileCount++;
            }
            else
            {
                float vacEff = InterSceneManager.vacuumEfficiency;
                float whiskEff = InterSceneManager.whiskersEfficiency;
                tile.cleanliness = (tile.hits * (vacEff/120)) + (tile.hits * (whiskEff/120));
                if (tile.cleanliness >= 100)
                {
                    tile.cleanliness = 100; // Cap cleanliness percentage to 100
                    fullyCleanCount++;
                }
                else
                {
                    touchedTileCount++;
                }
                totalEfficiency += tile.cleanliness;
                totalHits += tile.hits;
            }
        }
        totalEfficiency = (totalEfficiency/InterSceneManager.cleanedTiles.Count);
        totalEfficiency = (float)Math.Truncate(totalEfficiency * 100) / 100;

        if (InterSceneManager.algorithmName == "Random")
        {
            newEntry.Random.elapsedTime = finalElapsedTime;
            newEntry.Random.tilesCleaned = fullyCleanCount;
            newEntry.Random.tilesPartiallyCleaned = touchedTileCount;
            newEntry.Random.untouchedTiles = untouchedTileCount;
            newEntry.Random.batteryLifeEnd = finalBatteryLife;
            newEntry.Random.cleaningEfficiency = totalEfficiency;
        }
        else if (InterSceneManager.algorithmName == "Spiral")
        {
            newEntry.Spiral.elapsedTime = finalElapsedTime;
            newEntry.Spiral.tilesCleaned = fullyCleanCount;
            newEntry.Spiral.tilesPartiallyCleaned = touchedTileCount;
            newEntry.Spiral.untouchedTiles = untouchedTileCount;
            newEntry.Spiral.batteryLifeEnd = finalBatteryLife;
            newEntry.Spiral.cleaningEfficiency = totalEfficiency;
        }
        else if (InterSceneManager.algorithmName == "Snaking")
        {
            newEntry.Snaking.elapsedTime = finalElapsedTime;
            newEntry.Snaking.tilesCleaned = fullyCleanCount;
            newEntry.Snaking.tilesPartiallyCleaned = touchedTileCount;
            newEntry.Snaking.untouchedTiles = untouchedTileCount;
            newEntry.Snaking.batteryLifeEnd = finalBatteryLife;
            newEntry.Snaking.cleaningEfficiency = totalEfficiency;
        }
        else if (InterSceneManager.algorithmName == "WallFollow")
        {
            newEntry.WallFollow.elapsedTime = finalElapsedTime;
            newEntry.WallFollow.tilesCleaned = fullyCleanCount;
            newEntry.WallFollow.tilesPartiallyCleaned = touchedTileCount;
            newEntry.WallFollow.untouchedTiles = untouchedTileCount;
            newEntry.WallFollow.batteryLifeEnd = finalBatteryLife;
            newEntry.WallFollow.cleaningEfficiency = totalEfficiency;
        }
        else 
        {
            Debug.Log("You've given me a non-existent algorithm name in the CheckForAlgsController.cs file, George.");
        }

        try
        {
            if (parsedJSON.SIMULATION_DATA.Count <= InterSceneManager.JSONEntryNum)
            {
                parsedJSON.SIMULATION_DATA.Add(newEntry);
            }
            else
            {
                if (InterSceneManager.algorithmName == "Random")
                {
                    parsedJSON.SIMULATION_DATA[InterSceneManager.JSONEntryNum].Random = newEntry.Random;
                }
                else if (InterSceneManager.algorithmName == "Spiral")
                {
                    parsedJSON.SIMULATION_DATA[InterSceneManager.JSONEntryNum].Spiral = newEntry.Spiral;
                }
                else if (InterSceneManager.algorithmName == "Snaking")
                {
                    parsedJSON.SIMULATION_DATA[InterSceneManager.JSONEntryNum].Snaking = newEntry.Snaking;
                }
                else if (InterSceneManager.algorithmName == "WallFollow")
                {
                    parsedJSON.SIMULATION_DATA[InterSceneManager.JSONEntryNum].WallFollow = newEntry.WallFollow;
                }
            }
            string FullJSON = JsonUtility.ToJson(parsedJSON);
            System.IO.File.WriteAllText(Application.dataPath + "/StreamingAssets/" + InterSceneManager.fileSelection + ".json", FullJSON);
        }
        catch (Exception e)
        {
            Debug.Log("JSON Saving Exception: " + e.Message);
        }

        SceneManager.LoadScene("ShowColorCodedResults");
    }
}
