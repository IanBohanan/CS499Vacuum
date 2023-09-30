using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VacuumMovement : MonoBehaviour
{
    float speed = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += new Vector3(speed, 0, 0);
        if(this.transform.position.x >= 10)
        {
            //speed *= -1;
        }
        if(this.transform.position.x <= -10)
        {
            //speed *= -1;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Reclaulate trjectory using the given aglorithm
        print("Vacuum collided");
        speed *= -1;
    }
}
