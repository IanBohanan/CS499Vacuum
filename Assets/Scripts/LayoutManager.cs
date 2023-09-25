using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEditor.SearchService;

public class LayoutManager : MonoBehaviour
{
    // Get GameObject Prefabs:
    [SerializeField] GameObject Chair;
    [SerializeField] GameObject Table;
    [SerializeField] GameObject Chest;

    // GameObject List Categories:
    List<GameObject> Walls = new List<GameObject>();
    List<GameObject> RoomDoors = new List<GameObject>();
    List<GameObject> ExitDoors = new List<GameObject>();
    List<GameObject> Furniture = new List<GameObject>();

    #region List Add Functions
    public void addWall()
    {
        //Walls.Add();
    }
    public void addRoomDoor()
    {
        //RoomDoors.Add();
    }
    public void addExitDoor()
    {
        //ExitDoors.Add();
    }
    public void addFurniture(string type)
    {
        GameObject newFurniture;
        if (type == "chair") newFurniture = Instantiate(Chair, new Vector3(0, 0, 0), Quaternion.identity);
        else if (type == "table") newFurniture = Instantiate(Table, new Vector3(0, 0, 0), Quaternion.identity);
        else if (type == "chest") newFurniture = Instantiate(Chest, new Vector3(0, 0, 0), Quaternion.identity);
        else return; // Invalid input

        Furniture.Add(newFurniture);
    }
    #endregion

    #region List Delete/Clear Functions
        public void clearAll()
    {
        foreach(GameObject obj in Furniture) DestroyImmediate(obj, true);
        foreach(GameObject obj in Walls) DestroyImmediate(obj, true);
        foreach(GameObject obj in RoomDoors) DestroyImmediate(obj, true);
        foreach(GameObject obj in ExitDoors) DestroyImmediate(obj, true);
        Furniture.Clear();
        Walls.Clear();
        RoomDoors.Clear();
        ExitDoors.Clear();
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

    [System.Serializable] public class SerializableList<T>
    {
        public List<T> Furniture = new List<T>();
        public List<T> Walls = new List<T>();
        public List<T> RoomDoors = new List<T>();
        public List<T> ExitDoors = new List<T>();
    }

    [System.Serializable]
    public class Object
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
    public void saveToJSON(string JSONFilePath)
    {
        SerializableList<Object> FullList = new SerializableList<Object>();
        foreach (GameObject f in Furniture) FullList.Furniture.Add(new Object(f.name, f.transform.position.x, f.transform.position.y));
        foreach (GameObject w in Walls) FullList.Walls.Add(new Object(w.name, w.transform.position.x, w.transform.position.y));
        foreach (GameObject rd in RoomDoors) FullList.RoomDoors.Add(new Object(rd.name, rd.transform.position.x, rd.transform.position.y));
        foreach (GameObject ed in ExitDoors) FullList.ExitDoors.Add(new Object(ed.name, ed.transform.position.x, ed.transform.position.y));
        try
        {
            string FullJSON = JsonUtility.ToJson(FullList);
            System.IO.File.WriteAllText(Application.dataPath + "/StreamingAssets/"+JSONFilePath+".json", FullJSON);
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.Message);
        }
    }
    #endregion
}