using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SimulationController : MonoBehaviour
{
    // Define UI elements that will be used in the script
    Button playPauseBtn;
    Button fiveTimesSpeedBtn;
    Button fiftyTimesSpeedBtn;
    Button stopSimBtn;
    Label elapsedTimeLabel;
    Label batteryLifeLabel;
    Label speedLabel;
    System.Timers.Timer timer; // Timer for tracking elapsed time

    // Variables to keep track of the elapsed time
    int elapsedSeconds = 0;
    int elapsedMinutes = 0;
    int elapsedHours = 0;

    int speed = 1; // Current simulation speed

    // References to the vacuum cleaner object and its data
    GameObject vacuumBuddy;
    Vacuum vacuumData;

    // This function is called when the script is enabled
    private void OnEnable()
    {
        // Finding the VacuumBuddy GameObject and its Vacuum component
        vacuumBuddy = GameObject.FindGameObjectWithTag("VacuumBuddy");
        vacuumData = (vacuumBuddy.GetComponent<Vacuum>());

        // Accessing the UI elements from the UIDocument
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Extracting and setting up various UI elements from the UI hierarchy
        VisualElement body = root.Q<VisualElement>("Body");
        VisualElement bottomPanel = body.Q<VisualElement>("BottomPanel");
        VisualElement statusContainer = bottomPanel.Q<VisualElement>("StatusContainer");
        elapsedTimeLabel = statusContainer.Q<Label>("ElapsedTimeLabel");
        batteryLifeLabel = statusContainer.Q<Label>("BatteryLifeLabel");
        VisualElement speedAdjustmentContainer = bottomPanel.Q<VisualElement>("SpeedAdjustmentContainer");
        speedLabel = speedAdjustmentContainer.Q<Label>("SpeedLabel");
        VisualElement buttonContainer = speedAdjustmentContainer.Q<VisualElement>("ButtonContainer");
        playPauseBtn = buttonContainer.Q<Button>("PlayPause");
        fiveTimesSpeedBtn = buttonContainer.Q<Button>("FiveSpeed");
        fiftyTimesSpeedBtn = buttonContainer.Q<Button>("FiftySpeed");
        VisualElement exitButtonContainer = bottomPanel.Q<VisualElement>("ExitButtonContainer");
        stopSimBtn = exitButtonContainer.Q<Button>("ExitButton");

        // Subscribing UI buttons to their respective callback methods
        subscribeToCallbacks();

        // Setting up a repeating timer to update labels every second
        InvokeRepeating("updateLabels", 1, 1);

        // Initializing the speed label with the current speed
        speedLabel.text = "Speed: " + InterSceneManager.speedMultiplier + "x";
    }

    // Updates labels for elapsed time and battery life every second
    private void updateLabels()
    {
        // Update elapsed time and format it for display
        elapsedSeconds += (speed);
        while (elapsedSeconds > 59)
        {
            elapsedSeconds -= 60;
            elapsedMinutes++;
        }
        while (elapsedMinutes > 59)
        {
            elapsedMinutes -= 60;
            elapsedHours++;
        }
        string SecondsString = elapsedSeconds.ToString("D2");
        string MinutesString = elapsedMinutes.ToString("D2");
        string HoursString = elapsedHours.ToString("D2");
        elapsedTimeLabel.text = "Elapsed Time: " + HoursString + ":" + MinutesString + ":" + SecondsString;

        // Update and format battery life for display
        int batteryLifeSeconds = (int)vacuumData.currBatteryLife;
        int batteryLifeMinutes = 0;
        int batteryLifeHours = 0;
        while (batteryLifeSeconds > 59)
        {
            batteryLifeSeconds -= 60;
            batteryLifeMinutes++;
        }
        while (batteryLifeMinutes > 59)
        {
            batteryLifeMinutes -= 60;
            batteryLifeHours++;
        }
        SecondsString = batteryLifeSeconds.ToString("D2");
        MinutesString = batteryLifeMinutes.ToString("D2");
        HoursString = batteryLifeHours.ToString("D2");
        batteryLifeLabel.text = "Battery Life Remaining: " + HoursString + ":" + MinutesString + ":" + SecondsString;
    }

    // Subscribes UI elements to their respective event handlers
    private void subscribeToCallbacks()
    {
        playPauseBtn.clicked += () => { updateSpeed(0); playPausePressed(); };
        fiveTimesSpeedBtn.clicked += () => updateSpeed(5);
        fiftyTimesSpeedBtn.clicked += () => updateSpeed(50);
        stopSimBtn.clicked += () => endSimulation();
    }

    // Handles the Play/Pause button press
    private void playPausePressed()
    {
        Debug.Log("Play/Pause Pressed");
    }

    // Updates the simulation speed based on user input
    private void updateSpeed(int newSpeed)
    {
        if (newSpeed == 0) { // Toggling between play and pause
            if (speed == 0)
            {
                speed = 1;
                InterSceneManager.speedMultiplier = 1;
                speedLabel.text = "Speed: 1x";
            }
            else
            {
                speed = 0;
                InterSceneManager.speedMultiplier = 0;
                speedLabel.text = "Speed: 0x";
            }
        }
        else
        {
            speed = newSpeed;
            InterSceneManager.speedMultiplier = newSpeed;
            speedLabel.text = "Speed: " + newSpeed + "x";
        }
        Debug.Log("Speed set to " + InterSceneManager.speedMultiplier);
        return;
    }
    
    // Ends the simulation and loads a new scene
    private void endSimulation()
    {
        SceneManager.LoadScene(sceneName: "CheckForAlgs");
    }
}
