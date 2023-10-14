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

    void OnMouseDown()
    {

        wall.extendWall(this.transform.position);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        try
        {
            if(collision.gameObject.name.Equals("WallEndpoint"))
                connectedToWall = true;
            else
            {
                connectedToWall = false;
            }
        }
        catch (Exception e)
        {
            connectedToWall = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        connectedToWall = false;
    }
}
