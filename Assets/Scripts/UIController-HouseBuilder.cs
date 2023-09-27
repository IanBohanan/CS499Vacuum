using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

enum CurrentState
{
    Default,
    WallPlacement,
    DoorPlacement,
    FurniturePlacement,
}

public class HouseBuilderUI : MonoBehaviour
{
    LayoutManager layoutManager;

    Label status; // "Invalid" or "Valid" shown in bottom left corner.

    VisualElement cancelBar;
    Button cancelBtn;

    Button exportBtn;
    Button importBtn;

    VisualElement exportSelectionContainer;
    string fileSelection = "";
    DropdownField exportDropdown;
    Button exportSelectionButton;

    VisualElement exportPopup;
    Button exportNoBtn;
    Button exportYesBtn;

    VisualElement clearPopup;
    Button clearNoBtn;
    Button clearYesBtn;

    VisualElement modeOptionsPanel;
    Button wallModeBtn;
    Button doorModeBtn;
    Button furnitureModeBtn;

    Button disabledWallModeBtn;
    Button disabledDoorModeBtn;
    Button disabledFurnitureModeBtn;

    VisualElement furnitureOptionsPanel; 
    Button chairBtn;
    Button tableBtn;
    Button chestBtn;

    CurrentState state = CurrentState.Default;

    #region OnEnable Class Setup
    private void OnEnable()
    {
        // Get the LayoutManager component:
        layoutManager = GetComponent<LayoutManager>();
        // Do UI things:
        exploreUI();
        assignCallbacks();
        UpdateState(CurrentState.Default);
    }
    #endregion

    #region Explore UI and Assign Callbacks
    private void exploreUI()
    {
        // Get UIDocument Root:
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Get Import/Export Components:
        VisualElement importExportPanel = root.Q<VisualElement>("ImportExportContainer");
        importBtn = importExportPanel.Q<Button>("Import");
        exportBtn = importExportPanel.Q<Button>("Export");

        // Get Bottom Content Components:
        VisualElement contentContainer = root.Q<VisualElement>("MainContentContainer");
        VisualElement headerContainer = contentContainer.Q<VisualElement>("HeaderContainer");
        cancelBar = headerContainer.Q<VisualElement>("CancelBar");
        cancelBtn = cancelBar.Q<Button>("CancelButton");
        VisualElement bodyContainer = contentContainer.Q<VisualElement>("BodyContainer");
        VisualElement statusPanel = contentContainer.Q<VisualElement>("StatusPanel");
        status = statusPanel.Q<Label>("StatusText");
        VisualElement selectionPanel = contentContainer.Q<VisualElement>("SelectionPanel");
        modeOptionsPanel = selectionPanel.Q<VisualElement>("ModeOptionsPanel");
        wallModeBtn = modeOptionsPanel.Q<Button>("WallButton");
        doorModeBtn = modeOptionsPanel.Q<Button>("DoorButton");
        furnitureModeBtn = modeOptionsPanel.Q<Button>("FurnitureButton");
        disabledWallModeBtn = modeOptionsPanel.Q<Button>("DisabledWallButton");
        disabledDoorModeBtn = modeOptionsPanel.Q<Button>("DisabledDoorButton");
        disabledFurnitureModeBtn = modeOptionsPanel.Q<Button>("DisabledFurnitureButton");
        furnitureOptionsPanel = selectionPanel.Q<VisualElement>("FurnitureOptionsPanel");
        chairBtn = furnitureOptionsPanel.Q<Button>("ChairButton");
        tableBtn = furnitureOptionsPanel.Q<Button>("TableButton");
        chestBtn = furnitureOptionsPanel.Q<Button>("ChestButton");

        // Get popup windows and buttons:
        exportSelectionContainer = root.Q<VisualElement>("ExportSelectionContainer");
        exportDropdown = exportSelectionContainer.Q<DropdownField>("ExportDropdown");
        exportSelectionButton = exportSelectionContainer.Q<Button>("ExportSelectionButton");
        exportPopup = root.Q<VisualElement>("ExportPopupContainer");
        VisualElement exportPopupButtonContainer = exportPopup.Q<VisualElement>("ExportButtonContainer");
        exportYesBtn = exportPopupButtonContainer.Q<Button>("ExportYesButton");
        exportNoBtn = exportPopupButtonContainer.Q<Button>("ExportNoButton");
        clearPopup = root.Q<VisualElement>("ClearPopupContainer");
        VisualElement clearPopupButtonContainer = clearPopup.Q<VisualElement>("ClearButtonContainer");
        clearYesBtn = clearPopupButtonContainer.Q<Button>("ClearYesButton");
        clearNoBtn = clearPopupButtonContainer.Q<Button>("ClearNoButton");
    }
    private void assignCallbacks()
    {
        // Assign button callback functions:
        importBtn.clicked += () => importPress();
        exportBtn.clicked += () => exportPress();
        cancelBtn.clicked += () => UpdateState(CurrentState.Default);
        wallModeBtn.clicked += () => wallModeToggle();
        doorModeBtn.clicked += () => doorModeToggle();
        furnitureModeBtn.clicked += () => furnitureModeToggle();
        exportDropdown.RegisterValueChangedCallback(OnDropdownValueChanged);
        exportSelectionButton.clicked += () => confirmExportSelection();
        exportYesBtn.clicked += () => exportConfirm(true);
        exportNoBtn.clicked += () => exportConfirm(false);
        clearYesBtn.clicked += () => clearConfirm(true);
        clearNoBtn.clicked += () => clearConfirm(false);
        chairBtn.clicked += () => layoutManager.addFurniture("chair");
        tableBtn.clicked += () => layoutManager.addFurniture("table");
        chestBtn.clicked += () => layoutManager.addFurniture("chest");
    }
    #endregion

    #region Button Callbacks
    private void OnDropdownValueChanged(ChangeEvent<string> evt)
    {
        // Update the file selection variable
        fileSelection = exportDropdown.value;
    }
    private void confirmExportSelection()
    {
        exportPopup.style.display = DisplayStyle.Flex;
        exportSelectionContainer.style.display = DisplayStyle.None;
    }
    public void importPress()
    {
        clearPopup.style.display = DisplayStyle.Flex;
    }

    public void exportPress()
    {
        // Show Popup:
        exportSelectionContainer.style.display = DisplayStyle.Flex;
    }

    private void exportConfirm(bool areYouSure)
    {
        // Save to JSON if confirmed:
        if (areYouSure) layoutManager.saveToJSON(fileSelection);
        // Hide Popup:
        exportPopup.style.display = DisplayStyle.None;
    }

    public void clearConfirm(bool areYouSure)
    {
        // Clear scene if confimed:
        if (areYouSure) layoutManager.clearAll();
        // Hide Popup:
        clearPopup.style.display = DisplayStyle.None;
    }

    public void wallModeToggle()
    {
        UpdateState(CurrentState.WallPlacement);
        Debug.Log("Wall mode active");
    }

    public void doorModeToggle()
    {
        UpdateState(CurrentState.DoorPlacement);
        Debug.Log("Door mode active");
    }

    public void furnitureModeToggle()
    {
        UpdateState(CurrentState.FurniturePlacement);
    }
    #endregion

    #region UI Management Methods
    public void updateStatus(bool isValid)
    {
        if (isValid)
        {
            status.text = "VALID";
            status.style.color = new StyleColor(Color.green);
        }
        else
        {
            status.text = "INVALID";
            status.style.color = new StyleColor(Color.red);
        }
    }

    public void showCancelButton(bool show)
    {
        if (show)
        {
            cancelBar.style.display = DisplayStyle.Flex;
        }
        else
        {
            cancelBar.style.display = DisplayStyle.None;
        }
    }
    #endregion

    private void UpdateState(CurrentState newState)
    {
        state = newState; // Update State

        // Update Button Availability:
        switch (newState)
        {
            case CurrentState.Default:
                {
                    // Show all normal buttons:
                    wallModeBtn.style.display = DisplayStyle.Flex;
                    doorModeBtn.style.display = DisplayStyle.Flex;
                    furnitureModeBtn.style.display = DisplayStyle.Flex;
                    // Hide all disabled buttons:
                    disabledWallModeBtn.style.display = DisplayStyle.None;
                    disabledDoorModeBtn.style.display = DisplayStyle.None;
                    disabledFurnitureModeBtn.style.display = DisplayStyle.None;
                    // Hide furniture options panel:
                    furnitureOptionsPanel.style.display = DisplayStyle.None;
                    // Show mode options panel:
                    modeOptionsPanel.style.display = DisplayStyle.Flex;
                    // Hide cancel bar:
                    showCancelButton(false);
                    break;
                }
            case CurrentState.WallPlacement:
                {
                    wallModeBtn.style.display = DisplayStyle.Flex;
                    doorModeBtn.style.display = DisplayStyle.None;
                    furnitureModeBtn.style.display = DisplayStyle.None;
                    // Hide all disabled buttons:
                    disabledWallModeBtn.style.display = DisplayStyle.None;
                    disabledDoorModeBtn.style.display = DisplayStyle.Flex;
                    disabledFurnitureModeBtn.style.display = DisplayStyle.Flex;
                    // Show cancel bar:
                    showCancelButton(true);
                    break;
                }
            case CurrentState.DoorPlacement:
                {
                    wallModeBtn.style.display = DisplayStyle.None;
                    doorModeBtn.style.display = DisplayStyle.Flex;
                    furnitureModeBtn.style.display = DisplayStyle.None;
                    // Hide all disabled buttons:
                    disabledWallModeBtn.style.display = DisplayStyle.Flex;
                    disabledDoorModeBtn.style.display = DisplayStyle.None;
                    disabledFurnitureModeBtn.style.display = DisplayStyle.Flex;
                    // Show cancel bar:
                    showCancelButton(true);
                    break;
                }
            case CurrentState.FurniturePlacement:
                {
                    // wallModeBtn.style.display = DisplayStyle.None;
                    // doorModeBtn.style.display = DisplayStyle.None;
                    // furnitureModeBtn.style.display = DisplayStyle.Flex;
                    // Hide all disabled buttons:
                    // disabledWallModeBtn.style.display = DisplayStyle.Flex;
                    // disabledDoorModeBtn.style.display = DisplayStyle.Flex;
                    // disabledFurnitureModeBtn.style.display = DisplayStyle.None;

                    // Hide mode options panel:
                    modeOptionsPanel.style.display = DisplayStyle.None;
                    // Show furniture options panel:
                    furnitureOptionsPanel.style.display = DisplayStyle.Flex;
                    // Show cancel bar:
                    showCancelButton(true);
                    break;
                }
        }
    }
}
