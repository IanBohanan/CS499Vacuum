using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmTester : MonoBehaviour
{

    RandomWalk randomAlg = new RandomWalk();
    WallFollow wallFollow = new WallFollow();

    int counter;

    // Start is called before the first frame update
    void Start()
    {
        counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        counter++;
        if ((counter % 1000) == 0) {
            Vector2 newDirectionVec = wallFollow.getStartingVec(); //randomAlg.getNewDirectionVec(true, true);
            Debug.Log(newDirectionVec.x + ", " + newDirectionVec.y);
            counter = 0;
        }

    }
}
