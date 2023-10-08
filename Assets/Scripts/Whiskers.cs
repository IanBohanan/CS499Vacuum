using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whiskers : MonoBehaviour
{
    public float rotationSpeed = 30f;
    public int updateSpinCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        rotationSpeed = 30f;
    }

    // Update is called once per frame
    void Update()
    {
        updateSpinCount += 1;

        // Update rotation direction every once in a while
        if (updateSpinCount == 300)
        {
            updateSpinCount = 0;
            rotationSpeed *= -1;
        }
        // Rotate whiskers around their centerpoint
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
