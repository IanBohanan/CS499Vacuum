using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SimulationController : MonoBehaviour
{
    Button playPauseBtn;
    Button fiveTimesSpeedBtn;
    Button fiftyTimesSpeedBtn;
    Button stopSimBtn;
    Label elapsedTimeLabel;
    Label batteryLifeLabel;
    Label speedLabel;

    int speed;

    private void OnEnable()
    {
        // Get UIDocument Root
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

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

        subscribeToCallbacks();
    }
    private void subscribeToCallbacks()
    {
        playPauseBtn.clicked += () => { updateSpeed(0); playPausePressed(); };
        fiveTimesSpeedBtn.clicked += () => updateSpeed(5);
        fiftyTimesSpeedBtn.clicked += () => updateSpeed(50);
        stopSimBtn.clicked += () => endSimulation();
    }

    private void playPausePressed()
    {
        Debug.Log("Play/Pause Pressed");
    }

    private void updateSpeed(int newSpeed)
    {
        if (newSpeed == 0) { // Code 0 given, check if play or pause is active
            if (speed == 0)
            {
                InterSceneManager.speedMultiplier = 1;
                speedLabel.text = "Speed: 1x";
            }
            else
            {
                InterSceneManager.speedMultiplier = 0;
                speedLabel.text = "Speed: 0x";
            }
        }
        else
        {
            InterSceneManager.speedMultiplier = newSpeed;
            speedLabel.text = "Speed: " + newSpeed + "x";
        }
        Debug.Log("Speed set to " + InterSceneManager.speedMultiplier);
        return;
    }

    private void endSimulation()
    {
        SceneManager.LoadScene(sceneName: "MainMenu");
    }
}
