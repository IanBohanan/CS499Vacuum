using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
// The SimulationController class is responsible for handling the simulation controls,
// such as play, pause, speed adjustment, and exiting the simulation.
public class SimulationController : MonoBehaviour
{
    // UI buttons for controlling the simulation.
    Button playPauseBtn;
    Button fiveTimesSpeedBtn;
    Button fiftyTimesSpeedBtn;
    Button stopSimBtn;
    // UI labels to display the elapsed time, battery life, and current speed of the simulation.
    Label elapsedTimeLabel;
    Label batteryLifeLabel;
    Label speedLabel;
    // Variable to keep track of the simulation speed.
    int speed;
    // OnEnable is called when the object becomes active.
    // It sets up the UI elements by finding them in the scene.
    private void OnEnable()
    {
        // Get the root element of the UIDocument component attached to this GameObject.
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        // Access various parts of the UI hierarchy to find the relevant controls.
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
        // Subscribe to button click events.
        subscribeToCallbacks();
    }
    // subscribeToCallbacks sets up callback methods for the buttons' click events.
    private void subscribeToCallbacks()
    {
        playPauseBtn.clicked += () => { updateSpeed(0); playPausePressed(); };
        fiveTimesSpeedBtn.clicked += () => updateSpeed(5);
        fiftyTimesSpeedBtn.clicked += () => updateSpeed(50);
        stopSimBtn.clicked += () => endSimulation();
    }
    // playPausePressed handles the logic when the play/pause button is pressed.
    private void playPausePressed()
    {
        Debug.Log("Play/Pause Pressed");
    }
    // updateSpeed changes the simulation speed based on the button pressed.
    // If the new speed is 0, it toggles between play and pause.
    private void updateSpeed(int newSpeed)
    {
        if (newSpeed == 0) { // If code 0 is given, check if play or pause is active.
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
    // endSimulation handles the process to end the simulation and return to the main menu.
    private void endSimulation()
    {
        SceneManager.LoadScene(sceneName: "MainMenu");
    }
}
