using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static LayoutManager;

public class SimulationSetupController : MonoBehaviour
{
    Button whiskersButton;
    Slider batteryLifeSlider;
    Button startSimulationBtn;
    Label batteryLifeLabel; // New field for displaying battery life

    Slider robotSpeedSlider;
    Label robotSpeedLabel; // New field for displaying Robot Speed

    Slider vacuumEfficiencySlider;
    Label vacuumEfficiencyLabel; // New field for displaying Robot Speed

    Slider whiskersEfficiencySlider;
    Label whiskersEfficiencyLabel; // New field for displaying Robot Speed


    // Algorithm Buttons:
    Button randomBtn;
    Button spiralBtn;
    Button snakingBtn;
    Button wallFollowBtn;

    // Dropdown
    DropdownField floorCoveringDropdown;

    // Settings Values:
    bool whiskersEnabled = false;
    float batteryLife = 150;
    float robotSpeed = 12;
    float vacuumEfficiency = 90;
    float whiskersEfficiency = 30;
    string floorCovering = "Hardwood";
    bool randomAlg = false;
    bool spiralAlg = false;
    bool snakingAlg = false;
    bool wallFollowAlg = false;

    private (bool, string, int) myData;

    void OnEnable()
    {
        Debug.Log("There are: " + InterSceneManager.houseTiles.Count + " tiles.");
        // Get UIDocument Root:
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Parse UI Doc Tree:
        VisualElement body = root.Q<VisualElement>("Body");
        VisualElement settingsContainer = body.Q<VisualElement>("SettingsContainer");
        // Left Column:
        VisualElement leftColumn = settingsContainer.Q<VisualElement>("LeftColumn");
        VisualElement whiskers = leftColumn.Q<VisualElement>("Whiskers");
        VisualElement whiskersCheckbox = whiskers.Q<VisualElement>("WhiskersCheckbox");
        whiskersButton = whiskersCheckbox.Q<Button>("WhiskerButton");
        VisualElement floorCoveringContainer = leftColumn.Q<VisualElement>("FloorCovering");
        floorCoveringDropdown = floorCoveringContainer.Q<DropdownField>("FloorCoveringDropdown");
        // Right Column:
        VisualElement rightcolumn = settingsContainer.Q<VisualElement>("RightColumn");
        VisualElement algorithms = rightcolumn.Q<VisualElement>("Algorithms");
        VisualElement algorithmsCheckboxes = algorithms.Q<VisualElement>("AlgorithmsCheckboxes");
        VisualElement randomContainer = algorithmsCheckboxes.Q<VisualElement>("Random");
        VisualElement spiralContainer = algorithmsCheckboxes.Q<VisualElement>("Spiral");
        VisualElement snakingContainer = algorithmsCheckboxes.Q<VisualElement>("Snaking");
        VisualElement wallFollowContainer = algorithmsCheckboxes.Q<VisualElement>("WallFollow");
        randomBtn = randomContainer.Q<Button>("RandomCheckbox");
        spiralBtn = spiralContainer.Q<Button>("SpiralCheckbox");
        snakingBtn = snakingContainer.Q<Button>("SnakingCheckbox");
        wallFollowBtn = wallFollowContainer.Q<Button>("WallFollowCheckbox");

        // Battery Life Slider:
        batteryLifeSlider = root.Q<Slider>("BatteryLifeSlider");
        batteryLifeLabel = root.Q<Label>("BatteryLifeLabel"); // Initializing the battery life label
        batteryLifeSlider.value = batteryLife; // Setting the initial slider value
        UpdateBatteryLifeLabel(); // Display initial battery life

        // Robot Speed Slider:
        robotSpeedSlider = root.Q<Slider>("RobotSpeedSlider");
        robotSpeedLabel = root.Q<Label>("RobotSpeedLabel"); // Initializing the Robot Speed label
        robotSpeedSlider.value = robotSpeed; // Setting the initial slider value
        UpdaterobotSpeedLabel(); // Display initial Robot Speed

        // Vacuum Efficiency Slider:
        vacuumEfficiencySlider = root.Q<Slider>("VacuumEfficiencySlider");
        vacuumEfficiencyLabel = root.Q<Label>("VacuumEfficiencyLabel"); // Initializing the Robot Speed label
        vacuumEfficiencySlider.value = vacuumEfficiency; // Setting the initial slider value
        vacuumEfficiencyLabel.text = $"Vacuum Efficiency: {(int)vacuumEfficiency}%"; // Display initial vacuum efficiency

        // Whiskers Efficiency Slider:
        whiskersEfficiencySlider = root.Q<Slider>("WhiskersEfficiencySlider");
        whiskersEfficiencyLabel = root.Q<Label>("WhiskersEfficiencyLabel"); // Initializing the Robot Speed label
        whiskersEfficiencySlider.value = whiskersEfficiency; // Setting the initial slider value
        whiskersEfficiencyLabel.text = $"Whiskers Efficiency: {(int)whiskersEfficiency}%"; // Display initial whiskers efficiency


        startSimulationBtn = root.Q<Button>("StartButton");

        // Set all algorithms as enabled by default:
        toggleAlg("random");
        toggleAlg("spiral");
        toggleAlg("snaking");
        toggleAlg("wallFollow");

        // Subscribe to callback functions:
        whiskersButton.clicked += () => { whiskersToggleFunction(); };
        floorCoveringDropdown.RegisterValueChangedCallback(floorCoveringUpdate);
        batteryLifeSlider.RegisterValueChangedCallback(batteryLifeUpdate);
        robotSpeedSlider.RegisterValueChangedCallback(robotSpeedUpdate);
        vacuumEfficiencySlider.RegisterValueChangedCallback(vacuumEfficiencyUpdate);
        whiskersEfficiencySlider.RegisterValueChangedCallback(whiskersEfficiencyUpdate);
        randomBtn.clicked += () => toggleAlg("random");
        spiralBtn.clicked += () => toggleAlg("spiral");
        snakingBtn.clicked += () => toggleAlg("snaking");
        wallFollowBtn.clicked += () => toggleAlg("wallFollow");
        startSimulationBtn.clicked += () => onStartSimulationPress();
    }

    private void whiskersToggleFunction()
    {
        whiskersEnabled = !whiskersEnabled;
        if (whiskersEnabled) {
            whiskersButton.style.color = Color.black;
        }
        else
        {
            whiskersButton.style.color = new Color(0.7372f, 0.7372f, 0.7372f, 1);
        }
    }

    private void floorCoveringUpdate(ChangeEvent<string> evt)
    {
        floorCovering = floorCoveringDropdown.value;
        switch(floorCovering)
        {
            case "Hardwood":
                vacuumEfficiencySlider.value = 90;
                break;
            case "Loop Pile":
                vacuumEfficiencySlider.value = 75;
                break;
            case "Cut Pile":
                vacuumEfficiencySlider.value = 70;
                break;
            case "Frieze-Cut Pile":
                vacuumEfficiencySlider.value = 65;
                break;
            default:
                Debug.Log("That's not a real floor covering, George. SimulationSetupController.cs");
                break;
        }
    }

    private void batteryLifeUpdate(ChangeEvent<float> evt)
    {
        batteryLife = batteryLifeSlider.value;
        UpdateBatteryLifeLabel(); // Update the label when the slider value changes
    }

    private void robotSpeedUpdate(ChangeEvent<float> evt)
    {
        robotSpeed = robotSpeedSlider.value;
        UpdaterobotSpeedLabel(); // Update the label when the slider value changes
    }

    private void vacuumEfficiencyUpdate(ChangeEvent<float> evt)
    {
        vacuumEfficiency = vacuumEfficiencySlider.value;
        vacuumEfficiencyLabel.text = $"Vacuum Efficiency: {(int)vacuumEfficiency}%"; // Display updated vacuum efficiency
    }

    private void whiskersEfficiencyUpdate(ChangeEvent<float> evt)
    {
        whiskersEfficiency = whiskersEfficiencySlider.value;
        whiskersEfficiencyLabel.text = $"Whiskers Efficiency: {(int)whiskersEfficiency}%"; // Display updated whiskers efficiency
    }

    private void UpdaterobotSpeedLabel()
    {
        robotSpeedLabel.text = $"Robot Speed: {(int)robotSpeed} inch/sec"; // Display updated Robot Speed
    }

    private void UpdateBatteryLifeLabel()
    {
        batteryLifeLabel.text = $"Battery Life: {(int)batteryLife} mins"; // Display updated battery life
    }

    private void toggleAlg(string algName)
    {
        switch (algName)
        {
            case ("random"):
                randomAlg = !randomAlg;
                if (randomAlg)
                {
                    randomBtn.style.color = Color.black;
                }
                else
                {
                    randomBtn.style.color = new Color(0.7372f, 0.7372f, 0.7372f, 1);
                }
                break;
            case ("spiral"):
                spiralAlg = !spiralAlg;
                if (spiralAlg)
                {
                    spiralBtn.style.color = Color.black;
                }
                else
                {
                    spiralBtn.style.color = new Color(0.7372f, 0.7372f, 0.7372f, 1);
                }
                break;
            case ("snaking"):
                snakingAlg = !snakingAlg;
                if (snakingAlg)
                {
                    snakingBtn.style.color = Color.black;
                }
                else
                {
                    snakingBtn.style.color = new Color(0.7372f, 0.7372f, 0.7372f, 1);
                }
                break;
            case ("wallFollow"):
                wallFollowAlg = !wallFollowAlg;
                if (wallFollowAlg)
                {
                    wallFollowBtn.style.color = Color.black;
                }
                else
                {
                    wallFollowBtn.style.color = new Color(0.7372f, 0.7372f, 0.7372f, 1);
                }
                break;
            default:
                Debug.Log("Invalid algorithm name string given!");
                break;
        }

        if ((randomAlg == false) && (spiralAlg == false) && (snakingAlg == false) && (wallFollowAlg == false))
        {
            startSimulationBtn.style.display = DisplayStyle.None;
        }
        else
        {
            startSimulationBtn.style.display = DisplayStyle.Flex;
        }
    }

    public void onStartSimulationPress()
    {
        InterSceneManager.setSimulationSettings(whiskersEnabled, floorCovering, (int)batteryLife, randomAlg, spiralAlg, snakingAlg, wallFollowAlg);
        InterSceneManager.vacuumSpeed = (int)robotSpeedSlider.value;
        InterSceneManager.vacuumEfficiency = (int)vacuumEfficiencySlider.value;
        InterSceneManager.whiskersEfficiency = (int)whiskersEfficiencySlider.value;
        if (!whiskersEnabled) { InterSceneManager.whiskersEfficiency = 0; } // Whiskers have no efficiency if not enabled
        myData = InterSceneManager.getSimulationSettings();

        // Set JSON file entry num for use in all simulation scene runs:
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
        InterSceneManager.JSONEntryNum = parsedJSON.SIMULATION_DATA.Count;
        InterSceneManager.startDateTime = (DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString());

        // We want to load a new scene after setting up:
        SceneManager.LoadScene(sceneName: "Simulation");
    }
}
