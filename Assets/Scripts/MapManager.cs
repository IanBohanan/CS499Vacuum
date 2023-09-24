//This script holds the data of each simulated tile (the ones that detect cleanliness.)
//It should not modify the data, that is the job of cleanlinessManager. 
//This script just gives out data and receives it.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    enum FloorType { HARD, LOOP, CUT, FREEZECUT };

    [SerializeField]
    private Tilemap map;

    [SerializeField]
    private List<TileData> tileDatas;

    private Dictionary<TileBase, TileData> dataFromTiles;

    //Automatically fills the datafromtiles dictionary with data for each tile 
    void Awake()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach(var tileData in tileDatas)
        {
            foreach(var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Debug.Log(gameObject);
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);

            TileBase clickedTile = map.GetTile(gridPosition);

            float tileCleanliness = dataFromTiles[clickedTile].cleanliness;
            
        }
    }

    public float GetTileCleanliness(Vector2 worldPosition)
    {
        Vector3Int gridPosition = map.WorldToCell(worldPosition);

        TileBase tile = map.GetTile(gridPosition);

        //Just in case error checking. If trying to get a nonexistent tile, return a wacky answer
        if (tile == null)
            return -100;

        float tileCleanliness = dataFromTiles[tile].cleanliness;

        return tileCleanliness;
    }

    public bool IsTileOccupied(Vector2 worldPosition)
    {
        Vector3Int gridPosition = map.WorldToCell(worldPosition);
        TileBase tile = map.GetTile(gridPosition);
        bool tileOccupied = dataFromTiles[tile].occupied;
        return tileOccupied;
    }
}
