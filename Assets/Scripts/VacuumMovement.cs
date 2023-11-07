using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumMovement : MonoBehaviour
{
    enum Algorithm
    {
        Random = 0,
        WallFollow = 1,
        Spiral = 2,
        Snaking = 3,
        BadAlg = -1
    }
    Algorithm currentAlg;

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
    public Vector2 currentDirectionVec;

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

    // Start is called before the first frame update
    void Start()
    {
        // Set Speed
        speed = 0.005f;

        counter = 0;

        (pathingDict["random"], pathingDict["spiral"], pathingDict["snaking"], 
            pathingDict["wallfollow"]) = InterSceneManager.getPathAlgs();

        // Get Starting Vector for Pathing Alg, save dir globally
        // currentAlg = getNextAlg();
        currentAlg = getNextAlg();

        // Find the child GameObjects by their names.
        robotTransform = transform.Find("Robot");
        vacuumTransform = transform.Find("Vacuum");
        whiskersTransform = transform.Find("Whiskers");

        // Pathing Algorithm Setup
        if (currentAlg == Algorithm.Random)
        {
            currentDirectionVec = new Vector2(-1, 0);
           // currentDirectionVec = randomAlg.getStartingVec();
        }
    }

    // Update is called once per frame
    void Update()
    {
        counter++;

        if (currentAlg == Algorithm.Random)
        {
            //Debug.Log("Random");
        }
        else if (currentAlg == Algorithm.WallFollow)
        {
            Debug.Log("WallFollow");
        }
        else if (currentAlg == Algorithm.Spiral)
        {
            Debug.Log("Spiral");
        }
        else if (currentAlg == Algorithm.Snaking)
        {
            Debug.Log("Snaking");
        }

        // Move the entire "Vacuum-Robot" prefab.
        // Calculate next Vacuum move
        Vector3 movePosition = new Vector3(currentDirectionVec.x , currentDirectionVec.y, 0) * speed * InterSceneManager.speedMultiplier;
        // Move the child GameObjects along with the parent.
        transform.position += movePosition;
        robotTransform.position += movePosition;
        vacuumTransform.position += movePosition;
        whiskersTransform.position += movePosition;

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

        Debug.Log("yeah");

        if (canCollide)
        {

            if (currentAlg == Algorithm.Random)
            {
                RaycastHit2D shortestRay = hitData[0];
                int indexOfShortestRay = 0;
                for (int i = 0; i < hitData.Length; i++)
                {
                    RaycastHit2D compareTo = hitData[i];
                    if (compareTo.distance < shortestRay.distance)
                    {
                        shortestRay = compareTo;
                        indexOfShortestRay = i;
                    }

                }

                Direction closestDir = (Direction)indexOfShortestRay;

                Vector2 collisionDir = new Vector2(0, 0);
                if (closestDir == Direction.Up)
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
                }

                Debug.Log(closestDir);
                currentDirectionVec = randomAlg.getNewDirectionVec(collisionDir);
            } 
            else if (currentAlg == Algorithm.WallFollow)
            {
                Debug.Log("WallFollow");
            }
            else if (currentAlg == Algorithm.Spiral)
            {
                Debug.Log("Spiral");
            }
            else if (currentAlg == Algorithm.Snaking)
            {
                Debug.Log("Snaking");
            }

            // Set timer until we can collide again to prevent vacuum passing through
            // other objects at high-speeds
            canCollide = false;
            StartCoroutine(DelayCollisionEnable(0.0005f));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("No collision logic");
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
                        break;
                    case "spiral":
                        nextAlgorithm = Algorithm.Spiral;
                        break;
                    case "snaking":
                        nextAlgorithm = Algorithm.Snaking;
                        break;
                    case "wallfollow":
                        nextAlgorithm = Algorithm.WallFollow;
                        break;
                    default:
                        nextAlgorithm = Algorithm.Random;
                        break;
                }
                return nextAlgorithm;

            }
        }
        return Algorithm.BadAlg;
    }
}
