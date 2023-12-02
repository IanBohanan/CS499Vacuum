using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using JetBrains.Annotations;

public class ClickDrop : MonoBehaviour
{
    #region Delete Delegate and Event:
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
    #endregion

    private bool dragging = false;
    public bool isDragging
    {
        get { return dragging; }
        set
        {
            if (dragging == value) return;
            dragging = value;
        }
    }

    public bool isLongObject;
    private bool isDoor = false;

    // Offset values to be set for each furniture type. Allows correct grid snapping.
    private int offsetY;
    private float offsetX;

    public Collider2D myCollider;

    // Start is called before the first frame update
    void Start()
    {
        string objectType = (gameObject.name); // Chair, Table, or Chest
        switch (objectType)
        {
            case "Flag(Clone)":
                {
                    offsetY = 3;
                    offsetX = 0.5f;
                    break;
                }
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
                    offsetY = 0;
                    offsetX = 3;
                    break;
                }
            case "Door(Clone)":
                {
                    offsetY = 3;
                    offsetX = 3;
                    isDoor = true;
                    break;
                }
            case "VacuumRobot":
                    {
                    offsetX = 0;
                    offsetY = 0;
                    break;
                }
            default:
                {
                    offsetY = 0;
                    offsetX = 0;
                    break;
                }
        }
        float width = GetComponent<SpriteRenderer>().bounds.size.x;
        float height = GetComponent<SpriteRenderer>().bounds.size.y;
        isLongObject = (width >= height);
        // On spawn, user will be dragging item
        //isDragging = false;
        myCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            if (Input.GetKeyDown("r"))
            {
                transform.Rotate(0, 0, 90);
                float width = GetComponent<SpriteRenderer>().bounds.size.x;
                float height = GetComponent<SpriteRenderer>().bounds.size.y;
                isLongObject = (width >= height);
            }
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
            mousePosition = new Vector3(6 * Mathf.Round(mousePosition.x / 6), 6 * Mathf.Floor(mousePosition.y / 6), mousePosition.z);

            //If object is rotated 90 degrees, we must fix its placement by 3 grid points on both axis
            if (isLongObject)
            {
                mousePosition = new Vector3(mousePosition.x + offsetX, mousePosition.y + offsetY, mousePosition.z);
            }
            // Offset rotated chests by half a tile to the right:
            if ((!isLongObject) && transform.name == "Chest Variant(Clone)")
            {
                mousePosition = new Vector3(mousePosition.x + 3, mousePosition.y, mousePosition.z);
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
        else if (isDragging) //User is placing an object down
        {
            if(isDoor) //Unique check for doors, since we do want overlapping objects
            {
                if(IsDoorCompatible()) //Check that door can be placed
                {
                    isDragging = false;
                }
            }
            else //All other objects do NOT want to have any overlapping objects
            {
                if (!IsOverlapping())
                {
                    isDragging = false;
                }
            }
        }
        else //User is trying to pick up a previously placed object
        {
            if(isDoor)
            {
                //Doors will be overlapping, so add extra branch so just allow all doors to be picked up again
                isDragging = true; 
            }
            else
            {
                //Non door objects cannot be placed with any overlapping furniture
                if (!IsOverlapping())
                {
                    isDragging = true;
                }
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

    //Checks if there are four overlapping wall instances.
    //If so, true. Otherwise return false
    //This is used for Door Objects.
    bool IsDoorCompatible()
    {
        //Get the collider of the door
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        //Calculate its size and center
        Vector2 colliderSize = new Vector2(boxCollider.size.x * transform.localScale.x, boxCollider.size.y * transform.localScale.y);

        Vector2 colliderCenter = (Vector2)transform.position + boxCollider.offset;

        //Get list of every collider overlapping the door
        Collider2D[] colliders = Physics2D.OverlapBoxAll(colliderCenter, colliderSize, 0f);

        int wallCount = 0;
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.tag == "WallBuddy")
            {
                //Check that the rotation of the wall matches the door.
                //To do that, get the rotations of this door object and compare it to the wall. If angle >= 180 then the rotation is comptaible (horizontal door = horizontal wall, and vertical door = vertical wall)
                float wallRotation = collider.gameObject.transform.rotation.eulerAngles.z; //The the rotation of the wall object (in degrees)
                float curRotation = this.transform.rotation.eulerAngles.z;
                if(Math.Abs(wallRotation - curRotation) >= 180 || Math.Abs(wallRotation - curRotation) == 90)
                {
                    wallCount++;
                    if (wallCount >= 3)
                    {
                        print("ClickDrop: Wall compatible!");
                        return true;
                    }
                }
            }
        }

        print("ClickDrop: Not compatible! Only " + wallCount + " walls!");
        return false;
    }
}
