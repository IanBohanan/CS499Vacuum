// This script, WhiskersRotation, controls the rotation behavior of whisker components in the Unity application.
// It enables the rotation of whiskers, allowing control over rotation speed and multiplier.
// It also provides a method to stop the rotation when needed.
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiskersRotation : MonoBehaviour
{
    private bool isRotating; // Whether or not the whiskers are rotating
    private float rotationSpeed = 50f; // How fast the whiskers rotate
    private float rotationMultiplier = 1f; // How much the whiskers rotate

    // Start is called before the first frame update
    void Start()
    {
        isRotating = true;
        rotationSpeed = 30f; // Set the rotation speed to 30 degrees per second
    }

    // Update is called once per frame
    void Update()
    {
        // Update rotation direction every once in a while
        if (isRotating)
        {
            // Rotate whiskers around their centerpoint
            transform.Rotate(Vector3.forward * rotationSpeed * rotationMultiplier * Time.deltaTime);
        }
    }
    // Stop the rotation of whiskers
    public void StopRotation() 
    {
        isRotating = false;
    }

    public void SetRotationMultiplier(float mult) 
    {
        // Set the rotation multiplier to adjust rotation speed
        rotationMultiplier = mult;
    }
}
