using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Subscribes to stateUpdate from the HouseBuilderUI
//If the user is trying to place a wall, it updates these currently placed walls to allow user to click on the endpoints
//and attach another wall to the end

public class WallReceiver : MonoBehaviour
{

    public GameObject wallUI; //The wallUI gameobject that contains the pivot and spawner

    //When object created (and enabled) subscribe to the UI's stateUpdate action
    private void OnEnable()
    {
        HouseBuilderUI.stateUpdate += stateUpdated;
    }

    //When object created (and enabled) unsubscribe to the UI's stateUpdate action. 
    //Very important or else errors will happen!
    private void OnDisable()
    {
        HouseBuilderUI.stateUpdate -= stateUpdated;
    }

    //Triggered whenever the UI state changes
    private void stateUpdated(string newState)
    {
        if(newState == "WallPlacement")
        {
            wallUI.SetActive(true);
        }
        else
        {
            wallUI.SetActive(false);
        }
        //print("WallReceiver: The UI's new state was set to " + newState);
    }
}
