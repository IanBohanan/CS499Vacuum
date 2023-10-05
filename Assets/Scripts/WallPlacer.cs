using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlacer : MonoBehaviour
{
    public GameObject wallPrefab; //The wall prefab that will be spawned when this wall is extended.

    public Transform spawner; //The location of the new wall object when extended

    public GameObject UI; //The UI of the wall object

    //Disables the UI, leaving just the wall
    public void disableWallUI()
    {
        UI.SetActive(false);
    }

    //Extends the current wall by placing a wall object on the spawner.
    public void extendWall()
    {
        Instantiate(wallPrefab, spawner.position, Quaternion.identity); //Create the new wall object
        disableWallUI();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
