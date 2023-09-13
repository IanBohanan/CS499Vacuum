using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject GenericFurniture;

    Label status; // "Invalid" or "Valid" shown in bottom left corner.

    VisualElement cancelBar; 
    Button cancelBtn;

    VisualElement exportPopup;
    Button exportNoBtn;
    Button exportYesBtn;

    VisualElement clearPopup;
    Button clearNoBtn;
    Button clearYesBtn;

    enum CurrentState
    {
        Default,
        WallPlacement,
        DoorPlacement,
        FurniturePlacement,
    }

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
        Button wallModeBtn = selectionPanel.Q<Button>("WallButton");
        Button doorModeBtn = selectionPanel.Q<Button>("DoorButton");
        Button furnitureModeBtn = selectionPanel.Q<Button>("FurnitureButton");

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
        cancelBtn.clicked += () => cancelPress();
        wallModeBtn.clicked += () => wallModeToggle();
        doorModeBtn.clicked += () => doorModeToggle();
        furnitureModeBtn.clicked += () => furnitureModeToggle();
        exportYesBtn.clicked += () => exportYesPress();
        exportNoBtn.clicked += () => exportNoPress();
        clearYesBtn.clicked += () => clearYesPress();
        clearNoBtn.clicked += () => clearNoPress();
    }
    #endregion

    #region Button Callbacks
    public void importPress()
    {
        Debug.Log("Importing House...");
        showCancelButton(true);
        clearPopup.style.display = DisplayStyle.Flex;
    }

    public void exportPress()
    {
        Debug.Log("Exporting House...");
        showCancelButton(false);
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

    public void cancelPress()
    {

        Debug.Log("Cancelling action...");
    }

    public void wallModeToggle()
    {
        state = CurrentState.WallPlacement;
        Debug.Log("Wall mode active");
    }

    public void doorModeToggle()
    {
        state = CurrentState.DoorPlacement;
        Debug.Log("Door mode active");
    }

    public void furnitureModeToggle()
    {
        state = CurrentState.FurniturePlacement;
        Instantiate(GenericFurniture, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void chairModeActivate()
    {

        Debug.Log("Chair Mode Activated");
    }

    public void chestModeActivate()
    {
        Debug.Log("Chest Mode Activated");
    }

    public void tableModeActivate()
    {
        Debug.Log("Table Mode Activated");
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
            cancelBar.style.display = DisplayStyle.None;
        }
        else
        {
            cancelBar.style.display = DisplayStyle.Flex;
        }
    }
    #endregion
}
