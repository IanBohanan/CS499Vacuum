using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vacuum : MonoBehaviour
{
    // Keep track of all objects in robot vacuum prefab (Consider Serialized Fields?)
    private Transform robotTransform;
    private Transform vacuumTransform;
    private Transform whiskersTransform;

    // Vacuum Attributes
    private int efficiency
    {
        get { return efficiency; }
        set { efficiency = value; }
    }
    private float speed;              // Inches per Second

    public float batteryLifeMinutes;  // In Minutes
    private float currBatteryLife;    // In Seconds (for decrementing with timeDelta)
    private bool isBatteryDead;
    private bool canCollide = true;

    public BasePathingAlg pathingAlg;

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

        // Find the child GameObjects by their names.
        robotTransform = transform.Find("Robot");
        vacuumTransform = transform.Find("Vacuum");

        //TODO: Check if whiskers are needed for this run
        whiskersTransform = transform.Find("Whiskers");

        //TODO: Get batteryLife value from UI
        currBatteryLife = 150 * 60;
        if (currBatteryLife > 0) { isBatteryDead = false; }
    }

    // Update is called once per frame
    void Update()
    {

        if (!isBatteryDead)
        {
            // Decrement the batteryLife of the vacuum
            currBatteryLife -= Time.deltaTime * (float)InterSceneManager.speedMultiplier;

            // Get whiskers rotation script to call specific whiskers methods
            WhiskersRotation whiskersRotation = whiskersTransform.GetComponent<WhiskersRotation>();

            // Check that the vacuum is not dead
            if (currBatteryLife <= 0)
            {
                // If vacuum dead, stop whisker rotation and stop vacuum movement
                currBatteryLife = 0;
                if (whiskersRotation != null)
                {
                    whiskersRotation.StopRotation();
                }
                isBatteryDead = true;
            }

            // #################################################
            // # BASIC MOVEMENT TO BE REMOVED FOR PATHING ALGS #
            // #################################################

            // Move the entire "Vacuum-Robot" prefab.
            transform.position += new Vector3(speed * (float)InterSceneManager.speedMultiplier, 0, 0);

            // Move the child GameObjects along with the parent.
            robotTransform.position = transform.position;
            vacuumTransform.position = transform.position;
            whiskersTransform.position = transform.position;


            // ##########################################
            // # REPLACE BASIC MOVEMENT WITH CODE BELOW #
            // ##########################################

            // Move the Vacuum based on the selected pathing algorithm
            if (pathingAlg != null)
            {
                pathingAlg.nextMove(speed * (float)InterSceneManager.speedMultiplier, robotTransform, vacuumTransform, whiskersTransform);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canCollide)
        {
            Debug.Log("Vacuum Collided...");
            //TODO: Send collision signal to pathing alg (??)
            speed *= -1;

            // Set timer until we can collide again to prevent vacuum passing through
            // other objects at high-speeds
            canCollide = false;
            StartCoroutine(DelayCollisionEnable(0.0005f));
        }
    }
}
