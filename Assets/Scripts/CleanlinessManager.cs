// This script is responsible for controlling the camera's movement and zoom in a Unity 2D game.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Variables to store camera position and camera component
    private Vector3 CameraPostion;  // Stores the current camera position
    private Camera cam;             // Reference to the camera component

    [Header("Camera Settings")]
    public float cameraSpeed;       // Speed of camera movement
    private float zoom;             // Current camera zoom level
    private float zoomMultiplier = 4f; // Multiplier for zoom sensitivity
    private float minZoom = 20f;    // Minimum allowed zoom level
    private float maxZoom = 80f;    // Maximum allowed zoom level
    private float velocity = 0f;    // Velocity for smooth zooming
    private float smoothTime = 0.25f; // Smoothness factor for zooming

    private void Start()
    {
        // Initialize camera position and get a reference to the Camera component
        CameraPostion = transform.position;
        cam = GetComponent<Camera>();
        zoom = cam.orthographicSize;
    }

    private void getZoom()
    {
        // Adjust the camera's orthographic size (zoom) based on mouse scroll input
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoom -= scroll * zoomMultiplier;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        // Smoothly transition to the new zoom level
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, smoothTime);
    }

    private void Update()
    {
        // Capture user input for camera movement
        Vector3 movement;
        movement.x = Input.GetAxisRaw("Horizontal") * (cameraSpeed * 0.5f);
        movement.y = Input.GetAxisRaw("Vertical") * (cameraSpeed * 0.5f);
        movement.z = 0;
        // Update the camera position based on user input
        CameraPostion += movement;
        transform.position = CameraPostion;

        // Call the getZoom() function to handle zooming based on mouse scroll input
        getZoom();
    }
}
