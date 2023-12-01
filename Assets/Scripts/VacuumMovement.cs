using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class VacuumMovement : MonoBehaviour
{
    Tilemap tilemap;

    [SerializeField] GameObject hardwoodTile;
    public enum Algorithm
    {
        Random = 0,
        WallFollow = 1,
        Spiral = 2,
        Snaking = 3,
        BadAlg = -1
    }
    public Algorithm currentAlg;

    enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    // Keep track of all objects in robot vacuum prefab (Consider Serialized Fields?)
    private Transform robotTransform;
    private Transform vacuumTransform;
    private Transform whiskersTransform;

    RandomWalk randomAlg = new RandomWalk();
    WallFollow wallFollow = new WallFollow();
    bool wallFollowing = false;
    bool justTurned = false;
    bool currentlyTurningLeft = false;
    Vector3 targetPositionA;
    bool passedA = false;
    Vector3 targetPositionB;
    bool passedB = false;

    Spiral spiral = new Spiral();
    Vector3 spiralOrigin = Vector3.zero;
    bool isSpiraling = false;
    float spiralStartTime = 0f;

    bool collidedBefore = false;
    bool snakingHorizontalWalls = false;
    string snakingOffsetDirection = "up";
    bool currentlyOffsettingSnake = false;
    Vector3 postOffsetSnakeDirection = Vector3.zero;

    public Vector2 currentDirectionVec; // The normalized direction vector to tell the vacuum where to go

    private float speed;    // Speed of the vacuum object
    int counter;        
    bool canCollide = true;    // Helps prevent Vacuum phasing through objects

    private Dictionary<string, bool> pathingDict = new Dictionary<string, bool>();

    // Coroutine to prevent Vacuum passing through walls
    private IEnumerator DelayCollisionEnable(float delay)
    {
        // Set a timer until we can collide again
        yield return new WaitForSeconds(delay);
        canCollide = true;
    }

    //Changes the color of each the color matching the selected floor covering:
    private void colorTile(Vector3Int position)
    {
        //Change the color of the tile between white(ie. the base image) and tint

        //Okay Unity has some weird debug thing where it has a "lock color" flag for each tile.
        //Whenever setColor is called, ALL unlocked tiles get updated. So we have to unlock then lock each tile individually
        //Sooo just gonna have to unlock that flag for the tile, change the color, then lock the flag AGAIN.
        //Otherwise the entire tilemap gets updated and not just the one tile.
        tilemap.SetTileFlags(position, TileFlags.None);
        if (InterSceneManager.floorCovering == "Hardwood")
        {
            tilemap.SetColor(position, new UnityEngine.Color(150,75,0,255));
        }
        else if (InterSceneManager.floorCovering == "Loop Pile")
        {
            tilemap.SetColor(position, Color.blue);
        }
        else if (InterSceneManager.floorCovering == "Cut Pile")
        {
            tilemap.SetColor(position, Color.magenta);
        }
        else if (InterSceneManager.floorCovering == "Frieze-Cut Pile")
        {
            tilemap.SetColor(position, Color.cyan);
        }
        else
        {
            Debug.Log("You've lied to me, George. VacuumMovement.cs->ColorTile");
        }
        tilemap.SetTileFlags(position, TileFlags.LockColor);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set up tilemap data:
        tilemap = GameObject.Find("UIFloor").GetComponent<Tilemap>();
        InterSceneManager.cleanedTiles.Clear();
        foreach (Vector3Int tile in InterSceneManager.houseTiles)
        {
            // Create new "CleanedTiles" list with associated hit counters:
            SerializableTile newTile = new SerializableTile(tile, 0);
            InterSceneManager.cleanedTiles.Add(newTile);
            // Color the tiles in the simulation scene to match the floor covering selected:
            colorTile(tile);
            //Instantiate(hardwoodTile, tilemap.CellToWorld(tile), Quaternion.identity);
        }

        // Set Speed
        speed = 0.005f;

        counter = 0;

        (pathingDict["random"], pathingDict["spiral"], pathingDict["snaking"], 
            pathingDict["wallfollow"]) = InterSceneManager.getPathAlgs();

        pathingDict = initialAlgCheck(pathingDict);

        // Get Starting Vector for Pathing Alg, save dir globally
        currentAlg = getNextAlg();

        // Find the child GameObjects by their names.
        robotTransform = transform.Find("Robot");
        vacuumTransform = transform.Find("Vacuum");
        whiskersTransform = transform.Find("Whiskers");

        // Pathing Algorithm Setup
        if (currentAlg == Algorithm.Random)
        {
            //currentDirectionVec = new Vector2(-1, 0);
            currentDirectionVec = randomAlg.getStartingVec();
        }
        else if (currentAlg == Algorithm.WallFollow)
        {
            currentDirectionVec = new Vector2(0.1f, 0.9f); //wallFollow.getStartingVec();
        }
        else if (currentAlg == Algorithm.Spiral)
        {
            currentDirectionVec = new Vector2(1, 0); //spiral.getStartingVec();
        }
        else if (currentAlg == Algorithm.Snaking)
        {
            currentDirectionVec = randomAlg.getStartingVec();
        }
        else // Invalid algorithm, let's move on to data review
        {
            SceneManager.LoadScene(sceneName: "DataReview");
        }
/*
        RaycastHit2D hitDataLeft = Physics2D.Raycast(transform.position, -transform.right);
        Debug.Log("First: " + hitDataLeft.distance);
        transform.rotation = Quaternion.Euler(0, 0, -90);
        hitDataLeft = Physics2D.Raycast(transform.position, -transform.right);
        Debug.Log("Then: " + hitDataLeft.distance);*/
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        counter++;

        if (currentAlg == Algorithm.Random)
        {
            //Debug.Log("Random");
        }
        else if (currentAlg == Algorithm.WallFollow)
        {
            Vector3 botBottom = new Vector3(transform.position.x, transform.position.y + 6, transform.position.z);
            RaycastHit2D hitDataLeft = Physics2D.Raycast(transform.position, -transform.right);
            if ((!justTurned) && hitDataLeft.distance > 10)
            {
                if (wallFollowing)
                {
                    //speed = 0;
                    //Debug.Log(hitDataLeft.distance);
                    float x = (currentDirectionVec.x * 8);
                    float y = (currentDirectionVec.y * 8);
                    targetPositionA = (transform.position + (new Vector3(x, y, 0)));
                    x = -(currentDirectionVec.y); 
                    y = (currentDirectionVec.x);
                    targetPositionB = (targetPositionA + (new Vector3(x, y, 0)));

                    justTurned = true;
                }
            }
            else if (justTurned)
            {
                if ((!passedA) && (Vector3.Distance(targetPositionA, transform.position) < 3)){
                    Debug.Log("passed A");
                    transform.position = targetPositionA;
                    passedA = true;
                    currentDirectionVec = wallFollow.turnLeft(currentDirectionVec);
                    if (currentDirectionVec == new Vector2(1, 0))
                    {
                        Debug.Log("Right");
                        // Rotate to face right (positive x)
                        transform.rotation = Quaternion.Euler(0, 0, -90);
                    }
                    else if (currentDirectionVec == new Vector2(0, 1))
                    {
                        Debug.Log("up");
                        // Rotate to face upwards (positive y)
                        transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    else if (currentDirectionVec == new Vector2(-1, 0))
                    {
                        Debug.Log("Left");
                        // Rotate to face left (negative y)
                        transform.rotation = Quaternion.Euler(0, 0, 90);
                    }
                    else if (currentDirectionVec == new Vector2(0, -1))
                    {
                        Debug.Log("down");
                        // Rotate to face downwards (negative y)
                        transform.rotation = Quaternion.Euler(0, 0, 180);
                    }
                }
                else if ((!passedB) && (Vector3.Distance(targetPositionB, transform.position) < 3))
                {
                    Debug.Log("passed B");
                    transform.position = targetPositionB;
                    passedB = true;
                }
                if (passedA && passedB)
                {
                    Debug.Log("passed both");
                    justTurned = false;
                    passedA = false;
                    passedB = false;
                }
            }
        }
        else if (currentAlg == Algorithm.Spiral)
        {
            if (isSpiraling) {
                //(Vector2 newVec, Quaternion newQuat) = spiral.getNewSpiralVec(currentDirectionVec, transform.rotation);
                //float distance = (Vector3.Distance(transform.position, spiralOrigin))/100;
                //transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.eulerAngles.z + (0.5f-distance));
                //currentDirectionVec = newVec;}
                float angle = (Time.time - spiralStartTime) * (InterSceneManager.speedMultiplier); // replace 1 with the whatever speed
                angle = angle / 5;
                float radius = 1 + angle; // 1 is the radius

                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;

                Vector3 newPosition = spiralOrigin + new Vector3(x, y, 0);

                transform.position = newPosition;
            }
            else
            {
                bool enoughSpace = true;
                RaycastHit2D hitDataLeft = Physics2D.Raycast(transform.position, -transform.right);
                RaycastHit2D hitDataRight = Physics2D.Raycast(transform.position, transform.right);
                RaycastHit2D hitDataUp = Physics2D.Raycast(transform.position, transform.up);
                RaycastHit2D hitDataDown = Physics2D.Raycast(transform.position, -transform.up);
                if ((hitDataLeft.distance < 20) && (hitDataLeft.collider)) { enoughSpace = false; }
                if ((hitDataRight.distance < 20) && (hitDataRight.collider)) { enoughSpace = false; }
                if ((hitDataUp.distance < 20) && (hitDataUp.collider)) { enoughSpace = false; }
                if ((hitDataDown.distance < 20) && (hitDataDown.collider)) { enoughSpace = false; }

                if (enoughSpace) // We have enough space in all directions to start the spiraling algorithm
                { 
                    isSpiraling = true; 
                    spiralOrigin = transform.position;
                    spiralStartTime = Time.time;
                } 
            }
        }
        else if (currentAlg == Algorithm.Snaking)
        {
            if (currentlyOffsettingSnake && (Vector3.Distance(targetPositionA, transform.position) < 1))
            {
                transform.position = targetPositionA;
                currentlyOffsettingSnake = false;
                if ((postOffsetSnakeDirection.x ==1) || (postOffsetSnakeDirection.x == -1) || (postOffsetSnakeDirection.x == 0))
                {
                    postOffsetSnakeDirection = RotateVector45Degrees(postOffsetSnakeDirection);
                }
                currentDirectionVec = postOffsetSnakeDirection;
                //Debug.Log(postOffsetSnakeDirection);
            }
            //Debug.Log("Snaking");
        }

        // Move the entire "Vacuum-Robot" prefab.
        // Calculate next Vacuum move
        Vector3 movePosition = new Vector3(currentDirectionVec.x , currentDirectionVec.y, 0) * speed * InterSceneManager.speedMultiplier * (InterSceneManager.vacuumSpeed/3);
        // Move the child GameObjects along with the parent.
        transform.position += movePosition;

        // Figure out which tile in the tilemap we're currently over:
        Vector3Int cellPosition = tilemap.WorldToCell(transform.position);

        if (tilemap.cellBounds.Contains(cellPosition))
        {
            int tileIndex = InterSceneManager.houseTiles.IndexOf(cellPosition);
            if (tileIndex != -1)
            {
                //Debug.Log(tileIndex + " " + cellPosition.x + " " + cellPosition.y);
                InterSceneManager.cleanedTiles[tileIndex].hits += InterSceneManager.speedMultiplier;
            }
        }
        Vector3Int newcellPosition = new Vector3Int(cellPosition.x-1, cellPosition.y, cellPosition.z);
        if (tilemap.cellBounds.Contains(newcellPosition))
        {
            int tileIndex = InterSceneManager.houseTiles.IndexOf(newcellPosition);
            if (tileIndex != -1)
            {
                InterSceneManager.cleanedTiles[tileIndex].hits += Mathf.RoundToInt(InterSceneManager.speedMultiplier/2);
            }
        }
        newcellPosition = new Vector3Int(cellPosition.x + 1, cellPosition.y, cellPosition.z);
        if (tilemap.cellBounds.Contains(newcellPosition))
        {
            int tileIndex = InterSceneManager.houseTiles.IndexOf(newcellPosition);
            if (tileIndex != -1)
            {
                InterSceneManager.cleanedTiles[tileIndex].hits += Mathf.RoundToInt(InterSceneManager.speedMultiplier / 2);
            }
        }
        newcellPosition = new Vector3Int(cellPosition.x, cellPosition.y-1, cellPosition.z);
        if (tilemap.cellBounds.Contains(newcellPosition))
        {
            int tileIndex = InterSceneManager.houseTiles.IndexOf(newcellPosition);
            if (tileIndex != -1)
            {
                InterSceneManager.cleanedTiles[tileIndex].hits += Mathf.RoundToInt(InterSceneManager.speedMultiplier / 2);
            }
        }
        newcellPosition = new Vector3Int(cellPosition.x, cellPosition.y+1, cellPosition.z);
        if (tilemap.cellBounds.Contains(newcellPosition))
        {
            int tileIndex = InterSceneManager.houseTiles.IndexOf(newcellPosition);
            if (tileIndex != -1)
            {
                InterSceneManager.cleanedTiles[tileIndex].hits += Mathf.RoundToInt(InterSceneManager.speedMultiplier / 2);
            }
        }
        //---------------------------------
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cast rays in cardinal directions to determine more collision info
        RaycastHit2D hitDataLeft = Physics2D.Raycast(transform.position, -transform.right);
        RaycastHit2D hitDataRight = Physics2D.Raycast(transform.position, transform.right);
        RaycastHit2D hitDataUp = Physics2D.Raycast(transform.position, transform.up);
        RaycastHit2D hitDataDown = Physics2D.Raycast(transform.position, -transform.up);

        // Initialize Array 
        RaycastHit2D[] hitData = new RaycastHit2D[] { hitDataLeft, hitDataRight, hitDataUp, hitDataDown };

        if (canCollide)
        {
            if (currentAlg == Algorithm.Random)
            {
/*                RaycastHit2D shortestRay = hitData[0];
                int indexOfShortestRay = 0;
                for (int i = 0; i < hitData.Length; i++)
                {
                    RaycastHit2D compareTo = hitData[i];
                    if (compareTo.distance < shortestRay.distance)
                    {
                        shortestRay = compareTo;
                        indexOfShortestRay = i;
                    }

                }*/

                //Direction closestDir = (Direction)indexOfShortestRay;

                Vector2 collisionDir = new Vector2(0, 0);
/*                if (closestDir == Direction.Up)
                {
                    collisionDir = new Vector2(0, 1);
                } 
                else if (closestDir == Direction.Down)
                {
                    collisionDir = new Vector2(0, -1);
                }
                else if (closestDir == Direction.Right)
                {
                    collisionDir = new Vector2(1, 0);
                }
                else if (closestDir == Direction.Left)
                {
                    collisionDir = new Vector2(-1, 0);
                }*/

                //--------------------------------------------------------
                string closestName = "left";
                float closest = hitDataLeft.distance; //new Vector2(1, 0);

                if (closest > hitDataRight.distance)
                {
                    closest = hitDataRight.distance;
                    closestName = "right";
                }
                if (closest > hitDataUp.distance)
                {
                    closest = hitDataUp.distance;
                    closestName = "up";
                }
                if (closest > hitDataDown.distance)
                {
                    closest = hitDataDown.distance;
                    closestName = "down";
                }

                switch (closestName)
                {
                    case "left":
                        Debug.Log("left");
                        collisionDir = new Vector2(-1, 0);
                        break;
                    case "right":
                        Debug.Log("right");
                        collisionDir = new Vector2(1, 0);
                        break;
                    case "up":
                        Debug.Log("up");
                        collisionDir = new Vector2(0, 1);
                        break;
                    case "down":
                        Debug.Log("down");
                        collisionDir = new Vector2(0, -1);
                        break;
                    default:
                        Debug.Log("Uh oh...");
                        break;
                }
                //--------------------------------------------------------
                /*
                                ColliderDistance2D dis = GetComponent<Rigidbody2D>().Distance(collision);*/

                currentDirectionVec = -currentDirectionVec;

                float x = currentDirectionVec.x * 0.25f;
                float y = currentDirectionVec.y * 0.25f;

                currentDirectionVec = -currentDirectionVec;

                transform.position += new Vector3(x, y, 0);



                bool newDir = true;
                while (newDir)
                {
                    newDir = false;
                    currentDirectionVec = randomAlg.getStartingVec();
                    Debug.Log(currentDirectionVec);
                    if (currentDirectionVec.y > 0)
                    {
                        if (hitDataUp.distance < 1){
                            newDir = true;
                        }
                    }
                    if (currentDirectionVec.x > 0)
                    {
                        if (hitDataRight.distance < 1)
                        {
                            newDir = true;
                        }
                    }
                    if (currentDirectionVec.y < 0)
                    {
                        if (hitDataDown.distance < 1)
                        {
                            newDir = true;
                        }
                    }
                    if (currentDirectionVec.x < 0)
                    {
                        if (hitDataLeft.distance < 1)
                        {
                            newDir = true;
                        }
                    }
                }

            } 
            else if (currentAlg == Algorithm.WallFollow)
            {
                justTurned = false;
                currentDirectionVec = -currentDirectionVec;
                float x = currentDirectionVec.x * 0.5f;
                float y = currentDirectionVec.y * 0.5f;
                transform.position += new Vector3(x, y, 0);
                currentDirectionVec = -(currentDirectionVec);

                if (wallFollowing) {
                    Debug.Log("HERE: " + collision.name);
                    currentDirectionVec = wallFollow.turnRight(currentDirectionVec); 
                }
                else { currentDirectionVec = wallFollow.getFirstCollisionVec(currentDirectionVec, true); wallFollowing = true; }

                if (currentDirectionVec == new Vector2(1, 0))
                {
                    Debug.Log("Right");
                    // Rotate to face right (positive x)
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                }
                else if (currentDirectionVec == new Vector2(0, 1))
                {
                    Debug.Log("up");
                    // Rotate to face upwards (positive y)
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (currentDirectionVec == new Vector2(-1, 0))
                {
                    Debug.Log("Left");
                    // Rotate to face left (negative y)
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                else if (currentDirectionVec == new Vector2(0, -1))
                {
                    Debug.Log("down");
                    // Rotate to face downwards (negative y)
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                }
            }
            else if (currentAlg == Algorithm.Spiral)
            {
                isSpiraling = false; // Stop the spiral updates
                currentDirectionVec = -currentDirectionVec;
                float x = currentDirectionVec.x * 0.5f;
                float y = currentDirectionVec.y * 0.5f;
                transform.position += new Vector3(x, y, 0);
                currentDirectionVec = -(currentDirectionVec);
                currentDirectionVec = randomAlg.getStartingVec();
            }
            else if (currentAlg == Algorithm.Snaking)
            {
                currentlyOffsettingSnake = true;

                // Back up a bit:
                currentDirectionVec = -currentDirectionVec;
                float x = currentDirectionVec.x * 0.5f;
                float y = currentDirectionVec.y * 0.5f;
                transform.position += new Vector3(x, y, 0);
                currentDirectionVec = -(currentDirectionVec);

                float minDistance;
                if (collidedBefore == false) // First collision, determine what wall type we've hit
                {
                    collidedBefore = true; // Don't run this block again
                    minDistance = Mathf.Min(hitDataLeft.distance, hitDataRight.distance, hitDataUp.distance, hitDataDown.distance); // Get mininum distance
                    if (minDistance == hitDataLeft.distance)
                    {
                        snakingHorizontalWalls = false;
                        snakingOffsetDirection = "up";
                    }
                    else if (minDistance == hitDataRight.distance)
                    {
                        snakingHorizontalWalls = false;
                        snakingOffsetDirection = "down";
                    }
                    else if (minDistance == hitDataUp.distance)
                    {
                        snakingHorizontalWalls = true;
                        snakingOffsetDirection = "right";
                    }
                    else // minDistance == hitDataDown.distance
                    {
                        snakingHorizontalWalls = true;
                        snakingOffsetDirection = "left";
                    }
                    Debug.Log("Snaking Horizontal: "+snakingHorizontalWalls);
                }
                //---------------------------------------
                minDistance = Mathf.Min(hitDataLeft.distance, hitDataRight.distance, hitDataUp.distance, hitDataDown.distance); // Get mininum distance
                bool horizontalWallBounce;
                if (minDistance == hitDataLeft.distance)
                {
                    horizontalWallBounce = false;
                }
                else if (minDistance == hitDataRight.distance)
                {
                    horizontalWallBounce = false;
                }
                else if (minDistance == hitDataUp.distance)
                {
                    horizontalWallBounce = true;
                }
                else // minDistance == hitDataDown.distance
                {
                    horizontalWallBounce = true;
                }
                if (horizontalWallBounce != snakingHorizontalWalls) // We've hit a different wall type, so rotate angle of movement 90 degrees
                {
                    snakingHorizontalWalls = !snakingHorizontalWalls;
                    if (currentDirectionVec.x < 0)
                    {
                        currentDirectionVec = new Vector2(currentDirectionVec.y, -currentDirectionVec.x);
                    }
                    else if (currentDirectionVec.x > 0)
                    {
                        currentDirectionVec = new Vector2(-currentDirectionVec.y, currentDirectionVec.x);
                    }
                    else if (currentDirectionVec.y < 0)
                    {
                        currentDirectionVec = new Vector2(currentDirectionVec.y, -currentDirectionVec.x);
                    }
                    else if (currentDirectionVec.y > 0)
                    {
                        currentDirectionVec = new Vector2(-currentDirectionVec.y, currentDirectionVec.x);
                    }

                    if (snakingOffsetDirection == "up")
                    {
                        snakingOffsetDirection = "right";
                    }
                    else if (snakingOffsetDirection == "right")
                    {
                        snakingOffsetDirection = "down";
                    }
                    else if (snakingOffsetDirection == "down")
                    {
                        snakingOffsetDirection = "left";
                    }
                    else if (snakingOffsetDirection == "left")
                    {
                        snakingOffsetDirection = "up";
                    }
                    else
                    {
                        Debug.Log("Invalid snaking offset direction given, defaulting to 'right'");
                        snakingOffsetDirection = "right";
                    }
                }
                //----------------------------------
                postOffsetSnakeDirection = new Vector2(-currentDirectionVec.x, -currentDirectionVec.y);
                //----------------------------------
                if (snakingOffsetDirection == "up")
                {
                    targetPositionA = new Vector3(transform.position.x, transform.position.y + 12, transform.position.z);
                    currentDirectionVec = new Vector3(0, 1);
                }
                else if (snakingOffsetDirection == "right")
                {
                    targetPositionA = new Vector3(transform.position.x + 12, transform.position.y, transform.position.z);
                    currentDirectionVec = new Vector3(1, 0);
                }
                else if (snakingOffsetDirection == "down")
                {
                    targetPositionA = new Vector3(transform.position.x, transform.position.y - 12, transform.position.z);
                    currentDirectionVec = new Vector3(0, -1);
                }
                else if (snakingOffsetDirection == "left")
                {
                    targetPositionA = new Vector3(transform.position.x - 12, transform.position.y, transform.position.z);
                    currentDirectionVec = new Vector3(-1, 0);
                }
                else
                {
                    Debug.Log("Invalid snaking offset direction given.... WHERE DO I GO, GEORGE?!");
                }
                //transform.position = currentDirectionVec;
                //----------------------------------
                //Debug.Log("Snaking");
            }

            // Set timer until we can collide again to prevent vacuum passing through
            // other objects at high-speeds
            canCollide = false;
            StartCoroutine(DelayCollisionEnable(0.0005f));
        }
    }

    private Algorithm getNextAlg()
    {
        Debug.Log(pathingDict);
        Algorithm nextAlgorithm;
        foreach (var alg in pathingDict)
        {
            if (alg.Value == true)
            {
                pathingDict[alg.Key] = false;

                switch (alg.Key)
                {
                    case "random":
                        nextAlgorithm = Algorithm.Random;
                        InterSceneManager.setAlgorithm("random", false);
                        break;
                    case "spiral":
                        nextAlgorithm = Algorithm.Spiral;
                        InterSceneManager.setAlgorithm("spiral", false);
                        break;
                    case "snaking":
                        nextAlgorithm = Algorithm.Snaking;
                        InterSceneManager.setAlgorithm("snaking", false);
                        break;
                    case "wallfollow":
                        nextAlgorithm = Algorithm.WallFollow;
                        InterSceneManager.setAlgorithm("wallFollow", false);
                        break;
                    default:
                        nextAlgorithm = Algorithm.Random;
                        InterSceneManager.setAlgorithm("random", false);
                        break;
                }
                return nextAlgorithm;

            }
        }
        return Algorithm.BadAlg;
    }

    private Dictionary<string, bool> initialAlgCheck(Dictionary<string, bool> pathDict)
    {

        // Check if all pathing algorithms are turned off
        bool allValuesAreFalse = true;
        foreach (var pair in pathDict)
        {
            if (pair.Value != false)
            {
                allValuesAreFalse = false;
                break;
            }
        }

        // IF all are false, we create a default dictionary
        if (allValuesAreFalse)
        {
            // Create a default dictionary with only random enabled
            Dictionary<string, bool> defaultPathDict = new Dictionary<string, bool>
            {
                { "random", true },
                { "spiral", false },
                { "snaking", false },
                { "wallfollow", false }
            };
            return defaultPathDict;
        }
        // Otherwise, just return the dictionary that the user specified
        else
        {
            return pathDict;
        }

    }
    
    // Method to rotate a Vector3 by 45 degrees counterclockwise
    Vector3 RotateVector45Degrees(Vector3 direction)
    {
        float angleInRadians = Mathf.Deg2Rad * 45f;
        float cosTheta = Mathf.Cos(angleInRadians);
        float sinTheta = Mathf.Sin(angleInRadians);

        float x = direction.x * cosTheta - direction.y * sinTheta;
        float y = direction.x * sinTheta + direction.y * cosTheta;

        return new Vector3(x, y, direction.z).normalized;
    }
}
