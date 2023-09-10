//This script holds the data of each simulated tile (the ones that detect cleanliness.)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap map;

    [SerializeField]
    private List<TileData> tileDatas;

    private Dictionary<TileBase, TileData> dataFromTiles;

    //Automatically fills the datafromtiles dictionary with data for each tile 
    void Awake()
    {
        print("I am awake!");
        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach(var tileData in tileDatas)
        {
            foreach(var tile in tileData.tiles)
            {
                print("Added " + tile + ", " + tileData + " to dictionary");
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);

            TileBase clickedTile = map.GetTile(gridPosition);

           float tileCleanliness = dataFromTiles[clickedTile].cleanliness;

            print("At position " + gridPosition + " there is a " + clickedTile + " with " + tileCleanliness + "% cleanliness.");
        }
    }
}
