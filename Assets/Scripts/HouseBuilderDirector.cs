using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBuilderDirector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get the LayoutManager component:
        HouseBuilderUI userInterface = GetComponent<HouseBuilderUI>();
        LayoutManager layoutManager = GetComponent<LayoutManager>();
    }
}
