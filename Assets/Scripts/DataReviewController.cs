using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static LayoutManager;

public class DataReviewController : MonoBehaviour
{
    TreeView data;
    Button returnToMainMenu;
    Button prevBtn;
    Button nextBtn;

    // JSON Data for Currently Viewed Run:
    int runNumber;
    bool whiskers;
    string floorCovering;
    float batteryLifeStart;
    //List<RunData> algorithmRuns = new List<RunData>();

    void OnEnable()
    {
        // Get UIDocument Root:
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Get top panel items:
        returnToMainMenu = root.Q<Button>("GoHomeButton");

        // Get bottom panel items:
        prevBtn = root.Q<Button>("PrevButton");
        nextBtn = root.Q<Button>("NextButton");

        // Get Data Panel Items:
        data = root.Q<TreeView>("Data");

        subscribeToCallbacks();
    }

    // Subscribe to function callbacks:
    private void subscribeToCallbacks()
    {
        returnToMainMenu.clicked += () => { SceneManager.LoadScene(sceneName: "MainMenu"); };
        prevBtn.clicked += () => { fillDataPanel("idk", 1); };
        nextBtn.clicked += () => { clearDataPanel(); };
    }

    // Remove all elements from the data TreeView:
    private void clearDataPanel()
    {
        data.hierarchy.ElementAt(0).Clear();
    }

    // Load the data TreeView with a new run's info:
    private void fillDataPanel(string jsonFile, int runNum)
    {
        //for (int i = 0; i < 100; i++)
        //{
        //    AddLabelToData(i.ToString());
        //}
        readJSONFile("Layout1", 0);
    }

    [System.Serializable]
    public class SimulationData
    {
        public SimulationDataEntry[] SIMULATION_DATA;
    }

    [System.Serializable]
    public class SimulationDataEntry
    {
        public SimulationSettings Settings;
        public SimulationStats Random;
        public SimulationStats WallFollow;
        public SimulationStats Spiral;
        public SimulationStats Snaking;
    }

    [System.Serializable]
    public class SimulationSettings
    {
        public bool whiskers;
        public string floorCovering;
        public int batteryLifeStart;
    }

    [System.Serializable]
    public class SimulationStats
    {
        public int elapsedTime;
        public int batteryLifeEnd;
        public int cleaningEfficiency;
        public int tilesCleaned;
        public int untouchedTiles;
    }

    private void readJSONFile(string fileName, int runNum)
    {
        //SerializableList<RunData> parsedJSON = new SerializableList<RunData>();
        SimulationData parsedJSON = new SimulationData();
        try
        {
            string unparsedJSON = System.IO.File.ReadAllText(Application.dataPath + "/StreamingAssets/" + fileName + ".json");
            parsedJSON = JsonUtility.FromJson<SimulationData>(unparsedJSON);
            Debug.Log("A: " + parsedJSON);
        }
        catch (Exception e)
        {
            Debug.Log("JSON Read Exception: " + e.Message);
        }
        SimulationDataEntry data = parsedJSON.SIMULATION_DATA[0];
        Debug.Log("B: " + data.Random.elapsedTime.ToString());
        Debug.Log("Data: " + data.Settings.floorCovering);
    }

    // Update is called once per frame
    private void AddLabelToData(string text)
    {
        data.hierarchy.ElementAt(0).Add(new Label(text));
    }
}
