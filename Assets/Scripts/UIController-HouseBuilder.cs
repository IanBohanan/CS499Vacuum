using UnityEngine;
using UnityEngine.UIElements;

enum CurrentState
{
    Default,
    WallPlacement,
    DoorPlacement,
    FurniturePlacement,
}

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject Chair;
    [SerializeField] GameObject Table;
    [SerializeField] GameObject Chest;

    Label status; // "Invalid" or "Valid" shown in bottom left corner.

    VisualElement cancelBar;
    Button cancelBtn;

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
        // Get UIDocument Root:
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Get Import/Export Components:
        VisualElement importExportPanel = root.Q<VisualElement>("ImportExportContainer");
        Button importBtn = importExportPanel.Q<Button>("Import");
        Button exportBtn = importExportPanel.Q<Button>("Export");

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
        exportPopup = root.Q<VisualElement>("ExportPopupContainer");
        VisualElement exportPopupButtonContainer = exportPopup.Q<VisualElement>("ExportButtonContainer");
        exportYesBtn = exportPopupButtonContainer.Q<Button>("ExportYesButton");
        exportNoBtn = exportPopupButtonContainer.Q<Button>("ExportNoButton");
        clearPopup = root.Q<VisualElement>("ClearPopupContainer");
        VisualElement clearPopupButtonContainer = clearPopup.Q<VisualElement>("ClearButtonContainer");
        clearYesBtn = clearPopupButtonContainer.Q<Button>("ClearYesButton");
        clearNoBtn = clearPopupButtonContainer.Q<Button>("ClearNoButton");

        // Assign button callback functions:
        importBtn.clicked += () => importPress();
        exportBtn.clicked += () => exportPress();
        cancelBtn.clicked += () => UpdateState(CurrentState.Default);
        wallModeBtn.clicked += () => wallModeToggle();
        doorModeBtn.clicked += () => doorModeToggle();
        furnitureModeBtn.clicked += () => furnitureModeToggle();
        exportYesBtn.clicked += () => exportYesPress();
        exportNoBtn.clicked += () => exportNoPress();
        clearYesBtn.clicked += () => clearYesPress();
        clearNoBtn.clicked += () => clearNoPress();
        chairBtn.clicked += () => chairModeActivate();
        tableBtn.clicked += () => tableModeActivate();
        chestBtn.clicked += () => chestModeActivate();

        UpdateState(CurrentState.Default);
    }
    #endregion

    #region Button Callbacks
    public void importPress()
    {
        Debug.Log("Importing House...");
        clearPopup.style.display = DisplayStyle.Flex;
    }

    public void exportPress()
    {
        Debug.Log("Exporting House...");
        exportPopup.style.display = DisplayStyle.Flex;
    }

    public void exportYesPress()
    {
        Debug.Log("Export confimed...");
        exportPopup.style.display = DisplayStyle.None;
    }

    public void exportNoPress()
    {
        Debug.Log("Export cancelled...");
        exportPopup.style.display = DisplayStyle.None;
    }

    public void clearYesPress()
    {
        Debug.Log("Clear confirmed...");
        clearPopup.style.display = DisplayStyle.None;
    }

    public void clearNoPress()
    {
        Debug.Log("Clear cancelled...");
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

    public void chairModeActivate()
    {

        Instantiate(Chair, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void chestModeActivate()
    {
        Instantiate(Chest, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void tableModeActivate()
    {
        Instantiate(Table, new Vector3(0, 0, 0), Quaternion.identity);
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
