// This script checks for the presence of certain algorithms to run and loads the appropriate scene accordingly.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckForAlgsController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Check if there are any more algorithms to run.
        // If there are, go back to the simulation scene.
        // Otherwise, go to the data review scene.

        // Retrieve information about the available algorithms from the InterSceneManager.
        (bool rand, bool spiral, bool snaking, bool wall) = InterSceneManager.getPathAlgs();

        if (rand || spiral || snaking || wall)
        {
            // Load the "Simulation" scene if any of the algorithms are available.
            SceneManager.LoadScene(sceneName: "Simulation");
        }
        else
        {
            // Load the "DataReview" scene if none of the algorithms are available.
            SceneManager.LoadScene(sceneName: "DataReview");
        }
    }
}