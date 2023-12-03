// This class, Vacuum, represents the behavior of a robotic vacuum cleaner in a simulation.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Vacuum : MonoBehaviour
{
    // Keep track of all objects in robot vacuum prefab (Consider Serialized Fields?)
    private Transform robotTransform; // The robot's transform
    private Transform vacuumTransform; // The vacuum's transform
    private Transform whiskersTransform; // The whiskers' transform

    // Vacuum Attributes
    private int efficiency;             // Percentage of time the vacuum spends cleaning
    private float speed;              // Inches per Second

    public float batteryLifeMinutes;  // In Minutes
    public float currBatteryLife;    // In Seconds (for decrementing with timeDelta)
    private bool whiskersEnabled;   // Whether or not the whiskers are enabled
    private string floorCovering;   // Type of floor covering
    private bool isBatteryDead;     // Whether or not the battery is dead

    public static event Action batteryDead; //Sent out if battery dies

    // Start is called before the first frame update
    void Start()
    {
        // Set Speed
        speed = 0.005f;

        // Assign variables from simulation setup
        (whiskersEnabled, floorCovering, batteryLifeMinutes) = InterSceneManager.getSimulationSettings();

        // Find the child GameObjects by their names.
        robotTransform = transform.Find("Robot");
        vacuumTransform = transform.Find("Vacuum");
        whiskersTransform = transform.Find("Whiskers");

        // Use Simulation setup variables to determine if whiskers are needed
        if (whiskersEnabled) 
        {
            whiskersTransform.gameObject.SetActive(true);
        } else
        {
            whiskersTransform.gameObject.SetActive(false);
        }
        
        // Set battery life from Simulation Setup
        currBatteryLife = batteryLifeMinutes * 60;
        if (currBatteryLife > 0) { isBatteryDead = false; }

        //TODO: Determine efficiency from floorcovering
    }

    // Update is called once per frame
    void FixedUpdate()
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
                batteryDead?.Invoke();
            }
            
        }
    }

}
