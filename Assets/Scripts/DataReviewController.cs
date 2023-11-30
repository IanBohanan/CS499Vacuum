using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static LayoutManager; // Import a LayoutManager class, possibly from another script or package.

#region JSON Serializable Classes
[Serializable]
public class Settings
{
    public bool whiskers; // Boolean flag for whiskers setting.
    public string floorCovering; // String representing the floor covering type.
    public int batteryLifeStart; // Starting battery life value.
}

[Serializable]
public class RandomData
{
    public int elapsedTime; // Elapsed time data.
    public int batteryLifeEnd; // Ending battery life data.
    public int cleaningEfficiency; // Cleaning efficiency data.
    public int tilesCleaned; // Number of tiles cleaned data.
    public int untouchedTiles; // Number of untouched tiles data.
}

[Serializable]
public class SimulationEntry
{
    public Settings Settings; // Settings for a simulation run.
    public RandomData Random; // Random data for a simulation run.
    public RandomData WallFollow; // Wall follow data for a simulation run.
    public RandomData Spiral; // Spiral data for a simulation run.
    public RandomData Snaking; // Snaking data for a simulation run.
}

[Serializable]
public class RootObject<T>
{
    public List<T> SIMULATION_DATA; // List of simulation entries.
}
#endregion

public class DataReviewController : MonoBehaviour
{
    int currentRun = 0; // The currently viewed run index.
    string jsonFile = "Layout1"; // The JSON file to load data from.

    TreeView data;
    Button returnToMainMenu;
    Button prevBtn;
    Button nextBtn;

    // JSON Data for Currently Viewed Run:
    int runNumber; // The run number (not used in this script).
    bool whiskers; // Whiskers setting for the current run (not used in this script).
    string floorCovering; // Floor covering type for the current run (not used in this script).
    float batteryLifeStart; // Starting battery life for the current run (not used in this script).
    // List<RunData> algorithmRuns = new List<RunData>(); // Commented out code, possibly not used.

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
        // Read and parse JSON data from a file.
        string unparsedJSON = System.IO.File.ReadAllText(Application.dataPath + "/StreamingAssets/" + jsonFile + ".json");
        RootObject<SimulationEntry> parsedJSON = JsonUtility.FromJson<RootObject<SimulationEntry>>(unparsedJSON);

        // Check if there are more runs to navigate to.
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
        // Read and parse JSON data from a file.
        string unparsedJSON = System.IO.File.ReadAllText(Application.dataPath + "/StreamingAssets/" + jsonFile + ".json");
        RootObject<SimulationEntry> parsedJSON = JsonUtility.FromJson<RootObject<SimulationEntry>>(unparsedJSON);

        // Add run number label to the data panel.
        AddRunNumberLabel("Run Number: " + (currentRun + 1));

        // Add settings section with relevant data.
        AddNewSectionHeader("\nSettings:");
        AddLabelToData("Whiskers: " + parsedJSON.SIMULATION_DATA[currentRun].Settings.whiskers);
        AddLabelToData("Floor Covering: " + parsedJSON.SIMULATION_DATA[currentRun].Settings.floorCovering);
        AddLabelToData("Starting Battery Life: " + parsedJSON.SIMULATION_DATA[currentRun].Settings.batteryLifeStart);

        // Add random data section with relevant data.
        AddNewSectionHeader("\nRandom:");
        AddLabelToData("Elapsed Time: " + parsedJSON.SIMULATION_DATA[currentRun].Random.elapsedTime);
        AddLabelToData("Ending Battery Life: " + parsedJSON.SIMULATION_DATA[currentRun].Random.batteryLifeEnd);
        AddLabelToData("Cleaning Efficiency: " + parsedJSON.SIMULATION_DATA[currentRun].Random.cleaningEfficiency);
        AddLabelToData("Tiles Cleaned: " + parsedJSON.SIMULATION_DATA[currentRun].Random.tilesCleaned);
        AddLabelToData("Untouched Tiles: " + parsedJSON.SIMULATION_DATA[currentRun].Random.untouchedTiles);

        // Add wall follow data section with relevant data.
        AddNewSectionHeader("\nWall Follow:");
        AddLabelToData("Elapsed Time: " + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.elapsedTime);
        AddLabelToData("Ending Battery Life: " + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.batteryLifeEnd);
        AddLabelToData("Cleaning Efficiency: " + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.cleaningEfficiency);
        AddLabelToData("Tiles Cleaned: " + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.tilesCleaned);
        AddLabelToData("Untouched Tiles: " + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.untouchedTiles);

        // Add spiral data section with relevant data.
        AddNewSectionHeader("\nSpiral:");
        AddLabelToData("Elapsed Time: " + parsedJSON.SIMULATION_DATA[currentRun].Spiral.elapsedTime);
        AddLabelToData("Ending Battery Life: " + parsedJSON.SIMULATION_DATA[currentRun].Spiral.batteryLifeEnd);
        AddLabelToData("Cleaning Efficiency: " + parsedJSON.SIMULATION_DATA[currentRun].Spiral.cleaningEfficiency);
        AddLabelToData("Tiles Cleaned: " + parsedJSON.SIMULATION_DATA[currentRun].Spiral.tilesCleaned);
        AddLabelToData("Untouched Tiles: " + parsedJSON.SIMULATION_DATA[currentRun].Spiral.untouchedTiles);

        // Add snaking data section with relevant data.
        AddNewSectionHeader("\nSnaking:");
        AddLabelToData("Elapsed Time: " + parsedJSON.SIMULATION_DATA[currentRun].Snaking.elapsedTime);
        AddLabelToData("Ending Battery Life: " + parsedJSON.SIMULATION_DATA[currentRun].Snaking.batteryLifeEnd);
        AddLabelToData("Cleaning Efficiency: " + parsedJSON.SIMULATION_DATA[currentRun].Snaking.cleaningEfficiency);
        AddLabelToData("Tiles Cleaned: " + parsedJSON.SIMULATION_DATA[currentRun].Snaking.tilesCleaned);
        AddLabelToData("Untouched Tiles: " + parsedJSON.SIMULATION_DATA[currentRun].Snaking.untouchedTiles);
    }

    private void AddRunNumberLabel(string text)
    {
        // Add a label for the run number.
        Label newHeader = new Label(text);
        newHeader.style.fontSize = 18;
        newHeader.style.color = Color.white;
        newHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
        newHeader.style.alignSelf = Align.Center;
        data.hierarchy.ElementAt(0).Add(newHeader);
    }

    private void AddNewSectionHeader(string text)
    {
        // Add a header for a new section.
        Label newHeader = new Label(text);
        newHeader.style.fontSize = 16;
        newHeader.style.color = Color.white;
        newHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
        data.hierarchy.ElementAt(0).Add(newHeader);
    }

    private void AddLabelToData(string text)
    {
        // Add a label with data to the data panel.
        Label newLabel = new Label(text);
        newLabel.style.fontSize = 12;
        newLabel.style.color = Color.white;
        newLabel.style.marginLeft = 50;
        data.hierarchy.ElementAt(0).Add(newLabel);
    }
}