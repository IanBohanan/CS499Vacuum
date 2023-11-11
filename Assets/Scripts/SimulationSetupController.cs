using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SimulationSetupController : MonoBehaviour
{
    Button whiskersButton;
    Slider batteryLifeSlider;

    // Slider and Label for Range of Whiskers and Vacuum
    Slider whiskerRangeSlider; // Field for whisker range
    Slider vacuumRangeSlider;  // Field for vacuum range
    Label whiskerRangeLabel;   // Field for displaying Whisker's Range
    Label vacuumRangeLabel;    // Field for displaying Vacuum's efficiency Range

    Button startSimulationBtn;
    Label batteryLifeLabel; // Field for displaying battery life

    Slider robotSpeedSlider;
    Label robotSpeedLabel; // Field for displaying Robot Speed

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
    float whiskerRange = 10;
    float vacuumRange = 10;
    string floorCovering = "Hardwood";
    bool randomAlg = false;
    bool spiralAlg = false;
    bool snakingAlg = false;
    bool wallFollowAlg = false;

    private (bool, string, int) myData;

    void OnEnable()
    {
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

        batteryLifeSlider = root.Q<Slider>("BatteryLifeSlider");
        batteryLifeLabel = root.Q<Label>("BatteryLifeLabel"); // Initializing the battery life label
        batteryLifeSlider.value = batteryLife; // Setting the initial slider value
        UpdateBatteryLifeLabel(); // Display initial battery life

        // Robot Speed slider
        robotSpeedSlider = root.Q<Slider>("RobotSpeedSlider");
        robotSpeedLabel = root.Q<Label>("RobotSpeedLabel"); // Initializing the Robot Speed label
        robotSpeedSlider.value = robotSpeed; // Setting the initial slider value
        UpdaterobotSpeedLabel(); // Display initial Robot Speed

        // New Slider and Label for Range of Whiskers

        whiskerRangeSlider = root.Q<Slider>("WhiskerRangeSlider");
        whiskerRangeLabel = root.Q<Label>("WhiskerRangeLabel"); // Initializing the Whisker Range label
        whiskerRangeSlider.value = whiskerRange; // Setting the initial slider value
        UpdatewhiskerRangeLabel(); // Display initial Whisker Range

        // New Slider and Label for Range of Vacuum

        vacuumRangeSlider = root.Q<Slider>("VacuumRangeSlider");
        vacuumRangeLabel = root.Q<Label>("VacuumRangeLabel"); // Initializing the Vacuum Range label
        vacuumRangeSlider.value = vacuumRange; // Setting the initial slider value
        UpdatevacuumRangeLabel(); // Display initial Vacuum Range

        whiskerRangeSlider.RegisterValueChangedCallback(whiskerRangeUpdate);
        vacuumRangeSlider.RegisterValueChangedCallback(vacuumRangeUpdate);

        startSimulationBtn = root.Q<Button>("StartButton");

        // Subscribe to callback functions:
        whiskersButton.clicked += () => { whiskersToggleFunction(); };
        floorCoveringDropdown.RegisterValueChangedCallback(floorCoveringUpdate);
        batteryLifeSlider.RegisterValueChangedCallback(batteryLifeUpdate);
        robotSpeedSlider.RegisterValueChangedCallback(robotSpeedUpdate);
        // New Slider and Label for Range of Whiskers and Vacuum
        whiskerRangeSlider.RegisterValueChangedCallback(whiskerRangeUpdate);
        vacuumRangeSlider.RegisterValueChangedCallback(vacuumRangeUpdate);
        randomBtn.clicked += () => toggleAlg("random");
        spiralBtn.clicked += () => toggleAlg("spiral");
        snakingBtn.clicked += () => toggleAlg("snaking");
        wallFollowBtn.clicked += () => toggleAlg("wallFollow");
        startSimulationBtn.clicked += () => onStartSimulationPress();
    }

    void whiskersToggleFunction()
    {
        whiskersEnabled = !whiskersEnabled;
        if (whiskersEnabled)
        {
            whiskersButton.style.color = Color.black;
        }
        else
        {
            whiskersButton.style.color = new Color(0.7372f, 0.7372f, 0.7372f, 1);
        }
    }

    void floorCoveringUpdate(ChangeEvent<string> evt)
    {
        floorCovering = floorCoveringDropdown.value;
    }

    void batteryLifeUpdate(ChangeEvent<float> evt)
    {
        batteryLife = batteryLifeSlider.value;
        UpdateBatteryLifeLabel();
    }

    void robotSpeedUpdate(ChangeEvent<float> evt)
    {
        robotSpeed = robotSpeedSlider.value;
        UpdaterobotSpeedLabel();
    }

    void whiskerRangeUpdate(ChangeEvent<float> evt)
    {
        whiskerRange = whiskerRangeSlider.value;
        UpdatewhiskerRangeLabel();
    }

    void vacuumRangeUpdate(ChangeEvent<float> evt)
    {
        vacuumRange = vacuumRangeSlider.value;
        UpdatevacuumRangeLabel();
    }

    void UpdaterobotSpeedLabel()
    {
        robotSpeedLabel.text = $"robotSpeed: {robotSpeed} inch/sec";
    }

    void UpdateBatteryLifeLabel()
    {
        batteryLifeLabel.text = $"Battery Life: {batteryLife} mins";
    }

    void UpdatewhiskerRangeLabel()
    {
        whiskerRangeLabel.text = $"Whisker Range: {whiskerRange} %";
    }

    void UpdatevacuumRangeLabel()
    {
        vacuumRangeLabel.text = $"Vacuum Range: {vacuumRange} %";
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
    }

    public void onStartSimulationPress()
    {
        InterSceneManager.setSimulationSettings(whiskersEnabled, floorCovering, (int)batteryLife, (int)whiskerRange, (int)vacuumRange, randomAlg, spiralAlg, snakingAlg, wallFollowAlg);
        myData = InterSceneManager.getSimulationSettings();
        SceneManager.LoadScene(sceneName: "Simulation");
    }
}