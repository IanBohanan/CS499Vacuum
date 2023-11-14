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

    public GameObject upperExtender; //The location of the new wall object when extended

    public GameObject lowerExtender; //The location of the wall's pivot point

    public GameObject UI; //The UI of the wall object

    public GameObject wallEndpoint1;
    public GameObject wallEndpoint2;

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

        if (upperExtender.gameObject.GetComponent<WallExtender>().connectedToWall && lowerExtender.gameObject.GetComponent<WallExtender>().connectedToWall && isBeingPlaced)
        {
            //If both upper and lower extender are touching a wall, then declare room closed.
            print("WallPlacer: Room closed!");
            isBeingPlaced = false;
        }
        else
        {
            //This wall is not the final one for the room.
            isBeingPlaced = false;
            GameObject nextWall = Instantiate(wallPrefab, spawnPoint, Quaternion.identity); //Create the new wall object at one of the extenders (may be lower or upper, not guarenteed)
            nextWall.transform.rotation = this.transform.rotation;
            nextWall.GetComponent<WallPlacer>().isBeingPlaced = true; //Enables the wallPlacer for the OBJECT because the unity action turns off the wallPlacer as a global script
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

        this.transform.eulerAngles = newRotation;

    }

    //Triggered whenever the UI state changes
    private void stateUpdated(string newState)
    {
        if (newState == "WallPlacement")
        {
            UI.SetActive(true);
        }
        else //UI updated away from wall placement to somewhere else. Thus the user doesn't actually want this wall object placed!
        {
            disableWallUI();
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

        //Distance from upper position to mouse point
        Vector2 distance = mousePosition - upperExtender.transform.position;

        
        //Make sure mouse of far enough away so wall sensitivty isn't off the charts
        if(distance.magnitude > 10)
        {
            Quaternion rotation = transform.rotation;

            // Convert the quaternion to euler angles for easier comparison
            Vector3 eulerAngles = rotation.eulerAngles;

            //Get upperExtend in relation to lower extend - four possibilities
            /* 1)Wall facing up: rotation 0. Extension occurs if cursor Y > upperExtend.y
             * 2)Wall facing down: rotation 180 Extension occurs if cursor Y < upperExtend.y
             * 3)Wall facing left: rotation 90 Extension occurs if cursor X < upperExtend.X
             * 4)Wall facing right: rotation 270 Extension occurs if cursor X > upperExtend.X
             */
            //Check for each rotation, then see how the cursor is position relative to the upperExtend based on that.


            // Use a switch statement to check for different wall rotations
            switch ((int)eulerAngles.y)
            {
                case 0: //Wall facing up
                    if (mousePosition.y > upperExtender.transform.position.y)
                    {
                        extendWall(upperExtender.transform.position);
                    }
                    break;

                case 180: //wall facing down
                    if (mousePosition.y < upperExtender.transform.position.y)
                    {
                        extendWall(upperExtender.transform.position);
                    }
                    break;

                case 90: //Wall facing left
                    if (mousePosition.x < upperExtender.transform.position.x)
                    {
                        extendWall(upperExtender.transform.position);
                    }
                    break;

                case 270: //wall facing right
                    if (mousePosition.x > upperExtender.transform.position.x)
                    {
                        extendWall(upperExtender.transform.position);
                    }
                    break;
                default:
                    Debug.Log("ERROR: WallPlacer: Wall angle is in a non-clockwise rotation!");
                    break;
            }
        }



    }

    //checks if the mouse cursor is behind the lowerExtend
    //If so, then delete this wall to get closer to the cursor
    private void checkIfCursorBehind()
    {

    }

    private void Start()
    {
        if(isBeingPlaced)
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
            //Auto delete self if cursor is behind the lowerExtend
            checkIfCursorBehind();
        }
    }
}
