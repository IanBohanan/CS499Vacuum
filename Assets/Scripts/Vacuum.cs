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
    private float speedMultiplier;    // Based on simulation speed

    public float batteryLifeMinutes;  // In Minutes
    private float currBatteryLife;    // In Seconds (for decrementing with timeDelta)
    private bool isBatteryDead;

    public BasePathingAlg pathingAlg;

    // Start is called before the first frame update
    void Start()
    {
        // Set Speed
        speed = 0.005f;
        speedMultiplier = 1f;

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
        // Decrement the batteryLife of the vacuum
        currBatteryLife -= Time.deltaTime * speedMultiplier;

        // Check that the vacuum is not dead
        if (currBatteryLife <= 0)
        {
            currBatteryLife = 0;
            isBatteryDead = true;
        }

        if (!isBatteryDead)
        {
            // Check for 'F' keypress to change speed
            if (Input.GetKeyDown("f"))
            {
                // Increase the speed multiplier
                if (speedMultiplier == 1)
                {
                    speedMultiplier = 5f;
                }
                else if (speedMultiplier == 5)
                {
                    speedMultiplier = 50f;
                }
                else if (speedMultiplier == 50)
                {
                    speedMultiplier = 1f;
                }
            }

            // #################################################
            // # BASIC MOVEMENT TO BE REMOVED FOR PATHING ALGS #
            // #################################################

            // Move the entire "Vacuum-Robot" prefab.
            transform.position += new Vector3(speed * speedMultiplier, 0, 0);

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
                pathingAlg.nextMove(speed * speedMultiplier, robotTransform, vacuumTransform, whiskersTransform);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //TODO: Send collision signal to pathing alg (??)
        print("Vacuum collided");
        speed *= -1;
    }
}
