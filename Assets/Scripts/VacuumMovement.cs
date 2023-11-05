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

    // Keep track of all objects in robot vacuum prefab (Consider Serialized Fields?)
    private Transform robotTransform;
    private Transform vacuumTransform;
    private Transform whiskersTransform;

    RandomWalk randomAlg = new RandomWalk();
    WallFollow wallFollow = new WallFollow();
    Vector2 currentDirectionVec;

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

        // Get Starting Vector for Pathing Alg, save dir globally
        // currentAlg = getNextAlg();
        currentAlg = Algorithm.Random;
        counter = 0;

        (pathingDict["random"], pathingDict["spiral"], pathingDict["snaking"], 
            pathingDict["wallfollow"]) = InterSceneManager.getPathAlgs();

        // Find the child GameObjects by their names.
        robotTransform = transform.Find("Robot");
        vacuumTransform = transform.Find("Vacuum");
        whiskersTransform = transform.Find("Whiskers");

        // Pathing Algorithm Setup
        if (currentAlg == Algorithm.Random)
        {
            currentDirectionVec = randomAlg.getNewDirectionVec(true, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        counter++;

        if (currentAlg == Algorithm.WallFollow)
        {
            if ((counter % 500) == 0)
            {
                currentDirectionVec = wallFollow.getStartingVec();
                Debug.Log("Random: " + currentDirectionVec.x + ", " + currentDirectionVec.y);
                currentDirectionVec = wallFollow.getFirstCollisionVec(currentDirectionVec);
                Debug.Log("Cardinal: " + currentDirectionVec.x + ", " + currentDirectionVec.y);
                //currentDirectionVec = wallFollow.getNewDirectionVec(currentDirectionVec, false, false, true); 
                //currentDirectionVec = randomAlg.getNewDirectionVec(true, true);
                counter = 0;
            }
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
        Debug.Log("Bleh");
        Vector3 vacuumPos = transform.position;
        Vector3 colliderPos = collision.transform.position;

        Vector2 vacuumSize = GetComponent<Collider2D>().bounds.size;
        Vector2 colliderSize = collision.bounds.size;
        if (canCollide)
        {

            if (currentAlg == Algorithm.Random)
            {

                bool rightCollision = vacuumPos.x < colliderPos.x 
                    && Mathf.Abs(vacuumPos.y - colliderPos.y) < (vacuumSize.y + colliderSize.y) / 2;

                bool topCollision = vacuumPos.y > colliderPos.y
                    && Mathf.Abs(vacuumPos.x - colliderPos.x) < (vacuumSize.x + colliderSize.x) / 2;

                bool bottomCollision = vacuumPos.y < colliderPos.y
                    && Mathf.Abs(vacuumPos.x - colliderPos.x) < (vacuumSize.x + colliderSize.x) / 2;

                bool leftCollision = vacuumPos.x > colliderPos.x
                    && Mathf.Abs(vacuumPos.y - colliderPos.y) < (vacuumSize.y + colliderSize.y) / 2;

                bool horColl = ((topCollision || bottomCollision) && !(bottomCollision || topCollision));

                Debug.Log("RIGHT: " + rightCollision);
                Debug.Log("LEFT: " + leftCollision);
                Debug.Log("TOP: " + topCollision);
                Debug.Log("BOTTOM: " + bottomCollision);
                Debug.Log(collision.gameObject);
                bool movingPositively = (currentDirectionVec.x > 0 || currentDirectionVec.y > 0);
                currentDirectionVec = randomAlg.getNewDirectionVec(movingPositively, horColl);
                // Set timer until we can collide again to prevent vacuum passing through
                // other objects at high-speeds
                canCollide = false;
                StartCoroutine(DelayCollisionEnable(0.0005f));
            }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Error Warning");
    }

    private Algorithm getNextAlg()
    {
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
