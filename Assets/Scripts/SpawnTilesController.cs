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
    private void Start()
    {
        GameObject roomManager = GameObject.Find("RoomManager");
        string result = roomManager.GetComponent<RoomManager>().beginFlood();
    }

    private void doneSpawningTiles()
    {
        SceneManager.LoadScene("SimulationSetup");
    }
    #endregion
}
