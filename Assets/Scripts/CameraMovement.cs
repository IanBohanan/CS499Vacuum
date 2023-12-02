using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    private Vector3 CameraPostion;
    private Camera cam;

    [Header("Camera Settings")]
    public float cameraSpeed; //How fast the camera speed is
    private float zoom;
    private float zoomMultiplier = 4f;
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
