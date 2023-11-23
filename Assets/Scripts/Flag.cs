using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Flag : MonoBehaviour
{

    private bool placed = false;

    public string roomName = "Room A";

    //Check to make sure it is triggered
    private void OnMouseDown()
    {
        if(!placed)
        {
            placed = true;
        }
    }
}
