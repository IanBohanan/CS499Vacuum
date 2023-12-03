// This script, WallExtender, manages the behavior of wall extenders in a Unity application. Wall extenders are used to extend walls by connecting to other wall endpoints when they collide.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallExtender : MonoBehaviour
{
    public WallPlacer wall; //The wallPlacer script on the prefab wall object

    public bool connectedToWall = false; //Is this extender touching another wall?

    private void Start()
    {
        connectedToWall = false;
    }

    private void OnEnable()
    {
        connectedToWall = false;
    }

    //This function is called when the delete button is clicked. It triggers the deletion of the associated wall object.
    void OnMouseDown()
    {

        wall.extendWall(this.transform.position);
    }

    //This function is called when the wall extender is touching another wall endpoint. It extends the wall.
    void OnTriggerEnter2D(Collider2D collision)
    {
        try
        {
            if(collision.gameObject.name.Equals("WallEndpoint"))
                connectedToWall = true;
        }
        catch (Exception e)
        {
        }
    }
    //This function is called when the wall extender is no longer touching another wall endpoint. It extends the wall.
    void OnTriggerExit2D(Collider2D other)
    {
        connectedToWall = false;
    }
}
