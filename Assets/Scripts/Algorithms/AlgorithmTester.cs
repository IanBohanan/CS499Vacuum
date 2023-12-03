// The "AlgorithmTester" script is used to test and visualize pathfinding algorithms within a Unity scene.
// It simulates the movement and collision detection of an object (likely representing a robotic vacuum
// or similar entity) within the scene using various pathfinding algorithms. The script contains logic
// to display debug information, such as rays and collision detection results, to help visualize the algorithms' behavior.
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AlgorithmTester : MonoBehaviour
{

    RandomWalk randomAlg = new RandomWalk();
    WallFollow wallFollow = new WallFollow();
    Vector2 currentDirectionVec = new Vector2(1f, 0f);

    int counter;

    // Start is called before the first frame update
    void Start()
    {
        counter = 0;
        currentDirectionVec = wallFollow.getStartingVec();
        Debug.Log(currentDirectionVec);
        currentDirectionVec = wallFollow.getFirstCollisionVec(currentDirectionVec, true);
        Debug.Log(currentDirectionVec);
    }

    // Update is called once per frame
    void Update()
    {
        counter++;
        //ray = new Ray2D(transform.position, -transform.right);

        // NEED TO REPLACE Vector2.left WITH -transform.right
        Debug.DrawRay(transform.position, Vector3.left * 100, Color.white, 1);
        RaycastHit2D hitData;
        /* = Physics2D.Raycast(transform.position, Vector2.right);
        if (hitData.collider != null )
        {
            Debug.Log(hitData.collider.gameObject.name);
        }*/

        hitData = Physics2D.Raycast(transform.position, -transform.right);

        //Debug.Log(hitData.collider);
        //Debug.Log(ray.origin + " " + ray.direction + " Distance: " + hitData.distance);

        //hitData = Physics.RaycastAll(transform.position, -transform.right, 100);
        //foreach (var hit in hitData)
        //{
        //    if (!hit.transform.tag.Equals(transform.tag))
        //    {
        //        Debug.Log(hit.distance);
        //    }
        //    else
        //    {
        //        Debug.Log(hit.distance + " Name: " + hit.transform.tag);
        //    }
        //}

        if ((counter % 500) == 0)
        {
            //Debug.Log("Random: " + currentDirectionVec.x + ", " + currentDirectionVec.y);
            //currentDirectionVec = wallFollow.getFirstCollisionVec(currentDirectionVec);
            //Debug.Log("Cardinal: " + currentDirectionVec.x + ", " + currentDirectionVec.y);
            currentDirectionVec = wallFollow.getNewDirectionVec(currentDirectionVec, false, (hitData.distance < 3));
            //currentDirectionVec = randomAlg.getNewDirectionVec(true, true);
            Debug.Log(currentDirectionVec);
            counter = 0;
        }

    }
}
