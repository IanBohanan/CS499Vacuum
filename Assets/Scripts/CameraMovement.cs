using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This script controls the camera's movement and zoom in the game 
public class CameraMovement : MonoBehaviour
{
    // Variable to store the camera's position.
    private Vector3 CameraPostion;
    // Reference to the Camera component.
    private Camera cam;
    // Inspector-exposed settings for the camera.
    [Header("Camera Settings")]
    public float cameraSpeed; // The speed at which the camera moves.
    private float zoom; // The current zoom level of the camera.
    private float zoomMultiplier = 4f; // How much the zoom changes with input.
    private float minZoom = 20f; // Minimum allowed zoom level.
    private float maxZoom = 80f; // Maximum allowed zoom level.
    private float velocity = 0f; // Used by SmoothDamp to store the current velocity.
    private float smoothTime = 0.25f; // Time taken to smooth the zoom transition.
    
    // Start is called before the first frame update
    private void Start()
    {
        // Initialize the camera's position and the zoom level.
        CameraPostion = transform.position;
        cam = GetComponent<Camera>();
        zoom = cam.orthographicSize;

    }
    // Method to handle zoom input and apply the zoom effect.
    private void getZoom()
    {   
        // Get the scroll wheel input.
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        // Adjust the zoom level based on input, multiplied by the zoomMultiplier.
        zoom -= scroll * zoomMultiplier;
        // Clamp the zoom level to the min and max bounds.
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        // Smoothly transition to the new zoom level.
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, smoothTime);
    }
    // Update is called once per frame.
    private void Update()
    {   
        //Initialize a new Vector3 to store movement input
        Vector3 movement;
        // Get horizontal and vertical input and multiply by half of the cameraSpeed.
        movement.x = Input.GetAxisRaw("Horizontal") * (cameraSpeed*0.5f);
        movement.y = Input.GetAxisRaw("Vertical") * (cameraSpeed*0.5f);
        movement.z = 0;
        // Uncomment the following line if you want to ensure consistent movement speed in all directions.
        //movement = movement.normalized; // Normalize the movement vector so diagonal movement isn't faster.
        // Update the camera's position based on the movement input.
        CameraPostion += movement;
        // Apply the updated position to the camera's transform.
        transform.position = CameraPostion;
        // Call the getZoom method to handling the zooming
        getZoom();
    }

}
