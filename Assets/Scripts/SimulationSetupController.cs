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
    Button startSimulationBtn;
    Label batteryLifeLabel; // New field for displaying battery life

    Slider robotSpeedSlider;
    Label robotSpeedLabel; // New field for displaying Robot Speed


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


        startSimulationBtn = root.Q<Button>("StartButton");

        // Subscribe to callback functions:
        whiskersButton.clicked += () => { whiskersToggleFunction(); };
        floorCoveringDropdown.RegisterValueChangedCallback(floorCoveringUpdate);
        batteryLifeSlider.RegisterValueChangedCallback(batteryLifeUpdate);
        robotSpeedSlider.RegisterValueChangedCallback(robotSpeedUpdate);
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

    private void UpdaterobotSpeedLabel()
    {
        robotSpeedLabel.text = $"robotSpeed: {robotSpeed} inch/sec"; // Display updated Robot Speed
    }

    private void UpdateBatteryLifeLabel()
    {
        batteryLifeLabel.text = $"Battery Life: {batteryLife} mins"; // Display updated battery life
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
        InterSceneManager.setSimulationSettings(whiskersEnabled, floorCovering, (int)batteryLife, randomAlg, spiralAlg, snakingAlg, wallFollowAlg);
        myData = InterSceneManager.getSimulationSettings();
        //Debug.Log(myData);
        // We want to load a new scene after setting up:
        SceneManager.LoadScene(sceneName: "Simulation");
    }
}
