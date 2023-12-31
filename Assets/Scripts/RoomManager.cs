//RoomManager script. 
//This script keeps track of the flood fill algorithm and makes sure each room can be reached by the vacuum by flooding the house
//To do so, it gets every flag object (marked with a unique tag) in the scene. It stores each flag instance in a dictionary with whether it was found (or not)
/* Then it floods the house tilemap, marking clear tiles as "explored" in the exploredTiles dictionary.
 * Note: tiles with anything on them will not be counted as explored (except flags). IE any tile that cannot be explored by the vacuum is not checked.
 * Finally it checks if each of the flags in the foundFlags dictionary was found. If not, it say which flag was not reached by the flood algo.
 * */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;


public class RoomManager : MonoBehaviour
{

    public static event Action<bool> finishedFlooding; //Sent out at end of room flooding
    public static event Action<bool> unableToFlood; //Sent out at end of room flooding
    public static event Action spawnedTiles; //Sent out at end of room flooding

    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private Color floodedColor;
    bool foundAllFlags = false;

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
        yield return new WaitForSeconds(0.025f);

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

            if (colliders.Length > 0)
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

    //Changes the color of each tile to represent it has NOT been explored
    private void uncolorTile(Vector3Int position)
    {
        //Change the color of the tile between white(ie. the base image) and tint

        //Okay Unity has some weird debug thing where it has a "lock color" flag for each tile.
        //Whenever setColor is called, ALL unlocked tiles get updated. So we have to unlock then lock each tile individually
        //Sooo just gonna have to unlock that flag for the tile, change the color, then lock the flag AGAIN.
        //Otherwise the entire tilemap gets updated and not just the one tile.
        tilemap.SetTileFlags(position, TileFlags.None);
        tilemap.SetColor(position, Color.white);
        tilemap.SetTileFlags(position, TileFlags.LockColor);
    }


    //Starts flooding the entire room given a specific startposition
    //(Note: startposition should become a flag's square at some point instead of 0,0)
    //Then finds all the flags in the scene and keeps track of which ones were or weren't found.
    public string beginFlood()
    {
        print("RoomManager: Beginning flood.");

        // Reset tile colors from last flood:
        if (InterSceneManager.houseTiles != null)
        {
            foreach (Vector3Int tile in InterSceneManager.houseTiles)
            {
                uncolorTile(tile);
            }
        }

        //also get all instances 
        GameObject[] flags = GameObject.FindGameObjectsWithTag("RoomFlag");
        foundFlags.Clear(); //Reset the foundFlags dictionary
        debugDictList.Clear(); // Reset the touchedTiles list;

        if (flags.Length < 1)
        {
            unableToFlood?.Invoke(true);
        }

        isFlooding = true;

        foreach (GameObject flag in flags)
        {
            flag.GetComponent<Flag>().roomName = "Flag " + foundFlags.Count;
            foundFlags.Add(flag, false);
        }
        // Get vacuum object, disable its boxcollider when we do our thing, and re-enable it when done (in UI-Controller-HouseBuilder.cs). 
        // Also, the flood fill starts from the vacuum's position.
        GameObject vacuum = GameObject.Find("Vacuum-Robot");
        vacuum.GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(FloodFill(tilemap.WorldToCell(vacuum.transform.position)));

        if (foundAllFlags)
        {
            return "Found All";
        }
        else
        {
            return "Didn't Find All";
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {

            Vector3Int startPosition = new Vector3Int(0, 0, 0);
            BeginRoomDimensionCheck();
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
                    foundAllFlags = true;
                    InterSceneManager.houseTiles = debugDictList; //Send final list of floodable tiles to the next scene
                }
                else
                {
                    print("RoomManager: Did not find all flags!");
                    foundAllFlags = false;
                    //IDK do something to prevent going to next scene
                }
                isFlooding = false;
                exploredTiles.Clear(); //reset the dictionary next time we start flooding

                if (foundFlags.Count > 0)
                {
                    finishedFlooding?.Invoke(foundAllFlags);
                }
                else
                {
                    spawnedTiles?.Invoke();
                }
            }
        }
    }


    // Checks the dimensions of every room in a house scheme based on user-placed flags
    // that denote the location of each unique room.
    public bool BeginRoomDimensionCheck()
    {
        bool roomsValid = true;
        // The following vars keep track of how many rooms are too small and too large
        int tooSmall = 0;
        int tooLarge = 0;

        // Keep track of both flag and door objects in the scene.
        GameObject[] flags = GameObject.FindGameObjectsWithTag("RoomFlag");
        
        GameObject[] doors = GameObject.FindGameObjectsWithTag("DoorBuddy");

        // Temporarily set all doors inactive to continue flood
        foreach (GameObject door in doors)
        {
            door.SetActive(false);
        }

        // Flood rooms starting at each user-placed flag.
        foreach (GameObject flag in flags)
        {
            Flag flagObj = flag.GetComponent<Flag>();
            // Get position of flag in world
            Vector3 flagPos = flag.transform.position;
            // Convert flag position to tile position
            Vector3Int cellPosition = tilemap.WorldToCell(flagPos);
            Stack<Vector3Int> stack = new Stack<Vector3Int>();
            stack.Push(cellPosition);

            stack = RoomFlood(stack);
            while (stack.Count > 0)
            {
                RoomFlood(stack);
            }

            if (exploredTiles.Count < 8)
            {
                tooSmall++;
            }
            // REPLACE WITH CURRENT HOUSE SIZE 
            else if (exploredTiles.Count > 16000)
            {
                tooLarge++;
            }

            // Clear explored tiles for subsequent room flood iteration
            exploredTiles.Clear();
        }

        // Reactivate all of the doors in the scene for consistency
        foreach (GameObject door in doors)
        {
            door.SetActive(true);
        }


        // Print how many rooms are too large/small and return if the house scheme is valid
        if (tooLarge > 0)
        {
            Debug.Log(tooLarge + " flagged rooms are too large (must be smaller than current house size).");
            roomsValid = false;
        }

        if (tooSmall > 0)
        {
            Debug.Log(tooSmall + " flagged rooms are too small (must be 8 or more tiles).");
            roomsValid = false;
        }

        return roomsValid;
    }

    public Stack<Vector3Int> RoomFlood(Stack<Vector3Int> posStack)
    {

        while (posStack.Count > 0)
        {
            Vector3Int position = posStack.Pop();

            activeTiles++;

            if (tilemap.HasTile(position))
            {
                //Check if tile is already in dictionary. If yes, it has already been explored and should not be recounted.
                if (exploredTiles.ContainsKey(position))
                {
                    activeTiles--;
                    return posStack;
                }

                Vector3 worldPosition = tilemap.GetCellCenterWorld(position);

                try
                {
                    Collider2D[] colliders = Physics2D.OverlapPointAll(worldPosition);


                    if (colliders.Length > 0)
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
                                        return posStack;
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
                }
                catch (Exception e)
                {
                    return posStack;
                }

                exploredTiles.Add(position, true);
                colorTile(position);

                posStack.Push(position + (1 * Vector3Int.up));
                posStack.Push(position + (1 * Vector3Int.down));
                posStack.Push(position + (1 * Vector3Int.left));
                posStack.Push(position + (1 * Vector3Int.right));
            }
            else
            {
                //print(position + " is outside tilemap.");
            }

            activeTiles--;
        }
        return posStack;
    }

    public bool CheckFrontDoor()
    {
        //GameObject frontDoorChecker = GameObject.Find("FrontDoorChecker Variant");
        bool foundFrontDoor = true;

        // Clear explored tiles before starting, just in case
        exploredTiles.Clear();

       // Vector3Int cellPosition = tilemap.WorldToCell(frontDoorChecker.transform.position);
        //foundFrontDoor = FrontDoorFlood(cellPosition);

        Debug.Log("We found it? " + foundFrontDoor+".");

        // Clear explored tiles for subsequent room flood iteration
        exploredTiles.Clear();

        return foundFrontDoor;
    }

    // Finds whether or not the front door was found
    public bool FrontDoorFlood(Vector3Int position)
    {
        activeTiles++;

        if (!tilemap.HasTile(position))
        {
            activeTiles--;
            return false;
        }

        if (exploredTiles.ContainsKey(position))
        {
            activeTiles--;
            return false;
        }

        Vector3 worldPosition = tilemap.GetCellCenterWorld(position);
        Collider2D[] colliders = Physics2D.OverlapPointAll(worldPosition);

        foreach (Collider2D collider in colliders)
        {
            if (collider != null && collider.CompareTag("FrontDoorBuddy"))
            {
                activeTiles--;
                return true;
            }
        }

        exploredTiles.Add(position, true);
        colorTile(position);

        bool upResult = FrontDoorFlood(position + Vector3Int.up);
        bool downResult = FrontDoorFlood(position + Vector3Int.down);
        bool leftResult = FrontDoorFlood(position + Vector3Int.left);
        bool rightResult = FrontDoorFlood(position + Vector3Int.right);

        bool result = upResult || downResult || leftResult || rightResult;

        activeTiles--;

        return result;
    }


}
