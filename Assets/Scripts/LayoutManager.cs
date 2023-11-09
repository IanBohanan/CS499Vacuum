// Import necessary Unity modules.
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;

// Define a class named LayoutManager which inherits from MonoBehaviour.
public class LayoutManager : MonoBehaviour
{
    #region Prefabs and Prefab Lists:
    // Serialized fields allow you to link your GameObjects in the inspector.
    [SerializeField] GameObject Chair;
    [SerializeField] GameObject Table;
    [SerializeField] GameObject Chest;
    [SerializeField] GameObject Door;

    // Lists to manage different categories of GameObjects within the scene.
    List<GameObject> Walls = new List<GameObject>();
    List<GameObject> RoomDoors = new List<GameObject>();
    List<GameObject> ExitDoors = new List<GameObject>();
    List<GameObject> Furniture = new List<GameObject>();
    #endregion

    // Method called when the script is enabled.
    private void OnEnable()
    {
        // Attempt to import layout from a JSON file.
        importJSON();
        // Access the root VisualElement of the UIDocument component attached to the same GameObject.
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
    }

    #region List Add Functions
    // Methods to add elements to the respective lists.
    // Currently, these methods are placeholders and need to be implemented.
    public void addWall() { /* Add wall functionality here */ }
    public void addRoomDoor() { /* Add room door functionality here */ }
    public void addExitDoor() { /* Add exit door functionality here */ }
    
    // Method to add furniture to the scene and respective list.
    public void addFurniture(string type, float xPos = 0, float yPos = 0, bool imported = false)
    {
        GameObject newFurniture;
        // Instantiate the correct GameObject based on the type provided.
        if (type == "chair") newFurniture = Instantiate(Chair, new Vector3(xPos, yPos, 0), Quaternion.identity);
        else if (type == "table") newFurniture = Instantiate(Table, new Vector3(xPos, yPos, 0), Quaternion.identity);
        else if (type == "chest") newFurniture = Instantiate(Chest, new Vector3(xPos, yPos, 0), Quaternion.identity);
        else if (type == "door") newFurniture = Instantiate(Door, new Vector3(xPos, yPos, 0), Quaternion.identity);
        else return; // Exit if an invalid type is provided.
        // Subscribe to the onDeleteClicked event.
        newFurniture.GetComponent<ClickDrop>().onDeleteClicked += deleteFurniture;
        // If not imported, set the isDragging property to true.
        if (!imported) newFurniture.GetComponent<ClickDrop>().isDragging = true;
        // Add the new furniture to the list.
        Furniture.Add(newFurniture);
    }
    #endregion

    #region List Delete/Clear Functions
    // Method to clear all objects and their respective lists.
    public void clearAll()
    {
        // Destroy all GameObjects in each list and clear the lists.
        foreach(GameObject obj in Furniture) DestroyImmediate(obj, true);
        foreach(GameObject obj in Walls) DestroyImmediate(obj, true);
        foreach(GameObject obj in RoomDoors) DestroyImmediate(obj, true);
        foreach(GameObject obj in ExitDoors) DestroyImmediate(obj, true);
        // Clear the lists.
        Furniture.Clear();
        Walls.Clear();
        RoomDoors.Clear();
        ExitDoors.Clear();
    }
    
    // Methods to delete individual elements from their respective lists and the scene.
    private void deleteFurniture(GameObject obj)
    {
        Furniture.Remove(obj);
        DestroyImmediate(obj);
    }
    private void deleteWall(GameObject obj)
    {
        Walls.Remove(obj);
        DestroyImmediate(obj);
    }
    private void deleteRoomDoor(GameObject obj)
    {
        RoomDoors.Remove(obj);
        DestroyImmediate(obj);
    }
    private void deleteExitDoor(GameObject obj)
    {
        ExitDoors.Remove(obj);
        DestroyImmediate(obj);
    }
    #endregion

    #region List Return Functions
    // Methods to get the lists of objects.
    public List<GameObject> getWalls() { return Walls; }
    public List<GameObject> getRoomDoors() { return RoomDoors; }
    public List<GameObject> getExitDoors() { return ExitDoors; }
    public List<GameObject> getFurniture() { return Furniture; }
    #endregion

    #region JSON Management

    #region Serializable Classes
    // Serializable classes to enable JSON serialization of GameObject lists.
    [System.Serializable] public class SerializableList<T>
    {
        public List<T> Furniture = new List<T>();
        public List<T> Walls = new List<T>();
        public List<T> RoomDoors = new List<T>();
        public List<T> ExitDoors = new List<T>();
    }

    [System.Serializable] public class Object
    {
        public string type = "";
        public float posX = 0;
        public float posY = 0;

        public Object(string type, float posX, float posY)
        {
            this.type = type;
            this.posX = posX;
            this.posY = posY;
        }
    }
    #endregion

    // Method to import layout from a JSON file.
    private void importJSON()
    {
        SerializableList<Object> parsedJSON = new SerializableList<Object>();
        try
        {
            // Read the JSON file.
            string unparsedJSON = System.IO.File.ReadAllText(Application.dataPath + "/StreamingAssets/" + InterSceneManager.fileSelection + ".json");
            // Deserialize the JSON to the SerializableList<Object>.
            parsedJSON = JsonUtility.FromJson<SerializableList<Object>>(unparsedJSON);
        }
        catch (Exception e)
        {
            // Log any exceptions during import.
            Debug.Log("JSON Import Exception: " + e.Message);
        }
        // Iterate through the deserialized list and add furniture to the scene.
        // Add more cases as needed for different furniture types.
        for (int i = 0; i < parsedJSON.Furniture.Count; i++)
        {
            // Conditional checks for each type of furniture.
            // "Clone" suffix is a common convention when instantiating prefabs.
            if (parsedJSON.Furniture[i].type == "Chair(Clone)") {
                addFurniture("chair", parsedJSON.Furniture[i].posX, parsedJSON.Furniture[i].posY, true);
            }
            // Additional checks for other furniture types.
            // ...
        }
    }
    
    // Method to save the current layout to a JSON file.
    public void saveToJSON(string JSONFilePath)
    {
        SerializableList<Object> FullList = new SerializableList<Object>();
        // Populate the SerializableList with current objects and their positions.
        foreach (GameObject f in Furniture) FullList.Furniture.Add(new Object(f.name, f.transform.position.x, f.transform.position.y));
        // Repeat for other object lists.
        // ...
        
        try
        {
            // Serialize the full list to JSON and write to a file.
            string FullJSON = JsonUtility.ToJson(FullList, true);
            System.IO.File.WriteAllText(Application.dataPath + "/StreamingAssets/" + JSONFilePath+".json", FullJSON);
        }
        catch (Exception e)
        {
            // Log any exceptions during saving.
            Debug.Log("JSON Saving Exception: " + e.Message);
        }
    }
    #endregion
}
