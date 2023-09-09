using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{

    Label status;

    VisualElement cancelBar;
    Button cancelBtn;

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
        Button roomModeBtn = selectionPanel.Q<Button>("RoomButton");
        Button doorModeBtn = selectionPanel.Q<Button>("DoorButton");
        Button furnitureModeBtn = selectionPanel.Q<Button>("FurnitureButton");

        // Assign button callback functions:
        importBtn.clicked += () => importPress();
        exportBtn.clicked += () => exportPress();
        cancelBtn.clicked += () => cancelPress();
        roomModeBtn.clicked += () => roomModeToggle();
        doorModeBtn.clicked += () => doorModeToggle();
        furnitureModeBtn.clicked += () => furnitureModeActivate();
    }
    #endregion

    #region Button Callbacks
    public void importPress()
    {
        Debug.Log("Importing House...");
        showCancelButton(true);
    }

    public void exportPress()
    {
        Debug.Log("Exporting House...");
        showCancelButton(false);
    }

    public void cancelPress()
    {
        Debug.Log("Cancelling action...");
    }

    public void roomModeToggle()
    {
        Debug.Log("Room mode active");
    }

    public void doorModeToggle()
    {
        Debug.Log("Door mode active");
        updateStatus(false);
    }

    public void furnitureModeActivate()
    {
        Debug.Log("Furniture mode active");
        updateStatus(true);
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
