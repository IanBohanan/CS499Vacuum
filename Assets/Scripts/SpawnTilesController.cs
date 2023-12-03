// This script, SpawnTilesController, manages the spawning of tiles in a Unity application, particularly for a simulation setup.
// It subscribes to events triggered by the "RoomManager" to coordinate tile spawning and handles the transition to a new scene
// once the tiles have finished spawning.
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.SceneManagement;
using UnityEditor.EditorTools;

public class SpawnTilesController : MonoBehaviour
{
    #region OnEnable Class Setup
    private void OnEnable()
    {
        RoomManager.spawnedTiles += doneSpawningTiles;
    }
    // Start is called before the first frame update
    private void Start()
    {
        // Find the RoomManager GameObject in the scene
        GameObject roomManager = GameObject.Find("RoomManager");
        // Begin the flood simulation and store the result
        string result = roomManager.GetComponent<RoomManager>().beginFlood();
    }

    // Function to be called when tile spawning is done
    private void doneSpawningTiles()
    {
        // Load the "SimulationSetup" scene to proceed with the simulation
        SceneManager.LoadScene("SimulationSetup");
    }
    #endregion
}
