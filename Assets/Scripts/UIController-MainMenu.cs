using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    // Define UI elements for the main menu
    Button importBtn;
    Button createNewBtn;
    Button reviewPastSimulationsBtn;

    VisualElement importPopup;
    Button importJSONButton;

    VisualElement dataReviewPopup;
    Button dataReviewConfirmButton;

    DropdownField selectionDropdown;
    DropdownField dataReviewDropdown;

    void OnEnable()
    {
        // Initialize UI components when the script is enabled

        // Access the root element of the UI Document attached to this GameObject
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Accessing and initializing primary button components from the UI
        VisualElement buttonContainer = root.Q<VisualElement>("ButtonContainer");
        Button importBtn = buttonContainer.Q<Button>("ImportButton");
        Button createNewBtn = buttonContainer.Q<Button>("CreateNewButton");
        reviewPastSimulationsBtn = root.Q<Button>("DataReviewButton");

        // Setup for the import popup, including dropdown and button
        importPopup = root.Q<VisualElement>("ImportPopup");
        selectionDropdown = importPopup.Q<DropdownField>("SelectionDropdown");
        importJSONButton = importPopup.Q<Button>("ImportJSONButton");

        // Setup for the data review popup, including dropdown and confirm button
        dataReviewPopup = root.Q<VisualElement>("DataReviewPopup");
        dataReviewDropdown = dataReviewPopup.Q<DropdownField>("DataReviewDropdown");
        dataReviewConfirmButton = dataReviewPopup.Q<Button>("DataReviewConfirmButton");

        // Subscribing to button click events to handle user interactions
        importBtn.clicked += () => onImportPress();
        createNewBtn.clicked += () => onCreateNewPress();
        importJSONButton.clicked += () => onImportPopupPress();
        selectionDropdown.RegisterValueChangedCallback(onValueChanged);
        reviewPastSimulationsBtn.clicked += () => onDataReviewPress();
        dataReviewDropdown.RegisterValueChangedCallback(onDataReviewValueChanged);
        dataReviewConfirmButton.clicked += () => onDataReviewConfirmPress();
    }

    // Handle change event for the house builder import file selection
    private void onValueChanged(ChangeEvent<string> evt)
    {
        InterSceneManager.fileSelection = selectionDropdown.value;
    }

    // Handle import button press to display the import panel
    public void onImportPress() {
        importPopup.style.display = DisplayStyle.Flex;
    }

    // Handle data review button press to display the data review panel
    public void onDataReviewPress()
    {
        dataReviewPopup.style.display = DisplayStyle.Flex;
    }

    // Handle change event for data review file selection
    private void onDataReviewValueChanged(ChangeEvent<string> evt)
    {
        // Logic to handle data review file selection can be added here
    }

    // Handle confirm button press in data review, load the data review scene
    private void onDataReviewConfirmPress()
    {
        dataReviewPopup.style.display = DisplayStyle.None;
        SceneManager.LoadScene(sceneName: "DataReview");
    }

    // Handle import button press in the import popup, load the house builder scene with the selected file
    public void onImportPopupPress(){
        importPopup.style.display = DisplayStyle.None;
        Debug.Log("Loading " + InterSceneManager.fileSelection);
        SceneManager.LoadScene(sceneName: "HouseBuilder");
    }

    // Handle create new button press, load the blank house creation scene
    public void onCreateNewPress() {
        SceneManager.LoadScene(sceneName:"HouseBuilder");
    }
}
