//This script is a unique object that can hold the information per-tile.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// This attribute allows the creation of TileData assets from the Unity Editor
[CreateAssetMenu]
public class TileData : ScriptableObject
{
    // Array to hold references to different tile types
    public TileBase[] tiles; // Tiles associated with this data

    // Cleanliness value for the tile, can be used to store how clean/dirty a tile is
    public float cleanliness; // Float to represent the cleanliness level of the tile

    // Enumeration to define different types of tiles
    enum Type { HARD, LOOP, CUT, FREEZECUT }; // Types of tiles (e.g., different flooring types or characteristics)
}