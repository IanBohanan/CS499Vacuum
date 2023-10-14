using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SimulationSetupController : MonoBehaviour
{
    Button whiskersButton;
    Slider batteryLifeSlider;
    Button startSimulationBtn;

    // Algorithm Buttons:
    Button randomBtn;
    Button spiralBtn;
    Button snakingBtn;
    Button wallFollowBtn;

    // Dropdown
    DropdownField floorCoveringDropdown;

    // Settings Values:
    bool whiskersEnabled = false;
    int batteryLife = 150;
    string floorCovering = "Hardwood";
    bool randomAlg = false;
    bool spiralAlg = false;
    bool snakingAlg = false;
    bool wallFollowAlg = false;

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
        VisualElement wallFollowContainer = algorithmsCheckboxes.Q<VisualElement>("WallFolow");
        randomBtn = randomContainer.Q<Button>("RandomCheckbox");
        spiralBtn = spiralContainer.Q<Button>("SpiralCheckbox");
        snakingBtn = snakingContainer.Q<Button>("SnakingCheckbox");
        wallFollowBtn = wallFollowContainer.Q<Button>("WallFollowCheckbox");

        //batteryLifeSlider = root.Q<Slider>("BatteryLifeSlider");
        startSimulationBtn = root.Q<Button>("StartButton");

        // Subscribe to button callback functions:
        whiskersButton.clicked += () => { whiskersToggleFunction(); };
        floorCoveringDropdown.RegisterValueChangedCallback(floorCoveringUpdate);

        randomBtn.clicked += () => toggleAlg("random"); 
        spiralBtn.clicked += () => toggleAlg("spiral"); 
        snakingBtn.clicked += () => toggleAlg("snaking"); 
        wallFollowBtn.clicked += () => toggleAlg("wallFollow");
    }

    private void whiskersToggleFunction() // Fixed function declaration
    {
        Debug.Log("Whiskers button pressed");
    }

    private void floorCoveringUpdate(ChangeEvent<string> evt)
    {
        floorCovering = floorCoveringDropdown.value;
        Debug.Log(floorCovering);
    }

    private void toggleAlg(string algName)
    {
        switch (algName)
        {
            case ("random"):
                randomAlg = !randomAlg;
                break;
            case ("spiral"):
                spiralAlg = !spiralAlg;
                break;
            case ("snaking"):
                snakingAlg = !snakingAlg;
                break;
            case ("wallFollow"):
                wallFollowAlg = !wallFollowAlg;
                break;
            default: 
                Debug.Log("Invalid algorithm name string given!");
                break;
        }
    }

    public void onStartSimulationPress()
    {
        // Extract the selected values:

        // TODO: Use the extracted values to setup your simulation


        // Assuming you want to load a new scene after setting up:
        //SceneManager.LoadScene(sceneName: "SimulationScene");
    }
}
