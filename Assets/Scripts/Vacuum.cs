using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class represents the behavior of a robotic vacuum cleaner in a simulation.
public class Vacuum : MonoBehaviour
{
    // Transform components for the robot vacuum and its parts
    private Transform robotTransform;      // Transform of the robot itself
    private Transform vacuumTransform;     // Transform of the vacuum component
    private Transform whiskersTransform;   // Transform of the whiskers component

    // Vacuum Attributes
    private int efficiency;                // Efficiency of the vacuum (not yet implemented)
    private float speed;                   // Movement speed of the vacuum (inches per second)

    public float batteryLifeMinutes;       // Total battery life in minutes
    private float currBatteryLife;         // Current battery life in seconds
    private bool whiskersEnabled;          // Flag to check if whiskers are enabled
    private string floorCovering;          // Type of floor covering
    private bool isBatteryDead;            // Flag to check if the battery is dead
    private bool canCollide = true;        // Flag to allow collision
    private Dictionary<string, bool> pathingDict = new Dictionary<string, bool>(); // Pathing options dictionary

    // Coroutine to prevent the vacuum from passing through walls during collision
    private IEnumerator DelayCollisionEnable(float delay)
    {
        yield return new WaitForSeconds(delay);
        canCollide = true;
    }

    // Start method called before the first frame update
    void Start()
    {
        speed = 0.005f; // Set initial speed

        // Get simulation settings from InterSceneManager
        (whiskersEnabled, floorCovering, batteryLifeMinutes) = InterSceneManager.getSimulationSettings();

        // Find child GameObjects
        robotTransform = transform.Find("Robot");
        vacuumTransform = transform.Find("Vacuum");
        whiskersTransform = transform.Find("Whiskers");

        // Activate or deactivate whiskers based on settings
        whiskersTransform.gameObject.SetActive(whiskersEnabled);
        
        // Initialize battery life
        currBatteryLife = batteryLifeMinutes * 60;
        isBatteryDead = currBatteryLife <= 0;

        //TODO: Determine efficiency based on floor covering
    }

    // Update method called once per frame
    void Update()
    {
        if (!isBatteryDead)
        {
            // Decrement battery life based on time and speed multiplier
            currBatteryLife -= Time.deltaTime * (float)InterSceneManager.speedMultiplier;

            // Get whiskers component to control its rotation
            WhiskersRotation whiskersRotation = whiskersTransform.GetComponent<WhiskersRotation>();

            // Check if battery is dead to stop the vacuum
            if (currBatteryLife <= 0)
            {
                currBatteryLife = 0;
                whiskersRotation?.StopRotation();
                isBatteryDead = true;
            }

            // Basic movement logic (to be replaced with pathing algorithms)
            Vector3 movement = new Vector3(speed * (float)InterSceneManager.speedMultiplier, 0, 0);
            transform.position += movement;
            robotTransform.position = transform.position;
            vacuumTransform.position = transform.position;
            whiskersTransform.position = transform.position;
        }
    }

    // Handle collision with other objects
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canCollide)
        {
            Debug.Log("Vacuum Collided...");
            //TODO: Implement collision handling logic for pathing algorithm
            speed *= -1;

            // Delay next collision to prevent passing through objects at high speed
            canCollide = false;
            StartCoroutine(DelayCollisionEnable(0.0005f));
        }
    }
}
