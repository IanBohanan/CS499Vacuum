using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallExtender : MonoBehaviour
{
    public WallPlacer wall; //The wallPlacer script on the prefab wall object

    void OnMouseDown()
    {
        wall.extendWall();
    }
}
