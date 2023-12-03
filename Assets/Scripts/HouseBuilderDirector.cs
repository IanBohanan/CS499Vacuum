// This script, HouseBuilderDirector, is responsible for initializing and connecting components related to a house-building user interface.
// It retrieves and stores references to the "HouseBuilderUI" and "LayoutManager" components in the scene.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBuilderDirector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Get the HouseBuilderUI component attached to this GameObject:
        HouseBuilderUI userInterface = GetComponent<HouseBuilderUI>();
        //// Get the LayoutManager component attached to this GameObject:
        LayoutManager layoutManager = GetComponent<LayoutManager>();
    }
}
