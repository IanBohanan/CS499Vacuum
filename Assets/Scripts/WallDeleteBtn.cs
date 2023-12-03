// This script, WallDeleteBtn, represents the behavior of a delete button on a wall object in a Unity application. When the delete button is clicked, it triggers the deletion of the associated wall object.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The delete button on the wall. If clicked, delete the wall object it is attached to


public class WallDeleteBtn : MonoBehaviour
{
    //The wallPlacer script on the prefab wall object
    public WallPlacer wall; //The wallPlacer script on the prefab wall object

    void OnMouseDown()
    {
        print("WallDeleteBtn: Delete button clicked!");
        wall.deleteSelf();
    }
}
