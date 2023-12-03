// This script is a Unity MonoBehaviour that manages a data review interface. 
// It loads JSON data from a file, parses it into specific data structures, and 
// displays the information in a Unity UI using the UnityEngine.UIElements library.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static LayoutManager;

#region JSON Serializable Classes
// Serializable class for storing settings data
[Serializable]
public class Settings
{
    public string dateTime = "";
    public bool whiskers = false;
    public string floorCovering = "Hardwood";
    public int batteryLifeStart = 0;
}

// Serializable class for storing random data   

[Serializable]
public class RandomData
{
    public string algVersion = "1.0";
    public string elapsedTime = "00:00:00";
    public string batteryLifeEnd = "00:00:00";
    public float cleaningEfficiency = 0;
    public int tilesPartiallyCleaned = 0;
    public int tilesCleaned = 0;
    public int untouchedTiles = 0;
}
// Serializable class for a simulation entry that includes settings and various data for different algorithms
[Serializable]
public class SimulationEntry
{
    public Settings Settings = new Settings();
    public RandomData Random = new RandomData();
    public RandomData WallFollow = new RandomData();
    public RandomData Spiral = new RandomData();
    public RandomData Snaking = new RandomData();
}
// Serializable class for the root JSON object
[Serializable]
public class RootObject<T>
{
    public List<T> SIMULATION_DATA;
}
#endregion


public class DataReviewController : MonoBehaviour
{
    int currentRun = 0;
    string jsonFile = InterSceneManager.fileSelection;

    TreeView data;
    Button returnToMainMenu;
    Button prevBtn;
    Button nextBtn;

    // JSON Data for Currently Viewed Run:
    int runNumber;
    bool whiskers;
    string floorCovering;
    float batteryLifeStart;
    //List<RunData> algorithmRuns = new List<RunData>();

    void OnEnable()
    {
        // Get UIDocument Root:
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Get top panel items:
        returnToMainMenu = root.Q<Button>("GoHomeButton");

        // Get bottom panel items:
        prevBtn = root.Q<Button>("PrevButton");
        nextBtn = root.Q<Button>("NextButton");

        // Get Data Panel Items:
        data = root.Q<TreeView>("Data");

        subscribeToCallbacks();

        fillDataPanel();
    }

    // Subscribe to function callbacks:
    private void subscribeToCallbacks()
    {
        returnToMainMenu.clicked += () => { SceneManager.LoadScene(sceneName: "MainMenu"); };
        prevBtn.clicked += () => { prevButtonHandler(); };
        nextBtn.clicked += () => { nextButtonHandler(); };
    }

    private void prevButtonHandler()
    {
        if (currentRun > 0)
        {
            currentRun--;
            clearDataPanel();
            fillDataPanel();
        }
    }

    private void nextButtonHandler()
    {
        string unparsedJSON = System.IO.File.ReadAllText(Application.dataPath + "/StreamingAssets/" + jsonFile + ".json");
        RootObject<SimulationEntry> parsedJSON = JsonUtility.FromJson<RootObject<SimulationEntry>>(unparsedJSON);
        if (currentRun < parsedJSON.SIMULATION_DATA.Count - 1)
        {
            currentRun++;
            clearDataPanel();
            fillDataPanel();
        }
    }

    // Remove all elements from the data TreeView:
    private void clearDataPanel()
    {
        data.hierarchy.ElementAt(0).Clear();
    }

    // Load the data TreeView with a new run's info:
    private void fillDataPanel()
    {
        string unparsedJSON = System.IO.File.ReadAllText(Application.dataPath + "/StreamingAssets/" + jsonFile + ".json");
        RootObject<SimulationEntry> parsedJSON = JsonUtility.FromJson<RootObject<SimulationEntry>>(unparsedJSON);

        AddRunNumberLabel("Run Number: "+(currentRun+1));
        AddRunNumberLabel("Date & Time: " + parsedJSON.SIMULATION_DATA[currentRun].Settings.dateTime);

        AddNewSectionHeader("\nSettings:");
        AddLabelToData("Whiskers: "+parsedJSON.SIMULATION_DATA[currentRun].Settings.whiskers);
        AddLabelToData("Floor Covering: "+parsedJSON.SIMULATION_DATA[currentRun].Settings.floorCovering);
        AddLabelToData("Starting Battery Life: "+parsedJSON.SIMULATION_DATA[currentRun].Settings.batteryLifeStart);
        AddLabelToData("Covered Tiles: " + InterSceneManager.coveredTileNum);
        AddLabelToData("Covered Square Feet: " + (InterSceneManager.coveredTileNum/2));

        AddNewSectionHeader("\nRandom (" + parsedJSON.SIMULATION_DATA[currentRun].Random.algVersion+"):");
        AddLabelToData("Elapsed Time: " + parsedJSON.SIMULATION_DATA[currentRun].Random.elapsedTime);
        AddLabelToData("Ending Battery Life: " + parsedJSON.SIMULATION_DATA[currentRun].Random.batteryLifeEnd);
        AddLabelToData("Cleaning Efficiency: " + parsedJSON.SIMULATION_DATA[currentRun].Random.cleaningEfficiency + "%");
        AddLabelToData("Tiles Partially Cleaned: " + parsedJSON.SIMULATION_DATA[currentRun].Random.tilesPartiallyCleaned);
        AddLabelToData("Tiles Fully Cleaned: " + parsedJSON.SIMULATION_DATA[currentRun].Random.tilesCleaned);
        AddLabelToData("Untouched Tiles: " + parsedJSON.SIMULATION_DATA[currentRun].Random.untouchedTiles);
        AddLabelToData("Sqft Partially Cleaned: " + (parsedJSON.SIMULATION_DATA[currentRun].Random.tilesPartiallyCleaned / 2));
        AddLabelToData("Sqft Fully Cleaned: " + (parsedJSON.SIMULATION_DATA[currentRun].Random.tilesCleaned / 2));
        AddLabelToData("Untouched Sqft: " + (parsedJSON.SIMULATION_DATA[currentRun].Random.untouchedTiles / 2));


        AddNewSectionHeader("\nWall Follow (" + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.algVersion + "):");
        AddLabelToData("Elapsed Time: " + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.elapsedTime);
        AddLabelToData("Ending Battery Life: " + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.batteryLifeEnd);
        AddLabelToData("Cleaning Efficiency: " + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.cleaningEfficiency + "%");
        AddLabelToData("Tiles Partially Cleaned: " + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.tilesPartiallyCleaned);
        AddLabelToData("Tiles Fully Cleaned: " + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.tilesCleaned);
        AddLabelToData("Untouched Tiles: " + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.untouchedTiles);
        AddLabelToData("Sqft Partially Cleaned: " + (parsedJSON.SIMULATION_DATA[currentRun].WallFollow.tilesPartiallyCleaned / 2));
        AddLabelToData("Sqft Fully Cleaned: " + (parsedJSON.SIMULATION_DATA[currentRun].WallFollow.tilesCleaned / 2));
        AddLabelToData("Untouched Sqft: " + (parsedJSON.SIMULATION_DATA[currentRun].WallFollow.untouchedTiles / 2));

        AddNewSectionHeader("\nSpiral (" + parsedJSON.SIMULATION_DATA[currentRun].Spiral.algVersion + "):");
        AddLabelToData("Elapsed Time: " + parsedJSON.SIMULATION_DATA[currentRun].Spiral.elapsedTime);
        AddLabelToData("Ending Battery Life: " + parsedJSON.SIMULATION_DATA[currentRun].Spiral.batteryLifeEnd);
        AddLabelToData("Cleaning Efficiency: " + parsedJSON.SIMULATION_DATA[currentRun].Spiral.cleaningEfficiency + "%");
        AddLabelToData("Tiles Partially Cleaned: " + parsedJSON.SIMULATION_DATA[currentRun].Spiral.tilesPartiallyCleaned);
        AddLabelToData("Tiles Fully Cleaned: " + parsedJSON.SIMULATION_DATA[currentRun].Spiral.tilesCleaned);
        AddLabelToData("Untouched Tiles: " + parsedJSON.SIMULATION_DATA[currentRun].Spiral.untouchedTiles);
        AddLabelToData("Sqft Partially Cleaned: " + (parsedJSON.SIMULATION_DATA[currentRun].Spiral.tilesPartiallyCleaned / 2));
        AddLabelToData("Sqft Fully Cleaned: " + (parsedJSON.SIMULATION_DATA[currentRun].Spiral.tilesCleaned / 2));
        AddLabelToData("Untouched Sqft: " + (parsedJSON.SIMULATION_DATA[currentRun].Spiral.untouchedTiles / 2));

        AddNewSectionHeader("\nSnaking (" + parsedJSON.SIMULATION_DATA[currentRun].Snaking.algVersion + "):");
        AddLabelToData("Elapsed Time: " + parsedJSON.SIMULATION_DATA[currentRun].Snaking.elapsedTime);
        AddLabelToData("Ending Battery Life: " + parsedJSON.SIMULATION_DATA[currentRun].Snaking.batteryLifeEnd);
        AddLabelToData("Cleaning Efficiency: " + parsedJSON.SIMULATION_DATA[currentRun].Snaking.cleaningEfficiency + "%");
        AddLabelToData("Tiles Partially Cleaned: " + parsedJSON.SIMULATION_DATA[currentRun].Snaking.tilesPartiallyCleaned);
        AddLabelToData("Tiles Fully Cleaned: " + parsedJSON.SIMULATION_DATA[currentRun].Snaking.tilesCleaned);
        AddLabelToData("Untouched Tiles: " + parsedJSON.SIMULATION_DATA[currentRun].Snaking.untouchedTiles);
        AddLabelToData("Sqft Partially Cleaned: " + (parsedJSON.SIMULATION_DATA[currentRun].Snaking.tilesPartiallyCleaned / 2));
        AddLabelToData("Sqft Fully Cleaned: " + (parsedJSON.SIMULATION_DATA[currentRun].Snaking.tilesCleaned / 2));
        AddLabelToData("Untouched Sqft: " + (parsedJSON.SIMULATION_DATA[currentRun].Snaking.untouchedTiles / 2));
    }
    private void AddRunNumberLabel(string text)
    {
        Label newHeader = new Label(text);
        newHeader.style.fontSize = 18;
        newHeader.style.color = Color.white;
        newHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
        newHeader.style.alignSelf = Align.Center;
        data.hierarchy.ElementAt(0).Add(newHeader);
    }

    private void AddNewSectionHeader(string text)
    {
        Label newHeader = new Label(text);
        newHeader.style.fontSize = 16;
        newHeader.style.color = Color.white;
        newHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
        data.hierarchy.ElementAt(0).Add(newHeader);
    }

    private void AddLabelToData(string text)
    {
        Label newLabel = new Label(text);
        newLabel.style.fontSize = 12;
        newLabel.style.color = Color.white;
        newLabel.style.marginLeft = 50;
        data.hierarchy.ElementAt(0).Add(newLabel);
    }
}
