//This script is a unique object that can hold the information per-tile.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// This attribute allows the creation of TileData assets from the Unity Editor
[CreateAssetMenu]
public class TileData : ScriptableObject
{
    // The tile that will be placed on this tile.
    public TileBase[] tiles;

    public float cleanliness;
// Enumeration to define different types of tiles
    enum Type { HARD, LOOP, CUT, FREEZECUT };
}
