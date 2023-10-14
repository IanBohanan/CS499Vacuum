using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumMovement : MonoBehaviour
{
    public float speed = 0.03f; // Adjust the speed in the Inspector.

    private Transform robotTransform;
    private Transform vacuumTransform;
    private Transform whiskersTransform;

    [SerializeField]
    private GameObject whiskers;

    void Start()
    {
        // Find the child GameObjects by their names.
        robotTransform = transform.Find("Robot");
        vacuumTransform = transform.Find("Vacuum");
        whiskersTransform = transform.Find("Whiskers");

        // Instantiate with whiskers enabled/disabled based on button
    }

    void Update()
    {
        // Move the entire "Vacuum-Robot" prefab.
        transform.position += new Vector3(speed, 0, 0);

        // Move the child GameObjects along with the parent.
        robotTransform.position = transform.position;
        vacuumTransform.position = transform.position;
        whiskersTransform.position = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Reclaulate trjectory using the given aglorithm
        print("Vacuum collided");
        speed *= -1;
    }
}
