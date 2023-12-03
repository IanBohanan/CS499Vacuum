// This script, UseOrEdit, is responsible for handling UI buttons that allow the player
// to choose between using a feature or entering an editing mode in a Unity game.
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.SceneManagement;
using UnityEditor.EditorTools;

public class UseOrEdit : MonoBehaviour
{
    // This is the name of the scene that will be loaded when the player clicks the "Use" button.

    #region UI Component References 
    Button useBtn;
    Button editBtn;
    #endregion
// Compare this snippet from Assets/Scripts/UseOrEdit.cs:
    #region OnEnable Class Setup
    private void OnEnable()
    {
        // Do UI things:
        exploreUI();
        assignCallbacks();
    }
    #endregion

    #region Explore UI and Assign Callbacks 
    private void exploreUI()
    {
        // Get UIDocument Root:
        VisualElement root = GetComponent<UIDocument>().rootVisualElement; // Get the root of the UI document.
        useBtn = root.Q<Button>("UseButton"); // Get the "Use" button from the UI document.
        editBtn = root.Q<Button>("EditButton"); // Get the "Edit" button from the UI document.
    }
    private void assignCallbacks()
    {
        // Assign callbacks to buttons:
       useBtn.clicked += () => SceneManager.LoadScene(sceneName: "SpawnTiles"); // Load the scene that allows the player to use the feature.
       editBtn.clicked += () => SceneManager.LoadScene(sceneName: "HouseBuilder"); //   Load the scene that allows the player to edit the layout.
    }
    #endregion
}
