//This script is a unique object that can hold the information per-tile.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public TileBase[] tiles;

    public float cleanliness;
    public bool occupied;

    enum Type { HARD, LOOP, CUT, FREEZECUT };
}
