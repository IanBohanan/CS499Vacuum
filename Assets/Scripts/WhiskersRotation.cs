using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiskersRotation : MonoBehaviour
{
    public bool isRotating;
    public float rotationSpeed = 30f;
    public int updateSpinCount = 0;

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
            updateSpinCount += 1;
            if (updateSpinCount == 300)
            {
                updateSpinCount = 0;
                rotationSpeed *= -1;
            }
            // Rotate whiskers around their centerpoint
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }

    public void StopRotation()
    {
        isRotating = false;
    }
}
