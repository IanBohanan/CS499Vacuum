using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallExtender : MonoBehaviour
{
    public WallPlacer wall; //The wallPlacer script on the prefab wall object

    public bool isLowerExtend; //Is this the lower extender? If so, it spawns the extendWall at an offset

    void OnMouseDown()
    {
        print("Extender: Clicked!");
        wall.extendWall(this.transform.position, isLowerExtend);
    }
}
