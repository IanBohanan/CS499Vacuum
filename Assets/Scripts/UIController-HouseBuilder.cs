using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.SceneManagement;
using UnityEditor.EditorTools;

enum CurrentState
{
    Default,
    WallPlacement,
    DoorPlacement,
    FurniturePlacement,
    DeleteObjects,
}

public class HouseBuilderUI : MonoBehaviour
{
    LayoutManager layoutManager;

    string exportFileSelection = "";

    [SerializeField] GameObject FlagPrefab;

    public static event Action<string> stateUpdate; //An action that broadcasts when the UI's state has changed. When invoked, sends a string that matches the name of the new state

    #region UI Component References
    Label status; // "Invalid" or "Valid" shown in bottom left corner.
    Label statusLabel;
    VisualElement cancelBar;
    Button cancelBtn;
    Button exportBtn;
    Button importBtn;
    VisualElement exportSelectionContainer;
    DropdownField exportDropdown;
    Button exportSelectionButton;
    VisualElement exportPopup;
    Button exportNoBtn;
    Button exportYesBtn;
    VisualElement validityPopup;
    Button validityConfirmBtn;
    Button validityProblemBtn;
    VisualElement clearPopup;
    Button clearNoBtn;
    Button clearYesBtn;
    VisualElement modeOptionsPanel;
    Button deleteButton;
    Button flagButton;
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

    GameObject roomManager;
    #endregion

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
        RoomManager.finishedFlooding += updateStatus;
        RoomManager.unableToFlood += unableToFlood;
    }
    #endregion

    #region Explore UI and Assign Callbacks
    private void exploreUI()
    {
        // Get RoomManager:
        roomManager = GameObject.Find("RoomManager");

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
        status = root.Q<Label>("StatusText");
        statusLabel = root.Q<Label>("StatusLabel");
        VisualElement selectionPanel = contentContainer.Q<VisualElement>("SelectionPanel");
        modeOptionsPanel = selectionPanel.Q<VisualElement>("ModeOptionsPanel");
        deleteButton = modeOptionsPanel.Q<Button>("RemoveFurniture");
        flagButton = modeOptionsPanel.Q<Button>("PlaceFlag");
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
        validityPopup = root.Q<VisualElement>("ValidityCheckPopup");
        validityConfirmBtn = root.Q<Button>("ValidityConfirmButton");
        validityProblemBtn = root.Q<Button>("ValidityProblemButton");
    }
    private void assignCallbacks()
    {
        // Assign button callback functions:
        importBtn.clicked += () => importPress();
        exportBtn.clicked += () => exportPress();
        cancelBtn.clicked += () => UpdateState(CurrentState.Default);
        deleteButton.clicked += () => deleteModeToggle();
        flagButton.clicked += () => placeFlag();
        wallModeBtn.clicked += () => wallModeToggle();
        doorModeBtn.clicked += () => layoutManager.addFurniture("door", Quaternion.identity);//doorModeToggle();
        furnitureModeBtn.clicked += () => furnitureModeToggle();
        exportDropdown.RegisterValueChangedCallback(OnDropdownValueChanged);
        exportSelectionButton.clicked += () => confirmExportSelection();
        validityConfirmBtn.clicked += () => validityConfirmPress();
        validityProblemBtn.clicked += () => validityProblemPress();
        exportYesBtn.clicked += () => exportConfirm(true);
        exportNoBtn.clicked += () => exportConfirm(false);
        clearYesBtn.clicked += () => clearConfirm(true);
        clearNoBtn.clicked += () => clearConfirm(false);
        chairBtn.clicked += () => layoutManager.addFurniture("chair", Quaternion.identity);
        tableBtn.clicked += () => layoutManager.addFurniture("table", Quaternion.identity);
        chestBtn.clicked += () => layoutManager.addFurniture("chest", Quaternion.identity);
    }
    #endregion

    #region Button Callbacks
    private void OnDropdownValueChanged(ChangeEvent<string> evt)
    {
        // Update the file selection variable
        exportFileSelection = exportDropdown.value;
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
        // Disable Chair/Table colliders:
        // Disable colliders for chairs/tables:
        GameObject[] objectsThatShouldntHaveColliders = GameObject.FindGameObjectsWithTag("NoColliderBuddy");
        foreach (GameObject obj in objectsThatShouldntHaveColliders)
        {
            obj.GetComponent<BoxCollider2D>().enabled = false;
        }
        Debug.Log("FUCKEJRE: "+objectsThatShouldntHaveColliders.Length);
        // Start flood fill:
        setStatusWaiting();
        string result = roomManager.GetComponent<RoomManager>().beginFlood();
        // Show Popup:
        validityPopup.style.display = DisplayStyle.Flex;
        Camera cam = Camera.main;
        Vector3 newCamPosition = new Vector3(cam.transform.position.x, cam.transform.position.y + 50000, cam.transform.position.z);
        cam.transform.position = newCamPosition;
    }

    private void validityConfirmPress()
    {
        validityPopup.style.display = DisplayStyle.None;
        exportSelectionContainer.style.display = DisplayStyle.Flex;
    }

    private void validityProblemPress()
    {
        validityPopup.style.display = DisplayStyle.None;
        // Re-enable colliders for chairs/tables:
        GameObject[] objectsThatShouldntHaveColliders = GameObject.FindGameObjectsWithTag("NoColliderBuddy");
        foreach (GameObject obj in objectsThatShouldntHaveColliders)
        {
            obj.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    private void exportConfirm(bool areYouSure)
    {
        // Save to JSON if confirmed:
        if (areYouSure) 
        { 
            layoutManager.saveToJSON(exportFileSelection);
            InterSceneManager.fileSelection = exportDropdown.value;
            InterSceneManager.wallList.Clear(); // Clear the list of walls in case user returns to house builder
            InterSceneManager.flagList.Clear();
            SceneManager.LoadScene(sceneName: "SimulationSetup"); 
        }
            // Hide Popup:
            exportPopup.style.display = DisplayStyle.None;
        Camera cam = Camera.main;
        Vector3 newCamPosition = new Vector3(cam.transform.position.x, cam.transform.position.y - 50000, cam.transform.position.z);
        cam.transform.position = newCamPosition;
        // Re-enable colliders for chairs/tables:
        GameObject[] objectsThatShouldntHaveColliders = GameObject.FindGameObjectsWithTag("NoColliderBuddy");
        foreach (GameObject obj in objectsThatShouldntHaveColliders)
        {
            obj.GetComponent<BoxCollider2D>().enabled = true;
        }
    }

    public void clearConfirm(bool areYouSure)
    {
        // Clear scene if confimed:
        if (areYouSure) layoutManager.clearAll();
        // Hide Popup:
        clearPopup.style.display = DisplayStyle.None;
    }

    //Connect me to wall placement: IAN

    private void deleteModeToggle()
    {
        if (!InterSceneManager.deleteMode)
        {
            UpdateState(CurrentState.DeleteObjects);
            InterSceneManager.deleteMode = true;
            deleteButton.style.unityBackgroundImageTintColor = new Color(1,0,0,0.5f);
            deleteButton.style.unityBackgroundImageTintColor = new Color(1, 0, 0, 0.5f);
            deleteButton.style.borderBottomColor = new Color(1, 0, 0, 0.5f);
            deleteButton.style.borderLeftColor = new Color(1, 0, 0, 0.5f);
            deleteButton.style.borderRightColor = new Color(1, 0, 0, 0.5f);
            deleteButton.style.borderTopColor = new Color(1, 0, 0, 0.5f);
        }
        else
        {
            UpdateState(CurrentState.Default);
            InterSceneManager.deleteMode = false;
            deleteButton.style.unityBackgroundImageTintColor = new Color(1, 1, 1, 0.5f);
            deleteButton.style.borderBottomColor = new Color(1, 1, 1, 0.5f);
            deleteButton.style.borderLeftColor = new Color(1, 1, 1, 0.5f);
            deleteButton.style.borderRightColor = new Color(1, 1, 1, 0.5f);
            deleteButton.style.borderTopColor = new Color(1, 1, 1, 0.5f);
        }
    }

    private void placeFlag()
    {
        GameObject newFlag = Instantiate(FlagPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newFlag.GetComponent<ClickDrop>().isDragging = true;
        Debug.Log(InterSceneManager.flagList.Count);
        InterSceneManager.flagList.Add(newFlag);
    }

    public void wallModeToggle()
    {
        UpdateState(CurrentState.WallPlacement);

        Debug.Log("UI: Wall mode active");
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
            statusLabel.style.display = DisplayStyle.Flex;
            statusLabel.text = "If you've set your flags up correctly, then this layout is...";
            status.text = "VALID";
            status.style.color = new StyleColor(Color.green);
            validityConfirmBtn.style.display = DisplayStyle.Flex;
            validityProblemBtn.style.display = DisplayStyle.None;
        }
        else
        {
            statusLabel.style.display = DisplayStyle.Flex;
            statusLabel.text = "If you've set your flags up correctly, then this layout is...";
            status.text = "INVALID";
            status.style.color = new StyleColor(Color.red);
            validityConfirmBtn.style.display = DisplayStyle.None;
            validityProblemBtn.style.display = DisplayStyle.Flex;
        }
    }

    public void setStatusWaiting()
    {
        status.text = "Checking Layout...";
        statusLabel.style.display = DisplayStyle.None;
        status.style.color = new StyleColor(Color.white);
    }

    public void unableToFlood(bool ableToFlood)
    {
        statusLabel.style.display = DisplayStyle.Flex;
        statusLabel.text = "At least 2 flags are required to check layout validity. Proceed at your own risk.";
        status.text = "Unknown";
        status.style.color = new StyleColor(Color.yellow);
        validityConfirmBtn.style.display = DisplayStyle.Flex;
        validityProblemBtn.style.display = DisplayStyle.Flex;
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

        stateUpdate?.Invoke(newState.ToString()); //Let subscribers know the UI's state has updated, sending the current state as a String

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
                    // Hide mode options panel:
                    modeOptionsPanel.style.display = DisplayStyle.None;
                    // Show furniture options panel:
                    furnitureOptionsPanel.style.display = DisplayStyle.Flex;
                    // Show cancel bar:
                    showCancelButton(true);
                    break;
                }
            case CurrentState.DeleteObjects:
                {
                    wallModeBtn.style.display = DisplayStyle.None;
                    doorModeBtn.style.display = DisplayStyle.None;
                    furnitureModeBtn.style.display = DisplayStyle.None;
                    // Hide all disabled buttons:
                    disabledWallModeBtn.style.display = DisplayStyle.Flex;
                    disabledDoorModeBtn.style.display = DisplayStyle.Flex;
                    disabledFurnitureModeBtn.style.display = DisplayStyle.Flex;
                    break;
                }
        }
    }
}
