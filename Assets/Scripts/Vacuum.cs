using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Vacuum : MonoBehaviour
{
    // Keep track of all objects in robot vacuum prefab (Consider Serialized Fields?)
    private Transform robotTransform;
    private Transform vacuumTransform;
    private Transform whiskersTransform;

    // Vacuum Attributes
    private int efficiency;
    private float speed;              // Inches per Second

    public float batteryLifeMinutes;  // In Minutes
    public float currBatteryLife;    // In Seconds (for decrementing with timeDelta)
    private bool whiskersEnabled;
    private string floorCovering;
    private bool isBatteryDead;

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
