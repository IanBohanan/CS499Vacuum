// The script, Flag, is used to mark specific locations within rooms in a house-building scene.
// These flags help ensure that every room is accessible and connected. 
// Each flag object has a "roomName" variable, which denotes the name of the room to which it belongs.
// These flags are typically used to coordinate and track various aspects of room generation, navigation, or other game mechanics.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//The flag for each room. Used in the housebuilder scene to make sure every room is accessible.
//It denotes a space that's in each room
public class Flag : MonoBehaviour
{

    public string roomName = "Room A"; // The name of the room associated with this flag.

}
