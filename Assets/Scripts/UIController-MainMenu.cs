using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    Button importBtn;
    Button createNewBtn;

    void OnEnable()
    {
        // Get UIDocument Root:
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Get Button Components:
        VisualElement buttonContainer = root.Q<VisualElement>("ButtonContainer");
        Button importBtn = buttonContainer.Q<Button>("ImportButton");
        Button createNewBtn = buttonContainer.Q<Button>("CreateNewButton");

        // Subscribe to button events:
        importBtn.clicked += () => onImportPress();
        createNewBtn.clicked += () => onCreateNewPress();
    }

    // Open the JSON import panel
    public void onImportPress() {
        Debug.Log("Import JSON");
    }

    // Load in the house creation scene
    public void onCreateNewPress() {
        SceneManager.LoadScene (sceneName:"SampleScene");
    }
}