using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlacer : MonoBehaviour
{
    public GameObject wallPrefab; //The wall prefab that will be spawned when this wall is extended.

    public Transform spawner; //The location of the new wall object when extended

    public Transform pivot; //The location of the wall's pivot point

    public GameObject UI; //The UI of the wall object

    public Transform WallTransform; //The wall parent object at the top of the wall prefab hierarchy

    //Disables the UI, leaving just the wall
    public void disableWallUI()
    {
        UI.SetActive(false);
    }

    //Extends the current wall by placing a wall object on the spawner.
    public void extendWall()
    {
        GameObject nextWall = Instantiate(wallPrefab, spawner.position, Quaternion.identity); //Create the new wall object
        nextWall.transform.rotation = this.transform.rotation;
        disableWallUI();
        this.enabled = false; //Disable this script since the new wall is the primary one.
    }

    //Updates how the wall should be rotated (in 90 degree increments) based on the cursor's position relative to pivot point
    public void updateRotation()
    {

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 distance = mousePosition - pivot.position;

        Vector3 newRotation = this.transform.eulerAngles;

        //DEBUG DELETE ME LATER
        if (Input.GetKeyDown("o"))
        {
            print(distance);
        }

        if (distance.y < -1) //Mouse is far (two grid spaces) below the pivot point. Make it face downward
        {
            newRotation = new Vector3(0, 0, 180);
            if (Input.GetKeyDown("o"))
            {
                print("WallPlacer: mouse cursor is far below!");
            }
        }
        if(distance.y > 1)//Mouse is far above the pivot point. Make it face upright
        {
            newRotation = new Vector3(0, 0, 0);
            if (Input.GetKeyDown("o"))
            {
                print("WallPlacer: mouse cursor is far above!");
            }
        }

        if(distance.x > 3) //Mouse is far right, make it face right
        {
            newRotation = new Vector3(0, 0, 270);
            if (Input.GetKeyDown("o"))
            {
                print("WallPlacer: mouse cursor is right!");
            }
        }
        if (distance.x < -3) //Mouse is far left, make it face left
        {
            newRotation = new Vector3(0, 0, 90);
            if (Input.GetKeyDown("o"))
            {
                print("WallPlacer: mouse cursor is left!");
            }
        }

        this.transform.eulerAngles = newRotation;

    }

    // Update is called once per frame
    void Update()
    {
        updateRotation();
    }
}
