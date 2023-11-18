using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// WallPlacer class is responsible for handling wall placement and interaction in a house building environment.
public class WallPlacer : MonoBehaviour
{
    // Static event to notify when the first wall is selected in the placement process.
    public static event Action firstWallSelected;

    // References to prefab and UI elements for wall placement
    public GameObject wallPrefab;           // Prefab used for spawning new wall segments.
    public GameObject upperExtender;        // The upper extension point of the wall.
    public GameObject lowerExtender;        // The lower pivot point of the wall.
    public GameObject UI;                   // UI elements associated with the wall.
    public GameObject wallEndpoint1;        // First endpoint of the wall.
    public GameObject wallEndpoint2;        // Second endpoint of the wall.
    public Transform WallTransform;         // The transform of the wall's parent object.

    private bool isBeingPlaced = false;     // Flag to determine if the wall is currently being placed.

    // Subscribe to events when the object is enabled.
    private void OnEnable()
    {
        HouseBuilderUI.stateUpdate += stateUpdated;
        WallPlacer.firstWallSelected += disableSpawners;
    }

    // Unsubscribe from events when the object is disabled to prevent errors.
    private void OnDisable()
    {
        HouseBuilderUI.stateUpdate -= stateUpdated;
        WallPlacer.firstWallSelected -= disableSpawners;
    }

    // Method to disable wall extenders (spawn points) on other walls.
    public void disableSpawners()
    {
        if (!isBeingPlaced)
        {
            disableWallUI();
        }
    }

    // Disables the UI of the wall, leaving only the wall visible.
    public void disableWallUI()
    {
        UI.SetActive(false);
    }

    // Method to extend the wall by placing a new wall object at the spawn point.
    public void extendWall(Vector3 spawnPoint)
    {
        // Checks if the current wall is the final one to close the room.
        if (upperExtender.GetComponent<WallExtender>().connectedToWall && 
            lowerExtender.GetComponent<WallExtender>().connectedToWall && 
            isBeingPlaced)
        {
            print("WallPlacer: Room closed!");
            isBeingPlaced = false;
        }
        else
        {
            // Create the new wall object and initialize it.
            isBeingPlaced = false;
            GameObject nextWall = Instantiate(wallPrefab, spawnPoint, Quaternion.identity);
            nextWall.transform.rotation = this.transform.rotation;
            nextWall.GetComponent<WallPlacer>().isBeingPlaced = true;
            firstWallSelected?.Invoke();
        }
        disableWallUI();
        wallEndpoint1.SetActive(true);
        wallEndpoint2.SetActive(true);
    }

    // Updates the rotation of the wall based on the cursor's position relative to the pivot point.
    public void updateRotation()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 distance = mousePosition - lowerExtender.transform.position;
        Vector3 newRotation = this.transform.eulerAngles;

        // Update rotation based on mouse position relative to lower extender
        if (distance.y < -1) { newRotation = new Vector3(0, 0, 180); }
        if (distance.y > 1) { newRotation = new Vector3(0, 0, 0); }
        if (distance.x > 3) { newRotation = new Vector3(0, 0, 270); }
        if (distance.x < -3) { newRotation = new Vector3(0, 0, 90); }

        this.transform.eulerAngles = newRotation;
    }

    // Called when the UI state changes, to update wall placement UI accordingly.
    private void stateUpdated(string newState)
    {
        // Activate or deactivate UI based on the current UI state.
        if (newState == "WallPlacement") { UI.SetActive(true); }
        else
        {
            disableWallUI();
            if (isBeingPlaced) { Destroy(transform.root.gameObject); }
        }
    }

    // Initialize the wall endpoints at the start.
    private void Start()
    {
        if (isBeingPlaced)
        {
            wallEndpoint1.SetActive(false);
            wallEndpoint2.SetActive(false);
        }
    }

    // Update the wall rotation every frame if it is being placed.
    void Update()
    {
        if (isBeingPlaced) updateRotation();
    }
}