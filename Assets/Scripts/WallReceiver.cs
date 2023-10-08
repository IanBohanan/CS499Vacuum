using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Subscribes to event from UI and updates accordingly

public class WallReceiver : MonoBehaviour
{
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

    private void stateUpdated(string newState)
    {
        print("WallReceiver: The new state was set to " + newState);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
