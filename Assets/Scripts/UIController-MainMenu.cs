using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// Controller class for the Main Menu UI and functionality
public class MainMenuController : MonoBehaviour
{
    // UI Button References
    Button importBtn;
    Button createNewBtn;
    Button reviewPastSimulationsBtn;

    // Popup UI Elements
    VisualElement importPopup;
    Button importJSONButton;
    VisualElement dataReviewPopup;
    Button dataReviewConfirmButton;

    // Dropdowns for selecting files
    DropdownField selectionDropdown;
    DropdownField dataReviewDropdown;

    // Method called when the script is enabled
    void OnEnable()
    {
        // Initialize UI components and assign button callbacks
        // Get UIDocument Root:
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Initialize buttons:
        VisualElement buttonContainer = root.Q<VisualElement>("ButtonContainer");
        Button importBtn = buttonContainer.Q<Button>("ImportButton");
        Button createNewBtn = buttonContainer.Q<Button>("CreateNewButton");
        reviewPastSimulationsBtn = root.Q<Button>("DataReviewButton");

        // Initialize Import Popup, Dropdown, & Button:
        importPopup = root.Q<VisualElement>("ImportPopup");
        selectionDropdown = importPopup.Q<DropdownField>("SelectionDropdown");
        importJSONButton = importPopup.Q<Button>("ImportJSONButton");

        // Initialize Data Review Popup, Dropdown, & Button:
        dataReviewPopup = root.Q<VisualElement>("DataReviewPopup");
        dataReviewDropdown = dataReviewPopup.Q<DropdownField>("DataReviewDropdown");
        dataReviewConfirmButton = dataReviewPopup.Q<Button>("DataReviewConfirmButton");

        // Assign button callback functions:
        importBtn.clicked += () => onImportPress();
        createNewBtn.clicked += () => onCreateNewPress();
        importJSONButton.clicked += () => onImportPopupPress();
        selectionDropdown.RegisterValueChangedCallback(onValueChanged);
        reviewPastSimulationsBtn.clicked += () => onDataReviewPress();
        dataReviewDropdown.RegisterValueChangedCallback(onDataReviewValueChanged);
        dataReviewConfirmButton.clicked += () => onDataReviewConfirmPress();
    }

    // Callback methods for UI actions
    private void onValueChanged(ChangeEvent<string> evt)
    {
        // Update the file selection from the dropdown
    }

    public void onImportPress() 
    {
        // Display the import popup
    }

    public void onDataReviewPress()
    {
        // Display the data review popup
    }

    private void onDataReviewValueChanged(ChangeEvent<string> evt)
    {
        // Update the data review file selection
    }

    private void onDataReviewConfirmPress()
    {
        // Hide data review popup and load the data review scene
    }

    public void onImportPopupPress()
    {
        // Hide import popup and load the house builder scene with the selected file
    }

    public void onCreateNewPress() 
    {
        // Load a new, blank house creation scene
    }
}
