using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is designed to control the rotation of an object (e.g., whiskers) in a Unity game.
public class WhiskersRotation : MonoBehaviour
{
    private bool isRotating; // Flag to control whether the whiskers are rotating.
    private float rotationSpeed = 50f; // The speed at which the whiskers rotate.
    private float rotationMultiplier = 1f; // Multiplier to vary the rotation speed.

    // Start is called before the first frame update
    void Start()
    {
        // Initialize rotation settings when the game starts.
        isRotating = true; // Initially, the whiskers are set to rotate.
        rotationSpeed = 30f; // Set initial rotation speed.
    }

    // Update is called once per frame
    void Update()
    {
        // This method updates the rotation of the whiskers every frame.
        if (isRotating)
        {
            // If rotation is enabled, rotate the whiskers around their centerpoint.
            // The rotation is around the z-axis (Vector3.forward), and it accounts for frame time variability using Time.deltaTime.
            transform.Rotate(Vector3.forward * rotationSpeed * rotationMultiplier * Time.deltaTime);
        }
    }

    // Call this method to stop the rotation of the whiskers.
    public void StopRotation()
    {
        isRotating = false;
    }

    // Call this method with a float parameter to set a new rotation speed multiplier.
    public void SetRotationMultiplier(float mult)
    {
        rotationMultiplier = mult; // Update the rotation speed multiplier.
    }
}