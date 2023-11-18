using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;




public class RoomManager : MonoBehaviour
{

    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private Color floodedColor;

    public List<Vector3Int> debugDictList = new List<Vector3Int>();
    public Dictionary<Vector3Int, bool> exploredTiles = new Dictionary<Vector3Int, bool>(); //Dictionary that matches the positions with whether the tile was explored
    public int activeTiles = 0; //How many tiles are still flooding?
    public bool isFlooding = false; //Is the tilemap currently flooding?

    void Start()
    {
        // Define your starting point for flood fill.
    }

    //Explores a tile, then tries to explore the surrounding tiles (four cardinal directions).
    //Adds all explored tiles to the exploresTiles dictionary.
    //Params: Vector3Int position - the cell position that is being checked
    IEnumerator FloodFill(Vector3Int position)
    {
        // Wait for one second
        yield return new WaitForSeconds(1.0f);
        activeTiles++; //Tell object the tile is now active (and being explored)
        if (tilemap.HasTile(position)) //First check if its a valid position in the tilemap
        {
            //Check if tile is already in dictionary. If yes, it has already been explored and should not be recounted.
            if (exploredTiles.ContainsKey(position))
            {
                activeTiles--;
                yield break;
            }

            // Check if there's a game object at this position in the actual coordinates
            Vector3 worldPosition = tilemap.GetCellCenterWorld(position);

            Collider2D collider = Physics2D.OverlapPoint(worldPosition);

            if (collider != null)
            {
                // Stop inspecting cause furniture is here
                print(position + " had furniture on it.");
                activeTiles--;
                yield break;
            }

            //Add the tile to the dictionary to mark it as explored.
            exploredTiles.Add(position, true);
            debugDictList.Add(position);
            colorTile(position);
            //Mark the tile with a tint to show it has been explored

            //Reminder, the position is per TILE. So go by ones, not by world coordinates.
            //Invoke("LaunchProjectile", 2.0f);
            StartCoroutine(FloodFill(position + (1 * Vector3Int.up)));
            StartCoroutine(FloodFill(position + (1 * Vector3Int.down)));
            StartCoroutine(FloodFill(position + (1 * Vector3Int.left)));
            StartCoroutine(FloodFill(position + (1 * Vector3Int.right)));
            
        }
        else
        {
            //print(position + " is outside tilemap.");
        }
        activeTiles--;
    }

    //Changes the color of each tile to represent it has been explored
    private void colorTile(Vector3Int position)
    {
            //Change the color of the tile between white(ie. the base image) and tint

            //Okay Unity has some weird debug thing where it has a "lock color" flag for each tile.
            //Whenever setColor is called, ALL unlocked tiles get updated. So we have to unlock then lock each tile individually
            //Sooo just gonna have to unlock that flag for the tile, change the color, then lock the flag AGAIN.
            //Otherwise the entire tilemap gets updated and not just the one tile.
            tilemap.SetTileFlags(position, TileFlags.None);
            tilemap.SetColor(position, floodedColor);
            tilemap.SetTileFlags(position, TileFlags.LockColor);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3Int startPosition = new Vector3Int(0, 0, 0);
            isFlooding = true;
            print("RoomManager: Beginning flood.");
            StartCoroutine(FloodFill(startPosition));
            
        }


        if (isFlooding)
        {
            if(activeTiles <= 0)
            {
                activeTiles = 0;
                print("RoomManager: Done flooding!");
                isFlooding = false;
                exploredTiles.Clear(); //reset the dictionary next time we start flooding
            }
        }
    }

}
