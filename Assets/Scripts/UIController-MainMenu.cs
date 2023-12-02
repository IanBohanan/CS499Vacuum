using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
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
        // Get UIDocument Root:
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Get Primary Button Components:
        VisualElement buttonContainer = root.Q<VisualElement>("ButtonContainer");
        Button importBtn = buttonContainer.Q<Button>("ImportButton");
        Button createNewBtn = buttonContainer.Q<Button>("CreateNewButton");
        reviewPastSimulationsBtn = root.Q<Button>("DataReviewButton");

        // Get Import Popup, Dropdown, & Button:
        importPopup = root.Q<VisualElement>("ImportPopup");
        selectionDropdown = importPopup.Q<DropdownField>("SelectionDropdown");
        importJSONButton = importPopup.Q<Button>("ImportJSONButton");

        // Get Data Review Popup, Dropdown, & Button:
        dataReviewPopup = root.Q<VisualElement>("DataReviewPopup");
        dataReviewDropdown = dataReviewPopup.Q<DropdownField>("DataReviewDropdown");
        dataReviewConfirmButton = dataReviewPopup.Q<Button>("DataReviewConfirmButton");

        // Subscribe to button events:
        importBtn.clicked += () => onImportPress();
        createNewBtn.clicked += () => onCreateNewPress();
        importJSONButton.clicked += () => onImportPopupPress();
        selectionDropdown.RegisterValueChangedCallback(onValueChanged);
        reviewPastSimulationsBtn.clicked += () => onDataReviewPress();
        dataReviewDropdown.RegisterValueChangedCallback(onDataReviewValueChanged);
        dataReviewConfirmButton.clicked += () => onDataReviewConfirmPress();
    }

    // Update selected house builder import file:
    private void onValueChanged(ChangeEvent<string> evt)
    {
        InterSceneManager.fileSelection = selectionDropdown.value;
    }

    // Open the JSON house builder import panel:
    public void onImportPress() {
        importPopup.style.display = DisplayStyle.Flex;
    }

    // Open the JSON data review import panel:
    public void onDataReviewPress()
    {
        dataReviewPopup.style.display = DisplayStyle.Flex;
    }

    // Update selected data review import file:
    private void onDataReviewValueChanged(ChangeEvent<string> evt)
    {
        //InterSceneManager.dataReviewFileSelection = dataReviewDropdown.value;
    }

    private void onDataReviewConfirmPress()
    {
        dataReviewPopup.style.display = DisplayStyle.None;
        SceneManager.LoadScene(sceneName: "DataReview");
    }

    // Load in the imported house creation scene:
    public void onImportPopupPress(){
        importPopup.style.display = DisplayStyle.None;
        Debug.Log("Loading " + InterSceneManager.fileSelection);
        SceneManager.LoadScene(sceneName: "HouseBuilder");
    }

    // Load in the blank house creation scene:
    public void onCreateNewPress() {
        SceneManager.LoadScene(sceneName:"HouseBuilder");
    }
}
