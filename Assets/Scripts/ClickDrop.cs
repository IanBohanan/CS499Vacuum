using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using JetBrains.Annotations;

// This script handles object dragging and dropping functionality as well as a delete-click event for game objects in a Unity scene.
public class ClickDrop : MonoBehaviour
{
    #region Delete Delegate and Event
    // A flag to check if the delete action was triggered
    private bool deleteClicked;
    // Public property to access and modify the deleteClicked flag safely
    public bool isDeleteClicked
    {
        get { return deleteClicked; }
        set
        {
            // If value hasn't changed, return without doing anything
            if (deleteClicked == value) return;
            // Set the deleteClicked flag
            deleteClicked = value;
            // If we have subscribers to the onDeleteClicked event, invoke the event with the current gameObject
            if (onDeleteClicked != null) onDeleteClicked(gameObject);
        }
    }
    // Declaration of a delegate type for handling delete click events
    public delegate void OnVariableChangeDelegate(GameObject gameObject);
    // Event that gets triggered when an object's delete status changes
    public event OnVariableChangeDelegate onDeleteClicked;
    #endregion

    // Flag to check if the object is being dragged
    private bool dragging = false;
    // Public property to access and modify the dragging flag safely
    public bool isDragging
    {
        get { return dragging; }
        set
        {
            if (dragging == value) return;
            dragging = value;
        }
    }

    // Flag to check if the object is longer than it is wide, which can affect how it's handled when dragging
    public bool isLongObject;

    // Offset values to adjust the object's position for snapping to a grid
    private int offsetY;
    private int offsetX;

    // The object's collider component
    public Collider2D myCollider;

    // Start is called before the first frame update
    void Start()
    {
        // Determine the object type based on its name, and set offset values accordingly
        string objectType = gameObject.name; // Name could be "Chair", "Table Variant", etc.
        switch (objectType)
        {
            case "Chair(Clone)":
                offsetY = 3;
                offsetX = 0;
                break;
            case "Table Variant(Clone)":
                offsetY = 3;
                offsetX = 3;
                break;
            case "Chest Variant(Clone)":
                offsetY = 0;
                offsetX = 3;
                break;
            case "Door(Clone)":
                offsetY = 0;
                offsetX = 3;
                break;
            default:
                offsetY = 0;
                offsetX = 0;
                break;
        }
        // Determine if the object is 'long' based on its sprite dimensions
        float width = GetComponent<SpriteRenderer>().bounds.size.x;
        float height = GetComponent<SpriteRenderer>().bounds.size.y;
        isLongObject = (width >= height);
        
        // Initialize the collider component reference
        myCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // If the object is being dragged...
        if (isDragging)
        {
            // Check for rotation input (the 'R' key)
            if (Input.GetKeyDown("r"))
            {
                // Rotate the object by 90 degrees on the Z axis
                transform.Rotate(0, 0, 90);
                // Re-evaluate whether the object is 'long' after rotation
                float width = GetComponent<SpriteRenderer>().bounds.size.x;
                float height = GetComponent<SpriteRenderer>().bounds.size.y;
                isLongObject = (width >= height);
            }
            // Get the mouse position in world coordinates
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Adjust the mouse position to be on the same plane as the camera's near clipping plane
            mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
            // Snap the object's position to a grid based on the mouse position
            mousePosition = new Vector3(6 * Mathf.Round(mousePosition.x / 6), 6 * Mathf.Floor(mousePosition.y / 6), mousePosition.z);

            // Apply the offsets if the object is considered 'long'
            if (isLongObject)
            {
                mousePosition = new Vector3(mousePosition.x + offsetX, mousePosition.y + offsetY, mousePosition.z);
            }
            // Update the object's position to the new calculated mouse position
            transform.position = mousePosition;
        }
    }

    // This method is called when the user presses the mouse button while over the collider
    void OnMouseDown()
    {
        // If we're in delete mode, set the isDeleteClicked flag to true
        if (InterSceneManager.deleteMode)
        {
            isDeleteClicked = true;
        }
        // If we're not in delete mode, handle dragging
        else
        {
            // If the object is currently being dragged...
            if (isDragging)
            {
                // If the object is not overlapping with anything, stop dragging
                if (!IsOverlapping())
                {
                    isDragging = false;
                } 
            }
            else
            {
                // If the object is not being dragged and not overlapping, start dragging
                if (!IsOverlapping())
                {
                    isDragging = true;
                }
            }
        }
    }

    // Checks if the object's collider is overlapping with any other colliders
    bool IsOverlapping()
    {
        // Calculate the size and center of the object using its collider bounds
        Vector2 objectHalfSize = myCollider.bounds.size * 0.5f;
        Vector3 center = myCollider.bounds.center;

        // Check for other colliders intersecting with this object's collider
        Collider2D[] colliders = Physics2D.OverlapBoxAll(center, objectHalfSize, 0f);

        foreach (Collider2D collider in colliders)
        {
            // If we find a collider that's not this object's collider, there's an overlap
            if (collider != myCollider)
            {
                return true; // Overlap found
            }
        }
        return false; // No overlap found
    }
}
