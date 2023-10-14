using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallPlacer : MonoBehaviour
{
    //When selecting the first wall to extend from, all the walls activate their points.
    //When the first point is selected, every other wall should disable their UI since the extension has been chosen.
    //This action is what tells all the other walls that the extension has been chosen.
    public static event Action firstWallSelected; 

    public GameObject wallPrefab; //The wall prefab that will be spawned when this wall is extended.

    public Transform spawner; //The location of the new wall object when extended

    public Transform pivot; //The location of the wall's pivot point

    public GameObject UI; //The UI of the wall object

    public Transform WallTransform; //The wall parent object at the top of the wall prefab hierarchy

    private bool isBeingPlaced = false; //Is the wall the newest one being placed? 

    //When object created (and enabled) subscribe to the UI's stateUpdate action
    private void OnEnable()
    {
        HouseBuilderUI.stateUpdate += stateUpdated;
        WallPlacer.firstWallSelected += disableSpawners;
    }

    //When object created (and enabled) unsubscribe to the UI's stateUpdate action. 
    //Very important or else errors will happen!
    private void OnDisable()
    {
        HouseBuilderUI.stateUpdate -= stateUpdated;
        WallPlacer.firstWallSelected -= disableSpawners;
    }

    public void disableSpawners()
    {
        if(!isBeingPlaced)
        {
            disableWallUI();
        }
    }

    //Disables the UI, leaving just the wall
    public void disableWallUI()
    {
        UI.SetActive(false);
    }

    //Extends the current wall by placing a wall object on the spawner.
    public void extendWall(Vector3 spawnPoint)
    {
        GameObject nextWall = Instantiate(wallPrefab, spawnPoint, Quaternion.identity); //Create the new wall object
        nextWall.transform.rotation = this.transform.rotation;
        nextWall.GetComponent<WallPlacer>().isBeingPlaced = true; //Enables the wallPlacer for the OBJECT because the unity action turns off the wallPlacer as a global script
        disableWallUI();
        isBeingPlaced = false;
        firstWallSelected?.Invoke(); //Tell all the other walls that the main extension point has been selected
    }

    //Updates how the wall should be rotated (in 90 degree increments) based on the cursor's position relative to pivot point
    public void updateRotation()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 distance = mousePosition - pivot.position;

        Vector3 newRotation = this.transform.eulerAngles;

        if (distance.y < -1) //Mouse is far (two grid spaces) below the pivot point. Make it face downward
        {
            newRotation = new Vector3(0, 0, 180);
        }
        if(distance.y > 1)//Mouse is far above the pivot point. Make it face upright
        {
            newRotation = new Vector3(0, 0, 0);
        }

        if(distance.x > 3) //Mouse is far right, make it face right
        {
            newRotation = new Vector3(0, 0, 270);
        }
        if (distance.x < -3) //Mouse is far left, make it face left
        {
            newRotation = new Vector3(0, 0, 90);
        }

        this.transform.eulerAngles = newRotation;

    }

    //Triggered whenever the UI state changes
    private void stateUpdated(string newState)
    {
        if (newState == "WallPlacement")
        {
            UI.SetActive(true);
        }
        else
        {
            disableWallUI();
            if (isBeingPlaced)
            {
                Destroy(transform.root.gameObject); //Delete the acrtual wall gameObject, not just this script
            }
        }
        //print("WallReceiver: The UI's new state was set to " + newState);
    }

    // Update is called once per frame
    void Update()
    {
        if(isBeingPlaced)
            updateRotation();
    }
}
