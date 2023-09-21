using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    private Vector3 CameraPostion;

    [Header("Camera Settings")]
    public float cameraSpeed;

    private void Start()
    {
        CameraPostion = transform.position;
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
    }

}
