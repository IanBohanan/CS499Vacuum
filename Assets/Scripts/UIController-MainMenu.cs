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

    string fileSelection;
    DropdownField selectionDropdown;

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
        selectionDropdown = importPopup.Q<DropdownField>("SelectionDropdown");
        importJSONButton = importPopup.Q<Button>("ImportJSONButton");

        // Subscribe to button events:
        importBtn.clicked += () => onImportPress();
        createNewBtn.clicked += () => onCreateNewPress();
        importJSONButton.clicked += () => onImportPopupPress();
        selectionDropdown.RegisterValueChangedCallback(onValueChanged);
    }

    // Update selected import fule:
    private void onValueChanged(ChangeEvent<string> evt)
    {
        fileSelection = selectionDropdown.value;
    }

    // Open the JSON import panel:
    public void onImportPress() {
        importPopup.style.display = DisplayStyle.Flex;
    }

    // Load in the imported house creation scene:
    public void onImportPopupPress(){
        importPopup.style.display = DisplayStyle.None;
        Debug.Log("Loading " + fileSelection);
        SceneManager.LoadScene(sceneName: "HouseBuilder");
    }

    // Load in the blank house creation scene:
    public void onCreateNewPress() {
        SceneManager.LoadScene(sceneName:"HouseBuilder");
    }
}
