using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiskersRotation : MonoBehaviour
{
    private bool isRotating;
    private float rotationSpeed = 50f;
    private float rotationMultiplier = 1f;

    // Start is called before the first frame update
    void Start()
    {
        isRotating = true;
        rotationSpeed = 30f;
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

    public void StopRotation()
    {
        isRotating = false;
    }

    public void SetRotationMultiplier(float mult)
    {
        rotationMultiplier = mult;
    }
}
