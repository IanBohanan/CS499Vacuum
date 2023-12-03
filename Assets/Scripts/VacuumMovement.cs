// This script, VacuumMovement, is responsible for controlling the movement algorithms and behavior of a robotic vacuum cleaner in a simulation. It manages various movement algorithms such as Random, WallFollow, Spiral, and Snaking, and keeps track of the vacuum's position and state.
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class VacuumMovement : MonoBehaviour
{
    // The vacuum prefab
    Tilemap tilemap; // The tilemap of the vacuum prefab

    [SerializeField] GameObject hardwoodTile; // The hardwood tile prefab
    public enum Algorithm
    {
        // Algorithms for the vacuum to follow
        Random = 0, // Randomly move around the room
        WallFollow = 1, // Follow the walls
        Spiral = 2, // Spiral around the room
        Snaking = 3, // Snake around the room
        BadAlg = -1 // Invalid algorithm
    }
    public Algorithm currentAlg; // The current algorithm the vacuum is following

    enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    // Keep track of all objects in robot vacuum prefab (Consider Serialized Fields?)
    private Transform robotTransform; //    The robot transform of the vacuum prefab
    private Transform vacuumTransform;  // The vacuum transform of the vacuum prefab
    private Transform whiskersTransform; // The whiskers transform of the vacuum prefab

    RandomWalk randomAlg = new RandomWalk(); // The random algorithm for the vacuum
    WallFollow wallFollow = new WallFollow();  // The wall follow algorithm for the vacuum
    bool wallFollowing = false; // Whether the vacuum is currently following walls
    bool justTurned = false; // Whether the vacuum just turned
    bool currentlyTurningLeft = false; // Whether the vacuum is currently turning left
    Vector3 targetPositionA; // The target position for the vacuum to move to
    bool passedA = false; // Whether the vacuum has passed the target position
    Vector3 targetPositionB; // The target position for the vacuum to move to
    bool passedB = false; // Whether the vacuum has passed the target position

    int consecutiveLeftTurns = 0; // The number of consecutive left turns the vacuum has made

    Spiral spiral = new Spiral(); // The spiral algorithm for the vacuum
    Vector3 spiralOrigin = Vector3.zero; // The origin of the spiral algorithm
    bool isSpiraling = false; // Whether the vacuum is currently spiraling
    float spiralStartTime = 0f; // The time the spiral algorithm started

    bool collidedBefore = false; // Whether the vacuum has collided with an object before
    bool snakingHorizontalWalls = false;  // Whether the vacuum is currently snaking horizontally
    string snakingOffsetDirection = "up"; // The direction the vacuum is currently snaking horizontally
    bool currentlyOffsettingSnake = false; // Whether the vacuum is currently offsetting the snake
    Vector3 postOffsetSnakeDirection = Vector3.zero; // The direction the vacuum is currently offsetting the snake

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
            if ((!justTurned) && hitDataLeft.distance > ((InterSceneManager.speedMultiplier == 50)? 15:10))
            {
                if (wallFollowing && (consecutiveLeftTurns < 4))
                {
                    //speed = 0;
                    //Debug.Log(hitDataLeft.distance);
                    float x = (currentDirectionVec.x * 8);
                    float y = (currentDirectionVec.y * 8);
                    targetPositionA = (transform.position + (new Vector3(x, y, 0)));
                    x = -(currentDirectionVec.y); 
                    y = (currentDirectionVec.x);
                    targetPositionB = (targetPositionA + (new Vector3(x, y, 0)));

                    if (InterSceneManager.speedMultiplier == 50)
                    {
                        Debug.Log("here");
                        transform.position = targetPositionB;
                        currentDirectionVec = wallFollow.turnLeft(currentDirectionVec);
                    }
                    else
                    {
                        justTurned = true;
                    }
                    consecutiveLeftTurns++;
                }
            }
            else if (justTurned)
            {
                if ((!passedA) && (Vector3.Distance(targetPositionA, transform.position) < ((3)))){
                    transform.position = targetPositionA;
                    passedA = true;
                    currentDirectionVec = wallFollow.turnLeft(currentDirectionVec);
                    if (currentDirectionVec == new Vector2(1, 0))
                    {
                        // Rotate to face right (positive x)
                        transform.rotation = Quaternion.Euler(0, 0, -90);
                    }
                    else if (currentDirectionVec == new Vector2(0, 1))
                    {
                        // Rotate to face upwards (positive y)
                        transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                    else if (currentDirectionVec == new Vector2(-1, 0))
                    {
                        // Rotate to face left (negative y)
                        transform.rotation = Quaternion.Euler(0, 0, 90);
                    }
                    else if (currentDirectionVec == new Vector2(0, -1))
                    {
                        // Rotate to face downwards (negative y)
                        transform.rotation = Quaternion.Euler(0, 0, 180);
                    }
                }
                else if ((!passedB) && (Vector3.Distance(targetPositionB, transform.position) < ((3))))
                {
                    transform.position = targetPositionB;
                    passedB = true;
                }
                if (passedA && passedB)
                {
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
                if ((hitDataLeft.distance < 50) && (hitDataLeft.collider)) { enoughSpace = false; }
                if ((hitDataRight.distance < 50) && (hitDataRight.collider)) { enoughSpace = false; }
                if ((hitDataUp.distance < 50) && (hitDataUp.collider)) { enoughSpace = false; }
                if ((hitDataDown.distance < 50) && (hitDataDown.collider)) { enoughSpace = false; }

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
            if (currentlyOffsettingSnake && (Vector3.Distance(targetPositionA, transform.position) < (InterSceneManager.speedMultiplier/2)))
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
        Vector3 movePosition = new Vector3(currentDirectionVec.x , currentDirectionVec.y, 0) * speed * (currentlyOffsettingSnake ? InterSceneManager.speedMultiplier/2:InterSceneManager.speedMultiplier) * (InterSceneManager.vacuumSpeed);
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
                InterSceneManager.cleanedTiles[tileIndex].hits += Mathf.RoundToInt(InterSceneManager.speedMultiplier/2);
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
        //Debug.Log(cellPosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cast rays in cardinal directions to determine more collision info
        RaycastHit2D hitDataLeft = Physics2D.Raycast(transform.position, -transform.right);
        RaycastHit2D hitDataRight = Physics2D.Raycast(transform.position, transform.right);
        RaycastHit2D hitDataUp = Physics2D.Raycast(transform.position, transform.up);
        RaycastHit2D hitDataDown = Physics2D.Raycast(transform.position, -transform.up);

        if (canCollide)
        {
            if (currentAlg == Algorithm.Random)
            {
                currentDirectionVec = -currentDirectionVec;
                float x = currentDirectionVec.x * (0.25f * InterSceneManager.speedMultiplier);
                float y = currentDirectionVec.y * (0.25f * InterSceneManager.speedMultiplier);
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
                        if (hitDataUp.distance < 5){
                            newDir = true;
                        }
                    }
                    if (currentDirectionVec.x > 0)
                    {
                        if (hitDataRight.distance < 5)
                        {
                            newDir = true;
                        }
                    }
                    if (currentDirectionVec.y < 5)
                    {
                        if (hitDataDown.distance < 1)
                        {
                            newDir = true;
                        }
                    }
                    if (currentDirectionVec.x < 5)
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
                consecutiveLeftTurns = 0;
                // Back up a bit:
                currentDirectionVec = -currentDirectionVec;
                float x = currentDirectionVec.x * (0.1f * InterSceneManager.speedMultiplier);
                float y = currentDirectionVec.y * (0.1f * InterSceneManager.speedMultiplier);
                currentDirectionVec = -currentDirectionVec;
                transform.position += new Vector3(x, y, 0);

                if (wallFollowing) {
                    currentDirectionVec = wallFollow.turnRight(currentDirectionVec); 
                }
                else { currentDirectionVec = wallFollow.getFirstCollisionVec(currentDirectionVec, true); wallFollowing = true; }

                if (currentDirectionVec == new Vector2(1, 0))
                {
                    // Rotate to face right (positive x)
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                }
                else if (currentDirectionVec == new Vector2(0, 1))
                {
                    // Rotate to face upwards (positive y)
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (currentDirectionVec == new Vector2(-1, 0))
                {
                    // Rotate to face left (negative y)
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                else if (currentDirectionVec == new Vector2(0, -1))
                {
                    // Rotate to face downwards (negative y)
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                }
            }
            else if (currentAlg == Algorithm.Spiral)
            {
                if (isSpiraling)
                {
                    currentDirectionVec = -CalculateNormalizedDirection(transform.position, spiralOrigin);
                }
                isSpiraling = false; // Stop the spiral updates
                currentDirectionVec = -currentDirectionVec;
                float x = currentDirectionVec.x * (0.25f * InterSceneManager.speedMultiplier);
                float y = currentDirectionVec.y * (0.25f * InterSceneManager.speedMultiplier);
                currentDirectionVec = -currentDirectionVec;
                transform.position += new Vector3(x, y, 0);
                bool newDir = true;
                while (newDir)
                {
                    newDir = false;
                    currentDirectionVec = randomAlg.getStartingVec();
                    if (currentDirectionVec.y > 0)
                    {
                        if (hitDataUp.distance < 5)
                        {
                            newDir = true;
                        }
                    }
                    if (currentDirectionVec.x > 0)
                    {
                        if (hitDataRight.distance < 5)
                        {
                            newDir = true;
                        }
                    }
                    if (currentDirectionVec.y < 0)
                    {
                        if (hitDataDown.distance < 5)
                        {
                            newDir = true;
                        }
                    }
                    if (currentDirectionVec.x < 0)
                    {
                        if (hitDataLeft.distance < 5)
                        {
                            newDir = true;
                        }
                    }
                }
            }
            else if (currentAlg == Algorithm.Snaking)
            {
                currentlyOffsettingSnake = true;

                // Back up a bit:
                currentDirectionVec = -currentDirectionVec;
                float x = currentDirectionVec.x * (0.25f * InterSceneManager.speedMultiplier);
                float y = currentDirectionVec.y * (0.25f * InterSceneManager.speedMultiplier);
                currentDirectionVec = -currentDirectionVec;
                transform.position += new Vector3(x, y, 0);

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
            StartCoroutine(DelayCollisionEnable(0.0001f));
        }
    }

    // Example method to find normalized direction vector from origin to target
    public static Vector3 CalculateNormalizedDirection(Vector3 origin, Vector3 target)
    {

        // Calculate direction vector
        Vector3 direction = target - origin;

        // Ensure the direction vector is non-zero before normalizing
        if (direction != Vector3.zero)
        {
            // Normalize the direction vector
            direction.Normalize();
        }

        return direction;
    }

    private Algorithm getNextAlg()
    {
        // Get the next algorithm from the dictionary
        Debug.Log(pathingDict);
        Algorithm nextAlgorithm; // The algorithm we will return
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
        // Convert degrees to radians
        float angleInRadians = Mathf.Deg2Rad * 45f;
        float cosTheta = Mathf.Cos(angleInRadians);
        float sinTheta = Mathf.Sin(angleInRadians);

        float x = direction.x * cosTheta - direction.y * sinTheta;
        float y = direction.x * sinTheta + direction.y * cosTheta;

        return new Vector3(x, y, direction.z).normalized;
    }
}
