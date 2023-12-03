// This script, LayoutManager, is responsible for managing various functionalities related to layout editing and JSON data import/export in a Unity application.
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

// This script, LayoutManager, is responsible for managing various functionalities related to layout editing and JSON data import/export in a Unity application.
public class LayoutManager : MonoBehaviour
{
    #region Prefabs and Prefab Lists:
    // Get GameObject Prefabs:
    [SerializeField] GameObject Chair;
    [SerializeField] GameObject Table;
    [SerializeField] GameObject Chest;
    [SerializeField] GameObject Door;

    [SerializeField] GameObject Wall;

    // GameObject List Categories:
    List<GameObject> Walls = new List<GameObject>();
    List<GameObject> RoomDoors = new List<GameObject>();
    List<GameObject> ExitDoors = new List<GameObject>();
    List<GameObject> Furniture = new List<GameObject>();
    #endregion

    private void OnEnable()
    {
        // Import given file if there is one:
        importJSON();
        // Get UIDocument Root:
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
    }

    #region List Add Functions
    public void addWall()
    {
        // TODO: Add a wall to the Walls list.
    }
    public void addRoomDoor()
    {
        // TODO: Add a room door to the RoomDoors list.
    }
    public void addExitDoor()
    {
        // TODO: Add an exit door to the ExitDoors list.
    }
    public void addFurniture(string type, Quaternion rotation, float xPos = 0, float yPos = 0, bool imported = false)
    {
        // TODO: Add a furniture to the Furniture list.
        GameObject newFurniture;
        if (type == "chair") newFurniture = Instantiate(Chair, new Vector3(xPos, yPos, 0), rotation);
        else if (type == "table") newFurniture = Instantiate(Table, new Vector3(xPos, yPos, 0), rotation);
        else if (type == "chest") newFurniture = Instantiate(Chest, new Vector3(xPos, yPos, 0), rotation);
        else if (type == "door") newFurniture = Instantiate(Door, new Vector3(xPos, yPos, 0), rotation);
        else return; // Invalid input
        newFurniture.GetComponent<ClickDrop>().onDeleteClicked += deleteFurniture;
        if (!imported) newFurniture.GetComponent<ClickDrop>().isDragging = true;
        Furniture.Add(newFurniture);
    }
    #endregion

    #region List Delete/Clear Functions
    public void clearAll()
    {
        // TODO: Clear all objects in the lists and destroy them.
        foreach(GameObject obj in Furniture) DestroyImmediate(obj, true);
        foreach(GameObject obj in Walls) DestroyImmediate(obj, true);
        foreach(GameObject obj in RoomDoors) DestroyImmediate(obj, true);
        foreach(GameObject obj in ExitDoors) DestroyImmediate(obj, true);
        Furniture.Clear();
        InterSceneManager.wallList.Clear();
        RoomDoors.Clear();
        ExitDoors.Clear();
    }
    // Subscribe these functions to the GameObjects delete event:
    private void deleteFurniture(GameObject obj)
    {
        // TODO: Remove the object from the Furniture list and destroy it.
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
    public List<GameObject> getWalls()
    {
        return Walls;
    }
    public List<GameObject> getRoomDoors()
    {
        return RoomDoors;
    }
    public List<GameObject> getExitDoors()
    {
        return ExitDoors;
    }
    public List<GameObject> getFurniture()
    {
        return Furniture;
    }
    #endregion

    #region JSON Management

    #region Serializable Classes
    [System.Serializable] public class SerializableList<T>
    {
        // TODO: Create a class for each list type and add the necessary variables.
        public Vector3 vacuumPosition = Vector3.zero;
        public List<T> Furniture = new List<T>();
        public List<T> Walls = new List<T>();
        public List<T> RoomDoors = new List<T>();
        public List<T> ExitDoors = new List<T>();
        public List<SimulationEntry> SIMULATION_DATA = new List<SimulationEntry>();
    }

    [System.Serializable] public class Object
    {
        // TODO: Create a class for each object type and add the necessary variables.
        public string type = "";
        public float posX = 0;
        public float posY = 0;
        public Quaternion rotation = Quaternion.identity;

        public Object(string type, float posX, float posY, Quaternion rot)
        {
            this.type = type;
            this.posX = posX;
            this.posY = posY;
            this.rotation = rot;
        }
    }
    #endregion

    private void importJSON()
    {
        // TODO: Import JSON data and instantiate objects based on the data.
        SerializableList<Object> parsedJSON = new SerializableList<Object>();
        try
        {
            string unparsedJSON = "";
            if (InterSceneManager.userWantsDefaultHouse)
            {
                unparsedJSON = System.IO.File.ReadAllText(Application.dataPath + "/StreamingAssets/Default_Layout.json");
            }
            else
            {
                unparsedJSON = System.IO.File.ReadAllText(Application.dataPath + "/StreamingAssets/" + InterSceneManager.fileSelection + ".json");
            }
            parsedJSON = JsonUtility.FromJson<SerializableList<Object>>(unparsedJSON);
        }
        catch (Exception e)
        {
            Debug.Log("JSON Import Exception: " + e.Message);
            // TODO: Process and instantiate objects based on parsedJSON data.
        }
        for (int i = 0; i < parsedJSON.Furniture.Count; i++)
        {
            if (parsedJSON.Furniture[i].type == "Chair(Clone)") {
                addFurniture("chair", parsedJSON.Furniture[i].rotation, parsedJSON.Furniture[i].posX, parsedJSON.Furniture[i].posY, true);
            }
            else if (parsedJSON.Furniture[i].type == "Chest Variant(Clone)")
            {
                addFurniture("chest", parsedJSON.Furniture[i].rotation, parsedJSON.Furniture[i].posX, parsedJSON.Furniture[i].posY, true);
                InterSceneManager.coveredTileNum += 6;
            }
            else if (parsedJSON.Furniture[i].type == "Table Variant(Clone)")
            {
                addFurniture("table", parsedJSON.Furniture[i].rotation, parsedJSON.Furniture[i].posX, parsedJSON.Furniture[i].posY, true);
            }
            else if (parsedJSON.Furniture[i].type == "Door(Clone)")
            {
                addFurniture("door", parsedJSON.Furniture[i].rotation, parsedJSON.Furniture[i].posX, parsedJSON.Furniture[i].posY, true);
            }
        }
        for (int i =0; i < parsedJSON.Walls.Count; i++)
        {
            GameObject newWall = Instantiate(Wall, new Vector3(parsedJSON.Walls[i].posX, parsedJSON.Walls[i].posY, 0), parsedJSON.Walls[i].rotation);
            newWall.GetComponent<WallPlacer>().disableSpawners(); // Disable wall extending endpoints
        }

        var currentScene = SceneManager.GetActiveScene();
        var currentSceneName = currentScene.name;

        if (currentSceneName != "ShowColorCodedResults")
        {
            GameObject.Find("Vacuum-Robot").transform.position = parsedJSON.vacuumPosition;
        }

        if (currentSceneName == "Simulation")
        {
            // Disable all BoxColliders in the Chair and Table Objects:
            GameObject[] objectsThatShouldntHaveColliders = GameObject.FindGameObjectsWithTag("NoColliderBuddy");
            foreach (GameObject obj in objectsThatShouldntHaveColliders)
            {
                obj.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }
    public void saveToJSON(string JSONFilePath)
    {
        // TODO: Save the layout data to a JSON file.
        GameObject[] walls = GameObject.FindGameObjectsWithTag("WallBuddy");
        SerializableList<Object> FullList = new SerializableList<Object>();
        FullList.vacuumPosition = GameObject.Find("Vacuum-Robot").transform.position;
        foreach (GameObject f in Furniture) FullList.Furniture.Add(new Object(f.name, f.transform.position.x, f.transform.position.y, f.transform.rotation));
        foreach (GameObject w in walls) FullList.Walls.Add(new Object(w.name, w.transform.position.x, w.transform.position.y, w.transform.rotation));
        foreach (GameObject rd in RoomDoors) FullList.RoomDoors.Add(new Object(rd.name, rd.transform.position.x, rd.transform.position.y, rd.transform.rotation));
        foreach (GameObject ed in ExitDoors) FullList.ExitDoors.Add(new Object(ed.name, ed.transform.position.x, ed.transform.position.y, ed.transform.rotation));
        try
        {
            string FullJSON = JsonUtility.ToJson(FullList);
            System.IO.File.WriteAllText(Application.dataPath + "/StreamingAssets/" + JSONFilePath+".json", FullJSON);
        }
        catch (Exception e)
        {
            Debug.Log("JSON Saving Exception: " + e.Message);
        }
    }
    #endregion
}