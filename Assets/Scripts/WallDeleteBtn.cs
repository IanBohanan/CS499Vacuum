using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The delete button on the wall. If clicked, delete the wall object it is attached to


public class WallDeleteBtn : MonoBehaviour
{
    public WallPlacer wall; //The wallPlacer script on the prefab wall object

    void OnMouseDown()
    {
        print("WallDeleteBtn: Delete button clicked!");
        wall.deleteSelf();
    }
}
