using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmTester : MonoBehaviour
{

    RandomWalk randomAlg = new RandomWalk();
    WallFollow wallFollow = new WallFollow();
    Vector2 currentDirectionVec = new Vector2(-0.28f, -0.72f);

    int counter;

    Ray2D ray;

    // Start is called before the first frame update
    void Start()
    {
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        counter++;
        //ray = new Ray2D(transform.position, -transform.right);

        // NEED TO REPLACE Vector2.left WITH -transform.right
        Debug.DrawRay(transform.position, Vector3.left * 100, Color.white, 1);
        RaycastHit2D hitData = Physics2D.Raycast(transform.position, Vector2.left);
        if (hitData.collider != null )
        {
            Debug.Log(hitData.collider.gameObject.name);
        }


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

        if ((counter % 500) == 0) {
            currentDirectionVec = wallFollow.getStartingVec();
            Debug.Log("Random: "+currentDirectionVec.x + ", " + currentDirectionVec.y);
            currentDirectionVec = wallFollow.getFirstCollisionVec(currentDirectionVec);
            Debug.Log("Cardinal: "+currentDirectionVec.x + ", " + currentDirectionVec.y);
            //currentDirectionVec = wallFollow.getNewDirectionVec(currentDirectionVec, false, false, true); 
            //currentDirectionVec = randomAlg.getNewDirectionVec(true, true);
            counter = 0;
        }

    }
}
