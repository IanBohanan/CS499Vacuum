using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
        //Set up a check later if the current grid is the Ui grid (where just round) or if the grid is the bigger black grid (divide by six THEN round)
        mousePosition = new Vector3(6*Mathf.Round(mousePosition.x/6), 6*Mathf.Round(mousePosition.y/6), mousePosition.z);
        transform.position = mousePosition;
    }
}