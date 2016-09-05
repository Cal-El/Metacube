using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class PlaytestData : MonoBehaviour {

    protected static PlaytestData PTData;
    protected static float startTime;
    protected string userID;
    protected int totalDeathsThisCheckpoint;
    protected int totalDeaths;
    protected int currentCheckpoint;
    protected string sceneName;

    protected List<ApplicationEvent> events;
    protected List<CheatingEvent> cheatEvents;
    protected List<CheckpointEvent> checkpointEvents;
    protected List<DeathEvent> deathEvents;
    
    #region EventData Classes
    public abstract class EventData {
        public string name;
        public string timeStamp;
        public float timeSinceOpen;

        public EventData(string _name) {
            name = _name;
            timeStamp = System.DateTime.Now.ToString("HH:mm:ss");
            timeSinceOpen = Time.time - startTime;
        }

        public override string ToString() {
            return (timeStamp + "\t" + timeSinceOpen.ToString("n2") + "\t" + name);
        }
    }

    public class DeathEvent : EventData {
        public string levelName;
        public int currentCheckpointNum;
        public int totalDeathsThisCheckpoint;
        public int totalDeaths;

        public DeathEvent (string _name, string _levelName, int _checkpointNum) : base(_name) {
            levelName = _levelName;
            currentCheckpointNum = _checkpointNum;
            totalDeaths = PTData.totalDeaths;
            totalDeathsThisCheckpoint = PTData.totalDeathsThisCheckpoint;
        }

        public override string ToString() {
            string s = base.ToString();
            s += ("\t" + levelName + "\t" + currentCheckpointNum + "\t" + totalDeathsThisCheckpoint + "\t" + totalDeaths);
            return s;
        }
    }

    public class ApplicationEvent : EventData {
        public ApplicationEvent(string _name) : base(_name) {

        }

        public override string ToString() {
            return base.ToString();
        }
    }

    public class CheckpointEvent : EventData {
        public string levelName;
        public int checkpointNum;

        public CheckpointEvent(string _name, string _levelName, int _checkpointNum) : base(_name) {
            levelName = _levelName;
            checkpointNum = _checkpointNum;
        }

        public override string ToString() {
            return base.ToString() + "\t" + levelName + "\t" + checkpointNum;
        }
    }

    public class CheatingEvent : EventData {

        public CheatingEvent(string _name) : base(_name) {
            
        }

        public override string ToString() {
            return base.ToString();
        }
    }
    #endregion

    #region Monobehaviour Methods
    void Awake () {
        if (PTData == null) {
            DontDestroyOnLoad(this.gameObject);
            PTData = this;
            startTime = Time.time;
            
            PTData.events = new List<ApplicationEvent>();
            PTData.deathEvents = new List<DeathEvent>();
            PTData.checkpointEvents = new List<CheckpointEvent>();
            PTData.cheatEvents = new List<CheatingEvent>();
        } else {
            Destroy(this.gameObject);
        }
	}

    void Update() {
        if(sceneName != SceneManager.GetActiveScene().name) {
            LogSceneLoad();
        }
    }

    void OnApplicationQuit() {
        LogApplicationEvent("Session Ended");
        SaveData();
    }
    #endregion

    #region Log Events

    public static void LogNewUser(string ID) {
        if (PTData != null) {
            startTime = Time.time;
            PTData.userID = ID;
            PTData.currentCheckpoint = -1;
            PTData.totalDeaths = 0;
            PTData.totalDeathsThisCheckpoint = 0;
            PTData.sceneName = SceneManager.GetActiveScene().name;

            PTData.events.Add(new ApplicationEvent("User" + ID + " Playtest Started"));
        }
    }

    public static void LogApplicationEvent(string eventName) {
        if (PTData != null) {
            PTData.events.Add(new ApplicationEvent(eventName));
        }
    }

    public static void LogCheat(string eventName) {
        if (PTData != null) {
            PTData.cheatEvents.Add(new CheatingEvent(eventName));
        }
    }

    public static void LogSceneLoad() {
        if (PTData != null) {
            PTData.sceneName = SceneManager.GetActiveScene().name;
            PTData.currentCheckpoint = -1;
            PTData.totalDeaths = 0;
            PTData.events.Add(new ApplicationEvent(PTData.sceneName));
        }
    }

    public static void LogCheckpoint(int checkpoint) {
        if (PTData != null) {
            if (PTData.currentCheckpoint != checkpoint) {
                PTData.totalDeathsThisCheckpoint = 0;
                PTData.currentCheckpoint = checkpoint;
                PTData.checkpointEvents.Add(new CheckpointEvent("Yes", SceneManager.GetActiveScene().name, PTData.currentCheckpoint));
            } else {
                PTData.checkpointEvents.Add(new CheckpointEvent("No", SceneManager.GetActiveScene().name, PTData.currentCheckpoint));
            }
        }
    }

    public static void LogDeath(int checkpointNum, bool ofNaturalCauses = true) {
        if (PTData != null) {
            PTData.totalDeaths++;
            PTData.totalDeathsThisCheckpoint++;
            if (ofNaturalCauses) {
                PTData.deathEvents.Add(new DeathEvent("Death", SceneManager.GetActiveScene().name, checkpointNum));
            } else {
                PTData.deathEvents.Add(new DeathEvent("Reset", SceneManager.GetActiveScene().name, checkpointNum));
            }
        }
    }
#endregion

    public static void SaveData() {
        if (PTData != null) {
            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\Metacube\\TestData";
            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }
            StreamWriter sw = new StreamWriter(directoryPath + "\\User" + PTData.userID + ".txt");
            sw.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd") + "\tUser\t" + PTData.userID);

            sw.WriteLine("Application Events");
            sw.WriteLine("Timestamp\tTime\tEvent");
            foreach (ApplicationEvent e in PTData.events) {
                sw.WriteLine(e.ToString());
            }

            sw.WriteLine();
            sw.WriteLine("Checkpoint Events");
            sw.WriteLine("Timestamp\tTime\tProgressed\tLevelName\tCheckpoint");
            foreach (CheckpointEvent e in PTData.checkpointEvents) {
                sw.WriteLine(e.ToString());
            }

            sw.WriteLine();
            sw.WriteLine("Death Events");
            sw.WriteLine("Timestamp\tTime\tEvent\tLevelName\tCheckpoint\tDeaths this Checkpoint\tDeaths this Level");
            foreach(DeathEvent e in PTData.deathEvents) {
                sw.WriteLine(e.ToString());
            }

            sw.WriteLine();
            sw.WriteLine("Cheat Events");
            sw.WriteLine("Timestamp\tTime\tCheat Used");
            foreach (CheatingEvent e in PTData.cheatEvents) {
                sw.WriteLine(e.ToString());
            }

            sw.Close();
        }
    }
}
