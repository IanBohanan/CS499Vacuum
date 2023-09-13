using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    Button importBtn;
    Button createNewBtn;

    VisualElement importPopup;
    Button importJSONButton;

    void OnEnable()
    {
        // Get UIDocument Root:
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Get Primary Button Components:
        VisualElement buttonContainer = root.Q<VisualElement>("ButtonContainer");
        Button importBtn = buttonContainer.Q<Button>("ImportButton");
        Button createNewBtn = buttonContainer.Q<Button>("CreateNewButton");

        // Get Import Popup Panel & Button:
        importPopup = root.Q<VisualElement>("ImportPopup");
        importJSONButton = importPopup.Q<Button>("ImportJSONButton");

        // Subscribe to button events:
        importBtn.clicked += () => onImportPress();
        createNewBtn.clicked += () => onCreateNewPress();
        importJSONButton.clicked += () => onImportPopupPress();
    }

    // Open the JSON import panel
    public void onImportPress() {
        importPopup.style.display = DisplayStyle.Flex;
    }

    public void onImportPopupPress(){
        importPopup.style.display = DisplayStyle.None;
    }

    // Load in the house creation scene
    public void onCreateNewPress() {
        SceneManager.LoadScene (sceneName:"SampleScene");
    }
}