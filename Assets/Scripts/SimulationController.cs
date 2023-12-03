// This script, SimulationController, manages the user interface and functionality for controlling a simulation in a Unity application. It includes features such as play/pause, adjusting simulation speed, displaying elapsed time, and managing battery life.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Timers;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SimulationController : MonoBehaviour
{
    Button playPauseBtn;
    Button fiveTimesSpeedBtn;
    Button twentyFiveTimesSpeedBtn;
    Button fiftyTimesSpeedBtn;
    Button stopSimBtn;
    Label elapsedTimeLabel;
    Label batteryLifeLabel;
    Label speedLabel;
    System.Timers.Timer timer;
    int elapsedSeconds = 0;
    int elapsedMinutes = 0;
    int elapsedHours = 0;

    int speed = 1;

    GameObject vacuumBuddy;
    Vacuum vacuumData;

    private void OnEnable()
    {
        InterSceneManager.coveredTileNum = 0;
        // Get references to the UI elements and vacuum object:
        vacuumBuddy = GameObject.FindGameObjectWithTag("VacuumBuddy");
        vacuumData = (vacuumBuddy.GetComponent<Vacuum>());

        // Get UIDocument Root
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        // Retrieve UI elements:

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
        twentyFiveTimesSpeedBtn = buttonContainer.Q<Button>("TwentyFiveSpeed");
        fiftyTimesSpeedBtn = buttonContainer.Q<Button>("FiftySpeed");
        VisualElement exitButtonContainer = bottomPanel.Q<VisualElement>("ExitButtonContainer");
        stopSimBtn = exitButtonContainer.Q<Button>("ExitButton");
        // Subscribe to button click callbacks:
        subscribeToCallbacks();
        // Start a repeating timer that updates labels every second:

        InvokeRepeating("updateLabels", 1, 1); // Start a repeating timer that fires every second

        speedLabel.text = "Speed: " + InterSceneManager.speedMultiplier + "x"; // Set the initial speed label:

        Vacuum.batteryDead += endSimulation;
    }

    private void updateLabels()
    {
        // Update elapsed time label:
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
        string SecondsString = elapsedSeconds.ToString();
        string MinutesString = elapsedMinutes.ToString();
        string HoursString = elapsedHours.ToString();
        if (SecondsString.Length == 1) { SecondsString = "0"+SecondsString; }
        if (MinutesString.Length == 1) { MinutesString = "0"+ MinutesString; }
        if (HoursString.Length == 1) { HoursString = "0"+ HoursString; }
        elapsedTimeLabel.text = "Elapsed Time: " + HoursString + ":" + MinutesString + ":" + SecondsString;
        // Update battery life label:
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
        SecondsString = batteryLifeSeconds.ToString();
        MinutesString = batteryLifeMinutes.ToString();
        HoursString = batteryLifeHours.ToString();
        if (SecondsString.Length == 1) { SecondsString = "0" + SecondsString; }
        if (MinutesString.Length == 1) { MinutesString = "0" + MinutesString; }
        HoursString = "0" + HoursString;

        batteryLifeLabel.text = "Battery Life Remaining: " + HoursString + ":" + MinutesString + ":" + SecondsString;
    }

    private void subscribeToCallbacks()
    {
        // Subscribe to button click callbacks:
        playPauseBtn.clicked += () => { updateSpeed(0); playPausePressed(); };
        fiveTimesSpeedBtn.clicked += () => updateSpeed(5);
        twentyFiveTimesSpeedBtn.clicked += () => updateSpeed(25);
        fiftyTimesSpeedBtn.clicked += () => updateSpeed(50);
        stopSimBtn.clicked += () => endSimulation();
    }

    private void playPausePressed()
    {
        // Code 0 given, check if play or pause is active
        Debug.Log("Play/Pause Pressed");
    }

    private void updateSpeed(int newSpeed)
    {
        if (newSpeed == 0) { // Code 0 given, check if play or pause is active
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
    
    private void endSimulation()
    {
        // Store elapsed time and ending battery life in InterSceneManager to then be stored in JSON:
        int totalSecondsElapsed = elapsedSeconds + (elapsedMinutes*60) + (elapsedHours*60*60);

        InterSceneManager.algorithmName = vacuumBuddy.GetComponent<VacuumMovement>().currentAlg.ToString();
        InterSceneManager.simulationElapsedSeconds = totalSecondsElapsed;
        InterSceneManager.endingBatteryLifeSeconds = (int)vacuumData.currBatteryLife;
        // Load the "CheckForAlgs" scene when exiting the simulation.
        SceneManager.LoadScene(sceneName: "CheckForAlgs");
        
    }
}
