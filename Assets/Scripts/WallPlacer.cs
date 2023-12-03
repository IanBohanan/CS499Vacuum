// This script, WallPlacer, manages the behavior of wall placement in a Unity application. It handles the placement of wall objects, their rotation, deletion, and connection to other walls.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

public class WallPlacer : MonoBehaviour
{
    //When selecting the first wall to extend from, all the walls activate their points.
    //When the first point is selected, every other wall should disable their UI since the extension has been chosen.
    //This action is what tells all the other walls that the extension has been chosen.
    public static event Action firstWallSelected;

    public GameObject wallPrefab; //The wall prefab that will be spawned when this wall is extended.

    public GameObject upperExtender; //The location of the new wall object when extended

    public Tilemap grid;

    public GameObject lowerExtender; //The location of the wall's pivot point

    public GameObject UI; //The UI of the wall object
    public GameObject deleteBtn; //The big red x to delete the wall.

    public GameObject wallEndpoint1;
    public GameObject wallEndpoint2;

    public GameObject previousWallObject; //The last wall in the "chain" when the player is dragging walls to create
    public WallPlacer previousWallScript; //The script of the last wall in the chain

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

    //Re-enables this wall placer so it can be edited
    public void reEnable()
    {
        upperExtender.gameObject.GetComponent<WallExtender>().connectedToWall = false;
        UI.SetActive(true);
        isBeingPlaced = true;
        wallEndpoint1.SetActive(false);
        wallEndpoint2.SetActive(false);
    }

    //Extends the current wall by placing a wall object on the spawner.
    public void extendWall(Vector3 spawnPoint)
    {

        if (upperExtender.gameObject.GetComponent<WallExtender>().connectedToWall && isBeingPlaced)
        {
            //If both upper and lower extender are touching a wall, then declare room closed.
            print("WallPlacer: Room closed!");
            isBeingPlaced = false;
        }
        else
        {
            //This wall is not the final one for the room.
            isBeingPlaced = false;
            Vector3Int tilePosition = grid.WorldToCell(upperExtender.transform.position); //Convert the position of the next qall's spawn into tilemap coordinates
            Vector3 cellCenter = grid.GetCellCenterWorld(tilePosition); //Get the center of the tile of the extenderPoint. That way the next wall snaps to the center of the tile.
            cellCenter.z = 0;
            GameObject nextWall = Instantiate(wallPrefab, cellCenter, Quaternion.identity); //Create the new wall object in the middle of the next tile (based on the upperExtender's position)
            WallPlacer nextWallScript = nextWall.GetComponent<WallPlacer>();
            nextWall.transform.rotation = this.transform.rotation;
            nextWallScript.isBeingPlaced = true; //Enables the wallPlacer for the OBJECT because the unity action turns off the wallPlacer as a global script
            nextWallScript.previousWallScript = this;
            nextWallScript.previousWallObject = this.gameObject;
            firstWallSelected?.Invoke(); //If this is the first wall of a room, disable all other wall points
        }
        disableWallUI();
        wallEndpoint1.SetActive(true);
        wallEndpoint2.SetActive(true);
    }

    //Updates how the wall should be rotated (in 90 degree increments) based on the cursor's position relative to pivot point
    public void updateRotation()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 distance = mousePosition - lowerExtender.transform.position;

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

        try
        {
            //if new rotation and previous wall rotation has difference of 180, don't rotate! Otherwise it will collide with previous wall!
            float prevWallRotation = previousWallObject.transform.eulerAngles.z;

            //TODO: Fix this so it detects when the previous wall collision occurs
            if (!(Mathf.Abs(newRotation.z - prevWallRotation) == 180))
            {
                this.transform.eulerAngles = newRotation;
            }
            else //Else delete the current wall and go back one in the chain?
            {
                Destroy(transform.root.gameObject); //Delete the acrtual wall gameObject, not just this script
                                                    //Then tell the previous wall in chain that it is now being placed
                previousWallScript.reEnable();
            }
        }
        catch(Exception e)
        {

        }



    }

    //Triggered whenever the UI state changes
    private void stateUpdated(string newState)
    {
        if (newState == "WallPlacement")
        {
            UI.SetActive(true);

            //When the wall button freshly pressed, then allow user to delete the wall.
            deleteBtn.SetActive(true);
        }
        else //UI updated away from wall placement to somewhere else. Thus the user doesn't actually want this wall object placed!
        {
            disableWallUI();
            deleteBtn.SetActive(false);
            if (isBeingPlaced)
            {
                Destroy(transform.root.gameObject); //Delete the acrtual wall gameObject, not just this script
                
            }
        }
    }


    //checks if the mouse cursor is ahead of the upperExtend
    //If so, then auto-extend wall to get closer to the cursor position
    private void checkIfCursorAhead()
    {
        //Convert mouse position to world coordinates
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float wallDistance = (mousePosition - this.transform.position).magnitude;

        //Distance from upper position to mouse point
        float extenderDistance = (mousePosition - upperExtender.transform.position).magnitude;

        //Distance from lower position to mouse point
        float lowerDistance = (mousePosition - lowerExtender.transform.position).magnitude;

        //Make sure mouse of far enough away so wall sensitivty isn't off the charts
        if (wallDistance > 14)
        {
            if (extenderDistance < lowerDistance)
            {
                extendWall(upperExtender.transform.position);
            }
            else
            {
                //Destroy(transform.root.gameObject); //Delete the acrtual wall gameObject, not just this script
                //Then tell the previous wall in chain that it is now being placed
                //previousWallScript.reEnable();
            }
        }
    }

    //Deletes the entire wall object.
    //Triggered when the delete button is pressed
    public void deleteSelf()
    {
        print("WallPlacer: Delete button pressed! Deleting self!");
        Destroy(transform.root.gameObject); //Delete the acrtual wall gameObject, not just this script
    }

    private void Start()
    {
        //Get the tilemap of the housebuilder scene. We can't reference it since it is a prefab.
        try
        {
            grid = GameObject.FindWithTag("GridBuddy").GetComponent<Tilemap>();
        }
        catch(Exception e)
        { }
        
        if (isBeingPlaced)
        {
            wallEndpoint1.SetActive(false);
            wallEndpoint2.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {


        if (isBeingPlaced)
        { 
            //Update the rotation of the wall based on cursor
            updateRotation();
            //Auto place another wall if cursor further than the upperExtend
            checkIfCursorAhead();
        }
    }
}
