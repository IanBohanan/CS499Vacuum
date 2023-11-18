using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// CleanlinessManager handles the cleanliness state of each tile in the environment.
public class CleanlinessManager : MonoBehaviour
{
    public float debugCleanAmount; // Amount used for debugging cleanliness changes.

    [SerializeField]
    private Tilemap floorTiles; // Reference to the tilemap containing the floor tiles.
    private Vacuum vacuum; // Reference to the vacuum object in the scene.

    private Dictionary<Vector3Int, float> tileData = new Dictionary<Vector3Int, float>(); // Dictionary to track the cleanliness of each tile.

    [SerializeField]
    private Color cleanColor; // The color used to represent a clean tile.

    // Public method to clean a tile at a given world position by a specified amount.
    public void makeClean(Vector2 worldPosition, float cleanAmount)
    {
        Vector3Int gridPosition = floorTiles.WorldToCell(worldPosition);
        changeCleanliness(gridPosition, cleanAmount);
        VisualizeCleanliness();
    }

    // Private method to change the cleanliness of a tile.
    // Ensures that the cleanliness value is clamped between 0 and 100.
    private void changeCleanliness(Vector3Int gridPosition, float toChange)
    {
        // Add tile to dictionary if it's not already there, starting at 0% cleanliness.
        if (!tileData.ContainsKey(gridPosition))
            tileData.Add(gridPosition, 0f);

        float newValue = tileData[gridPosition] + toChange;
        // Clamp the cleanliness value to ensure it's between 0 and 100.
        tileData[gridPosition] = Mathf.Clamp(newValue, 0f, 100f);

        // Debug log to print the new cleanliness value.
        print("CleanlinessManager: Tile cleanliness: " + tileData[gridPosition]);
    }

    // Visualizes the cleanliness of each tile by changing its color.
    private void VisualizeCleanliness()
    {
        foreach (var tile in tileData)
        {
            // Lerp the color between white (dirty) and green (clean) based on cleanliness percentage.
            Color newTileColor = Color.Lerp(Color.white, Color.green, tile.Value / 100);

            // Unlock and lock tile color to update its color individually due to Unity's tilemap behavior.
            floorTiles.SetTileFlags(tile.Key, TileFlags.None);
            floorTiles.SetColor(tile.Key, newTileColor);
            floorTiles.SetTileFlags(tile.Key, TileFlags.LockColor);
        }
    }

    // Debug method to allow cleaning tiles by clicking on them.
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            makeClean(mousePosition, debugCleanAmount);
        }
    }
}
