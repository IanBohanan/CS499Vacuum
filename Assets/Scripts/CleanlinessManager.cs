//This script keeps track of which tile is clean and which is dirty.
//It change change the cleanliness and also the color of the tile.


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CleanlinessManager : MonoBehaviour
{

    public float debugCleanAmount;

    [SerializeField]
    private Tilemap floorTiles;

    private Dictionary<Vector3Int, float> tileData = new Dictionary<Vector3Int, float>(); //Dictionary that matches the positions with tile cleanliness.



    [SerializeField]
    private Color cleanColor;

    //The outside-facing API that takes in the game world position and changes how clean a tile is.
    public void makeClean(Vector2 worldPosition, float cleanAmount)
    {
        Vector3Int gridPosition = floorTiles.WorldToCell(worldPosition);
        
        changeCleanliness(gridPosition, cleanAmount);
        VisualizeCleanliness();
    }

    //Changes the cleanliness of a tile given its position and how much to modify it.
    //Also ensures each tile will be between 0 and 100% clean.
    //Params: Vector3Int gridPosition - the grid's X,Y,and Z values.
    //        float toChange -amount that will be added to the the tile's cleanliness value
    private void changeCleanliness(Vector3Int gridPosition, float toChange)
    {
        //Check if tile is already in dictionary. If not, add the tile to dictionary and make it completely dirty
        if (!tileData.ContainsKey(gridPosition))
            tileData.Add(gridPosition, 0f);

        float newValue = tileData[gridPosition] + toChange;

        //Set the tile to the new value. Note: Grid tiles cannot go above 100% clean. 
        //The Mathf "clamp" function will prevent tile from going over value of 100
        tileData[gridPosition] = Mathf.Clamp(newValue, 0f, 100f);

        print("CleanlinessManager: Tile cleanliness: " + tileData[gridPosition]);
    }

    //Changes the color of each tile to represent how clean they are
    private void VisualizeCleanliness()
    {
        foreach(var tile in tileData)
        {
            //Change the color of the tile between white(ie. the base image) and pure green.
            Color newTileColor = Color.Lerp(Color.white, Color.green, tile.Value/100);
            

            //Okay Unity has some weird debug thing where it has a "lock color" flag for each tile.
            //Whenever setColor is called, ALL unlocked tiles get updated. So we have to unlock then lock each tile individually
            //Sooo just gonna have to unlock that flag for the tile, change the color, then lock the flag AGAIN.
            //Otherwise the entire tilemap gets updated and not just the one tile.
            floorTiles.SetTileFlags(tile.Key, TileFlags.None);
            floorTiles.SetColor(tile.Key,newTileColor);
            floorTiles.SetTileFlags(tile.Key, TileFlags.LockColor);
        }
    }

    //Debug function that changes cleanliness of tile just by clicking on it
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            makeClean(mousePosition, debugCleanAmount);
        }

    }
}
