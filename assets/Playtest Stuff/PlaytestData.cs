using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class PlaytestData : MonoBehaviour {

    public static PlaytestData PTData;
    public string userID;
    public int totalDeaths;
    public string sceneName;

    public class EventData {
        public string name;
        public string timeStamp;

        public EventData(string _name) {
            name = _name;
            timeStamp = System.DateTime.Now.ToString("yyyy:MM:dd-HH:mm:ss");
        }

        public virtual void AddToFile(StreamWriter _sw) {
            _sw.WriteLine(name + " - " + timeStamp);
        }
    }

    public class DeathEvent : EventData {
        public string levelName;
        public string currentCheckpointNum;
        public string totalDeaths;

        public DeathEvent (string _name, string _levelName, string _checkpointNum) : base(_name) {
            levelName = _levelName;
            currentCheckpointNum = _checkpointNum;
            totalDeaths = PTData.totalDeaths.ToString();
        }

        public override void AddToFile(StreamWriter _sw) {
            base.AddToFile(_sw);
            _sw.WriteLine("Level: " + levelName);
            _sw.WriteLine("CheckpointNum: " + currentCheckpointNum);
            _sw.WriteLine("Total Deaths: " + totalDeaths);
        }
    }

    public List<EventData> events;

	// Instance Methods
	void Awake () {
        if (PTData == null) {
            DontDestroyOnLoad(this.gameObject);
            PTData = this;
            PTData.events = new List<EventData>();
        } else {
            Destroy(this.gameObject);
        }
	}

    void Update() {
        if(sceneName != SceneManager.GetActiveScene().name) {
            sceneName = SceneManager.GetActiveScene().name;
            totalDeaths = 0;
            LogSceneLoad();
        }
    }

    void OnApplicationQuit() {
        AddEvent(new EventData("Closed Application"));
        SaveData();
    }

    //Static Methods
    public static void AddEvent(EventData e) {
        if (PTData != null) {
            PTData.events.Add(e);
        }
    }

    public static void LogSceneLoad() {
        if (PTData != null) {
            AddEvent(new EventData(PTData.sceneName + " Loaded"));
        }
    }

    public static void LogDeath(int checkpointNum) {
        if (PTData != null) {
            PTData.totalDeaths++;
            AddEvent(new DeathEvent("Player Died", SceneManager.GetActiveScene().name, checkpointNum.ToString()));
        }
    }

    public static void SaveData() {
        if (PTData != null) {
            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\Metacube\\TestData";
            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }
            StreamWriter sw = new StreamWriter(directoryPath + "\\User" + PTData.userID + ".txt");

            foreach (EventData e in PTData.events) {
                e.AddToFile(sw);
                sw.WriteLine();
            }

            sw.Close();
        }
    }

    public static void SetUserID (string ID) {
        if (PTData != null) {
            PTData.userID = ID;
            PTData.totalDeaths = 0;
            PTData.sceneName = SceneManager.GetActiveScene().name;
            PTData.events = new List<EventData>();

            AddEvent(new EventData("User" + ID + " Playtest Started"));
        }
    }
}
