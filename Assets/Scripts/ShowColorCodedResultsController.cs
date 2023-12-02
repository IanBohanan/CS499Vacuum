using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.SceneManagement;
using UnityEditor.EditorTools;
using UnityEngine.Tilemaps;

public class ShowColorCodedResultsController : MonoBehaviour
{
    [SerializeField]
    private Tilemap tilemap;
    Button closeButton;
    #region OnEnable Class Setup
    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        closeButton = root.Q<Button>("ExitButton");
        closeButton.clicked += closeScene;
    }
    private void Start()
    {
        foreach (SerializableTile tile in InterSceneManager.cleanedTiles)
        {
            colorTile(tile);
        }
    }

    private void colorTile(SerializableTile tile)
    {
        //Change the color of the tile between white(ie. the base image) and tint

        //Okay Unity has some weird debug thing where it has a "lock color" flag for each tile.
        //Whenever setColor is called, ALL unlocked tiles get updated. So we have to unlock then lock each tile individually
        //Sooo just gonna have to unlock that flag for the tile, change the color, then lock the flag AGAIN.
        //Otherwise the entire tilemap gets updated and not just the one tile.
        tilemap.SetTileFlags(tile.pos, TileFlags.None);

        // Calculate the blend factor based on the number of clicks
        float blendFactor = Mathf.Clamp01((float)tile.hits / 100f);
        // Interpolate between red and green based on the blend factor
        Color tileColor = Color.Lerp(Color.red, Color.green, blendFactor);
        // Set the color of the tile
        tilemap.SetColor(tile.pos, tileColor);

        tilemap.SetTileFlags(tile.pos, TileFlags.LockColor);
    }

    private void closeScene()
    {
        //------------------------------------------------------
        //------------------------------------------------------
        //------------------------------------------------------
        // Check if there are any more algorithms to run.
        // If so, go back to the simulation scene,
        // otherwise, go to the data review scene
        (bool rand, bool spiral, bool snaking, bool wall) = InterSceneManager.getPathAlgs();

        if (rand || spiral || snaking || wall)
        {
            SceneManager.LoadScene(sceneName: "Simulation");
        }
        else
        {
            SceneManager.LoadScene(sceneName: "DataReview");
        }
    }
    #endregion
}
