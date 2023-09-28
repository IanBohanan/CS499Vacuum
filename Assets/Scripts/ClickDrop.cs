using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using JetBrains.Annotations;

public class ClickDrop : MonoBehaviour
{
    private bool deleteClicked;
    public bool isDeleteClicked
    {
        get { return deleteClicked; }
        set
        {
            if (deleteClicked == value) return;
            deleteClicked = value;
            if (onDeleteClicked != null) onDeleteClicked(gameObject);
        }
    }
    public delegate void OnVariableChangeDelegate(GameObject gameObject);
    public event OnVariableChangeDelegate onDeleteClicked;

    private bool dragging;
    public bool isDragging
    {
        get { return dragging; }
        set
        {
            if (dragging == value) return;
            dragging = value;
        }
    }

    private bool isLongObject;

    // Offset values to be set for each furniture type. Allows correct grid snapping.
    private int offsetY;
    private int offsetX;

    public Collider2D myCollider;

    // Start is called before the first frame update
    void Start()
    {
        string objectType = (gameObject.name); // Chair, Table, or Chest
        switch (objectType)
        {
            case "Chair(Clone)":
                {
                    offsetY = 3;
                    offsetX = 0;
                    break;
                }
            case "Table Variant(Clone)":
                {
                    offsetY = 3;
                    offsetX = 3;
                    break;
                }
            case "Chest Variant(Clone)":
                {
                    offsetY = 3;
                    offsetX = 3;
                    break;
                }
        }
        float width = GetComponent<SpriteRenderer>().bounds.size.x;
        float height = GetComponent<SpriteRenderer>().bounds.size.y;
        isLongObject = (width >= height);
        // On spawn, user will be dragging item
        isDragging = true;
        myCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
            //TODO: Set up a check later if the current grid is the Ui grid (where just round) or if the grid is the bigger black grid (divide by six THEN round)
            mousePosition = new Vector3(6 * Mathf.Round(mousePosition.x / 6), 6 * Mathf.Floor(mousePosition.y / 6), mousePosition.z);
            
            //If object is rotated 90 degrees, we must fix its placement by 3 grid points on both a
            if (isLongObject)
            {
                mousePosition = new Vector3(mousePosition.x + offsetX, mousePosition.y + offsetY, mousePosition.z);
            }
            transform.position = mousePosition;
        }
    }

    void OnMouseDown()
    {
        if (InterSceneManager.deleteMode)
        {
            isDeleteClicked = true;
        }
        // If not deleting:
        else if (isDragging)
        {
            if (!IsOverlapping())
            {
                isDragging = false;
            } 
        }
        else
        {
            if (!IsOverlapping())
            {
                isDragging = true;
            }
        }
    }

    bool IsOverlapping()
    {
        // Calculate the furniture's size and center based on the size of the object's Collider2D bounds
        Vector2 objectHalfSize = myCollider.bounds.size * 0.5f;
        Vector3 center = myCollider.bounds.center;

        // Check for overlapping colliders within a smaller circle
        Collider2D[] colliders = Physics2D.OverlapBoxAll(center, objectHalfSize, 0f);

        foreach (Collider2D collider in colliders)
        {
            if (collider != myCollider)
            {
                // If overlapping objects are found, return true
                return true;
            }
        }
        // If no overlapping objects found, return false
        return false;
    }
}
