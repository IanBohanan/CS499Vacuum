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
        // If so, go back to the simulation scene,
        // otherwise, go to the data review scene

        (bool rand, bool spiral, bool snaking, bool wall) = InterSceneManager.getPathAlgs();

        if (rand || spiral || snaking || wall)
        {
            SceneManager.LoadScene(sceneName: "Simulation");
        }
        else
        {
            SceneManager.LoadScene(sceneName: "DataReview");
        }
    }
}
