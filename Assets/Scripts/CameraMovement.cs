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

    private void getZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoom -= scroll * zoomMultiplier;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, smoothTime);
    }

    private void Update()
    {
        Vector3 movement;
        movement.x = Input.GetAxisRaw("Horizontal") * (cameraSpeed*0.5f);
        movement.y = Input.GetAxisRaw("Vertical") * (cameraSpeed*0.5f);
        movement.z = 0;
        //movement = movement.normalized; //normalize the movement vector so diagonal movement isn't faster
        CameraPostion += movement;
        transform.position = CameraPostion;

        getZoom();
    }

}
