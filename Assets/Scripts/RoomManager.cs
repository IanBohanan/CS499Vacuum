//RoomManager script. 
//This script keeps track of the flood fill algorithm and makes sure each room can be reached by the vacuum by flooding the house
//To do so, it gets every flag object (marked with a unique tag) in the scene. It stores each flag instance in a dictionary with whether it was found (or not)
/* Then it floods the house tilemap, marking clear tiles as "explored" in the exploredTiles dictionary.
 *          Note: tiles with anything on them will not be counted as explored (except flags). IE any tile that cannot be explored by the vacuum is not checked.
 * Finally it checks if each of the flags in the foundFlags dictionary was found. If not, it say which flag was not reached by the flood algo.
 * */

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
    private Dictionary<GameObject, bool> foundFlags = new Dictionary<GameObject, bool>(); //Which flags were currently found by the floodFill aglo
    public Dictionary<Vector3Int, bool> exploredTiles = new Dictionary<Vector3Int, bool>(); //Dictionary that matches the positions with whether the tile was explored
    public int activeTiles = 0; //How many tiles are still flooding?
    public bool isFlooding = false; //Is the tilemap currently flooding?

    //Explores a tile, then tries to explore the surrounding tiles (four cardinal directions).
    //Adds all explored tiles to the exploresTiles dictionary.
    //Params: Vector3Int position - the cell position that is being checked
    IEnumerator FloodFill(Vector3Int position)
    {
        activeTiles++;
        // Wait for one second
        yield return new WaitForSeconds(0.5f);
       
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

            Collider2D[] colliders = Physics2D.OverlapPointAll(worldPosition);

            if(colliders.Length > 0)
            {
                foreach (Collider2D collider in colliders)
                {
                    if (collider != null)
                    {
                        //Something is on this tile. Could be a flag, wall, object, etc.

                        //Make sure it is not the room flag we're looking for
                        if (collider.tag != "RoomFlag")
                        {
                            // Now it may be a furniture or wall object.
                            //Check to see if there is a door we can go through
                            if (collider.tag != "DoorBuddy")
                            {
                                //It's not a door! So leave the tile alone
                                activeTiles--;
                                yield break;
                            }
                            else
                            {
                                print("RoomManager: Encountered a door skipping through!");
                                break;
                            }
                        }
                        else //otherwise it is a room flag! So mark flag as found then continue flooding.
                        {
                            foundFlags[collider.gameObject] = true; //Mark the specific flag as found.
                            print("Found " + collider.gameObject.GetComponent<Flag>().roomName);
                            break;
                        }
                    }
                }
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


    //Starts flooding the entire room given a specific startposition
    //(Note: startposition should become a flag's square at some point instead of 0,0)
    //Then finds all the flags in the scene and keeps track of which ones were or weren't found.
    private void beginFlood(Vector3Int startPosition)
    {
        print("RoomManager: Beginning flood.");

        //also get all instances 
        GameObject[] flags = GameObject.FindGameObjectsWithTag("RoomFlag");
        foundFlags.Clear(); //Reset the foundFlags dictionary

        foreach (GameObject flag in flags)
        {
            flag.GetComponent<Flag>().roomName = "Flag " + foundFlags.Count;
            foundFlags.Add(flag, false);
        }

        StartCoroutine(FloodFill(startPosition));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector3Int startPosition = new Vector3Int(0, 0, 0);
            isFlooding = true;
            beginFlood(startPosition);
        }


        if (isFlooding)
        {
            if(activeTiles <= 0)
            {
                activeTiles = 0;
                print("RoomManager: Done flooding!");

                bool unreachableFlag = false;

                //Check to see if any flags were not able to be reached
                foreach (var flag in foundFlags)
                {
                    bool wasFound = flag.Value;
                    if(!wasFound)
                    {
                        GameObject flagObject = flag.Key;
                        print("Was unable to access " + flagObject.GetComponent<Flag>().roomName);
                        unreachableFlag = true;
                    }
                }

                if(!unreachableFlag)
                {
                    print("RoomManager: Found all flags!");
                }
                else
                {
                    print("RoomManager: Did not find all flags!");
                    //IDK do something to prevent going to next scene
                }

                InterSceneManager.houseTiles = debugDictList; //Send final list of floodable tiles to the next scene
                isFlooding = false;
                exploredTiles.Clear(); //reset the dictionary next time we start flooding
            }
        }
    }

}
