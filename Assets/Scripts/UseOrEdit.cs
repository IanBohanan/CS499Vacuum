using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.SceneManagement;
using UnityEditor.EditorTools;

public class UseOrEdit : MonoBehaviour
{

    #region UI Component References
    Button useBtn;
    Button editBtn;
    #endregion

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
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        useBtn = root.Q<Button>("UseButton");
        editBtn = root.Q<Button>("EditButton");
    }
    private void assignCallbacks()
    {
       useBtn.clicked += () => SceneManager.LoadScene(sceneName: "SpawnTiles");
       editBtn.clicked += () => SceneManager.LoadScene(sceneName: "HouseBuilder");
    }
    #endregion
}
