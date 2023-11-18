using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The WallReceiver class subscribes to state updates from the HouseBuilderUI.
// It enables or disables the wall UI based on whether the user is in wall placement mode.
public class WallReceiver : MonoBehaviour
{
    public GameObject wallUI; // The UI GameObject that contains the pivot and spawner for wall placement.

    // Subscribe to the HouseBuilderUI's stateUpdate action when the GameObject is enabled.
    // This is important for dynamically updating the UI based on the user's current action in the house builder.
    private void OnEnable()
    {
        HouseBuilderUI.stateUpdate += stateUpdated;
    }

    // Unsubscribe from the HouseBuilderUI's stateUpdate action when the GameObject is disabled.
    // This is crucial to prevent errors due to trying to update UI on a disabled or destroyed object.
    private void OnDisable()
    {
        HouseBuilderUI.stateUpdate -= stateUpdated;
    }

    // This method is triggered whenever the UI state changes.
    // It activates or deactivates the wall UI based on the current state of the UI, particularly checking for "WallPlacement" state.
    private void stateUpdated(string newState)
    {
        // Enable the wall UI if the new state is WallPlacement, otherwise disable it.
        if (newState == "WallPlacement")
        {
            wallUI.SetActive(true);
        }
        else
        {
            wallUI.SetActive(false);
        }
        // Uncomment the below line for debugging purposes to print the current state.
        //print("WallReceiver: The UI's new state was set to " + newState);
    }
}