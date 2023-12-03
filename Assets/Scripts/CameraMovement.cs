using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    private Vector3 CameraPostion;
    private Camera cam;

    [Header("Camera Settings")]
    public float cameraSpeed; //How fast the default camera speed is. Determined in the Unity editor
    public float scaledCameraSpeed; //The actual used camera speed determined by the zoom level
    public float zoom;
    private float zoomMultiplier = 12f; //How fast the camera should move in and out
    public float zoomSpeedMultiplier = 10f; //How much the zoom should affect the camera's movement speed
    public float maxMoveSpeed = 2f;
    private float minZoom = 20f;
    private float maxZoom = 150f;
    private float velocity = 0f;
    private float smoothTime = 0.25f;

    private void Start()
    {
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

        //Then make the camera faster/slower depending on how zoomed in it is
        // Use Mathf.InverseLerp to get a t value between 0 and 1 based on the zoom level
        float t = Mathf.InverseLerp(minZoom, maxZoom, zoom);

        // Use Mathf.Lerp to interpolate between baseMoveSpeed and maxMoveSpeed based on t
        scaledCameraSpeed = Mathf.Lerp(cameraSpeed, maxMoveSpeed, t);
    }

    private void Update()
    {
        getZoom();
        Vector3 movement;
        movement.x = Input.GetAxisRaw("Horizontal") * (scaledCameraSpeed * 0.5f);
        movement.y = Input.GetAxisRaw("Vertical") * (scaledCameraSpeed * 0.5f);
        movement.z = 0;
        //movement = movement.normalized; //normalize the movement vector so diagonal movement isn't faster
        CameraPostion += movement;
        transform.position = CameraPostion;

        
    }

}
