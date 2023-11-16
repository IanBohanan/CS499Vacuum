using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumMovement : MonoBehaviour
{
    // Enumeration for different vacuum cleaning algorithms
    enum Algorithm
    {
        Random = 0,
        WallFollow = 1,
        Spiral = 2,
        Snaking = 3,
        BadAlg = -1
    }
    Algorithm currentAlg;

    // References to different parts of the vacuum robot
    private Transform robotTransform;
    private Transform vacuumTransform;
    private Transform whiskersTransform;

    // Instances of algorithm classes
    RandomWalk randomAlg = new RandomWalk();
    WallFollow wallFollow = new WallFollow();
    public Vector2 currentDirectionVec;

    private float speed; // Movement speed of the vacuum
    int counter;        
    bool canCollide = true; // Flag to control collision behavior

    // Dictionary to manage pathing algorithms
    private Dictionary<string, bool> pathingDict = new Dictionary<string, bool>();

    // Coroutine to handle collision delay
    private IEnumerator DelayCollisionEnable(float delay)
    {
        yield return new WaitForSeconds(delay);
        canCollide = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = 0.005f; // Initialize speed
        counter = 0;

        // Retrieve pathing algorithms from scene manager
        (pathingDict["random"], pathingDict["spiral"], pathingDict["snaking"], 
            pathingDict["wallfollow"]) = InterSceneManager.getPathAlgs();

        // Determine initial algorithm
        currentAlg = getNextAlg();

        // Find and assign the child GameObjects
        robotTransform = transform.Find("Robot");
        vacuumTransform = transform.Find("Vacuum");
        whiskersTransform = transform.Find("Whiskers");

        // Initialize the pathing algorithm
        if (currentAlg == Algorithm.Random)
        {
            currentDirectionVec = randomAlg.getNewDirectionVec(true, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        counter++;

        // Log current algorithm for debugging
        Debug.Log(currentAlg.ToString());

        // Calculate and apply the movement of the vacuum
        Vector3 movePosition = new Vector3(currentDirectionVec.x, currentDirectionVec.y, 0) * speed * InterSceneManager.speedMultiplier;
        transform.position += movePosition;
        robotTransform.position += movePosition;
        vacuumTransform.position += movePosition;
        whiskersTransform.position += movePosition;
    }

    // Handle trigger events for collisions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canCollide)
        {
            // Collision logic for different algorithms
            HandleCollision(collision);

            // Start collision delay coroutine
            canCollide = false;
            StartCoroutine(DelayCollisionEnable(0.0005f));
        }
    }

    // Empty method for collision events - To be implemented if needed
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("No collision logic");
    }

    // Method to determine the next cleaning algorithm
    private Algorithm getNextAlg()
    {
        foreach (var alg in pathingDict)
        {
            if (alg.Value)
            {
                pathingDict[alg.Key] = false;

                return alg.Key switch
                {
                    "random" => Algorithm.Random,
                    "spiral" => Algorithm.Spiral,
                    "snaking" => Algorithm.Snaking,
                    "wallfollow" => Algorithm.WallFollow,
                    _ => Algorithm.BadAlg,
                };
            }
        }
        return Algorithm.BadAlg;
    }

    // Method to handle collision based on the current algorithm
    private void HandleCollision(Collider2D collision)
    {
        // Add specific collision handling logic for each algorithm here
        switch (currentAlg)
        {
            case Algorithm.Random:
                // Add Random algorithm collision logic
                break;
            case Algorithm.WallFollow:
                // Add WallFollow algorithm collision logic
                break;
            case Algorithm.Spiral:
                // Add Spiral algorithm collision logic
                break;
            case Algorithm.Snaking:
                // Add Snaking algorithm collision logic
                break;
        }
    }
}