using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static LayoutManager;
// Define serializable classes to be used for parsing JSON data
#region JSON Serializable Classes
[Serializable]
public class Settings
{
    public bool whiskers;
    public string floorCovering;
    public int batteryLifeStart;
}

[Serializable]
public class RandomData
{
    public int elapsedTime;
    public int batteryLifeEnd;
    public int cleaningEfficiency;
    public int tilesCleaned;
    public int untouchedTiles;
}

[Serializable]
public class SimulationEntry
{
    public Settings Settings;
    public RandomData Random;
    public RandomData WallFollow;
    public RandomData Spiral;
    public RandomData Snaking;
}

[Serializable]
public class RootObject<T>
{
    public List<T> SIMULATION_DATA;
}
#endregion

// Controller class for reviewing data within the Unity Engine
public class DataReviewController : MonoBehaviour
{
    int currentRun = 0; // Index of the current simulation run being reviewed
    string jsonFile = "Layout1"; // The default JSON file name that contains simulation data
    
     // UI Elements that will be interacted with in the scene
    TreeView data;
    Button returnToMainMenu;
    Button prevBtn;
    Button nextBtn;

    // Fields for displaying JSON data for the currently viewed run
    // These are not used in the current script and can be removed or implemented for additional features.
    int runNumber;
    bool whiskers;
    string floorCovering;
    float batteryLifeStart;
    // List<RunData> algorithmRuns = new List<RunData>();

    // Called when the script is loaded and the GameObject it is attached to is activated
    void OnEnable()
    {
        // Access the UI Document and retrieve the root visual element
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Initialize UI elements by finding them by name in the UI hierarchy
        returnToMainMenu = root.Q<Button>("GoHomeButton");

        // Get bottom panel items:
        prevBtn = root.Q<Button>("PrevButton");
        nextBtn = root.Q<Button>("NextButton");

        // Get Data Panel Items:
        data = root.Q<TreeView>("Data");
        // Setup callback functions for button interactions
        subscribeToCallbacks();

        // Load and display the initial data onto the UI
        fillDataPanel();
    }

    // Method for setting up callbacks for button interactions
    private void subscribeToCallbacks()
    {
        // When the main menu button is clicked, load the main menu scene
        returnToMainMenu.clicked += () => { SceneManager.LoadScene(sceneName: "MainMenu"); };
        // When the previous button is clicked, show the previous simulation run data
        prevBtn.clicked += () => { prevButtonHandler(); };
        // When the next button is clicked, show the next simulation run data
        nextBtn.clicked += () => { nextButtonHandler(); };
    }
    // Handler for the previous button; decrement the current run index and update the UI
    private void prevButtonHandler()
    {
        if (currentRun > 0)
        {
            currentRun--;
            clearDataPanel();
            fillDataPanel();
        }
    }
    // Handler for the next button; increment the current run index and update the UI
    private void nextButtonHandler()
    {   // Read the JSON file containing simulation data
        string unparsedJSON = System.IO.File.ReadAllText(Application.dataPath + "/StreamingAssets/" + jsonFile + ".json");
        RootObject<SimulationEntry> parsedJSON = JsonUtility.FromJson<RootObject<SimulationEntry>>(unparsedJSON);
        // If not at the end of the list, increment the current run and update the UI
        if (currentRun < parsedJSON.SIMULATION_DATA.Count - 1)
        {
            currentRun++;
            clearDataPanel();
            fillDataPanel();
        }
    }

    /// Clears all data from the TreeView UI component
    private void clearDataPanel()
    {
        data.hierarchy.ElementAt(0).Clear();
    }

    // Populates the TreeView with the data from the current simulation run
    private void fillDataPanel()
    {
        // Read the JSON file again to refresh the data
        string unparsedJSON = System.IO.File.ReadAllText(Application.dataPath + "/StreamingAssets/" + jsonFile + ".json");
        RootObject<SimulationEntry> parsedJSON = JsonUtility.FromJson<RootObject<SimulationEntry>>(unparsedJSON);
        // Add UI elements to the TreeView for each piece of data in the simulation run
        // For each category (Settings, Random, WallFollow, Spiral, Snaking), add headers and corresponding values
        AddRunNumberLabel("Run Number: "+(currentRun+1));
        // ...
        // (Additional UI elements are added for each section of the simulation data)
        // ...
        AddNewSectionHeader("\nSettings:");
        AddLabelToData("Whiskers: "+parsedJSON.SIMULATION_DATA[currentRun].Settings.whiskers);
        AddLabelToData("Floor Covering: "+parsedJSON.SIMULATION_DATA[currentRun].Settings.floorCovering);
        AddLabelToData("Starting Battery Life: "+parsedJSON.SIMULATION_DATA[currentRun].Settings.batteryLifeStart);

        AddNewSectionHeader("\nRandom:");
        AddLabelToData("Elapsed Time: " + parsedJSON.SIMULATION_DATA[currentRun].Random.elapsedTime);
        AddLabelToData("Ending Battery Life: " + parsedJSON.SIMULATION_DATA[currentRun].Random.batteryLifeEnd);
        AddLabelToData("Cleaning Efficiency: " + parsedJSON.SIMULATION_DATA[currentRun].Random.cleaningEfficiency);
        AddLabelToData("Tiles Cleaned: " + parsedJSON.SIMULATION_DATA[currentRun].Random.tilesCleaned);
        AddLabelToData("Untouched Tiles: " + parsedJSON.SIMULATION_DATA[currentRun].Random.untouchedTiles);


        AddNewSectionHeader("\nWall Follow:");
        AddLabelToData("Elapsed Time: " + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.elapsedTime);
        AddLabelToData("Ending Battery Life: " + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.batteryLifeEnd);
        AddLabelToData("Cleaning Efficiency: " + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.cleaningEfficiency);
        AddLabelToData("Tiles Cleaned: " + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.tilesCleaned);
        AddLabelToData("Untouched Tiles: " + parsedJSON.SIMULATION_DATA[currentRun].WallFollow.untouchedTiles);

        AddNewSectionHeader("\nSpiral:");
        AddLabelToData("Elapsed Time: " + parsedJSON.SIMULATION_DATA[currentRun].Spiral.elapsedTime);
        AddLabelToData("Ending Battery Life: " + parsedJSON.SIMULATION_DATA[currentRun].Spiral.batteryLifeEnd);
        AddLabelToData("Cleaning Efficiency: " + parsedJSON.SIMULATION_DATA[currentRun].Spiral.cleaningEfficiency);
        AddLabelToData("Tiles Cleaned: " + parsedJSON.SIMULATION_DATA[currentRun].Spiral.tilesCleaned);
        AddLabelToData("Untouched Tiles: " + parsedJSON.SIMULATION_DATA[currentRun].Spiral.untouchedTiles);

        AddNewSectionHeader("\nSnaking:");
        AddLabelToData("Elapsed Time: " + parsedJSON.SIMULATION_DATA[currentRun].Snaking.elapsedTime);
        AddLabelToData("Ending Battery Life: " + parsedJSON.SIMULATION_DATA[currentRun].Snaking.batteryLifeEnd);
        AddLabelToData("Cleaning Efficiency: " + parsedJSON.SIMULATION_DATA[currentRun].Snaking.cleaningEfficiency);
        AddLabelToData("Tiles Cleaned: " + parsedJSON.SIMULATION_DATA[currentRun].Snaking.tilesCleaned);
        AddLabelToData("Untouched Tiles: " + parsedJSON.SIMULATION_DATA[currentRun].Snaking.untouchedTiles);
    }
    // Helper method to create and add a label for the run number in the UI
    private void AddRunNumberLabel(string text)
    {
        Label newHeader = new Label(text);
        // Style the label
        newHeader.style.fontSize = 18;
        newHeader.style.color = Color.white;
        newHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
        newHeader.style.alignSelf = Align.Center;
        data.hierarchy.ElementAt(0).Add(newHeader);
    }
    // Helper method to create and add section headers in the UI
    private void AddNewSectionHeader(string text)
    {
        Label newHeader = new Label(text);
        // Style the label
        newHeader.style.fontSize = 16;
        newHeader.style.color = Color.white;
        newHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
        data.hierarchy.ElementAt(0).Add(newHeader);
    }
    // Helper method to create and add general data labels in the UI
    private void AddLabelToData(string text)
    {
        Label newLabel = new Label(text);
        // Style the label
        newLabel.style.fontSize = 12;
        newLabel.style.color = Color.white;
        newLabel.style.marginLeft = 50;
        data.hierarchy.ElementAt(0).Add(newLabel);
    }
}
