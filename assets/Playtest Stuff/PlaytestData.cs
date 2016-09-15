using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class PlaytestData : MonoBehaviour {

    public static PlaytestData PTData;
    protected static float startTime;
    protected static float timeLastSession = 0;
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
            timeSinceOpen = Time.time - startTime + timeLastSession;
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
        
        public DeathEvent (string _name, string _levelName, int _checkpointNum, int _totalDeathsInCheckpoint, int _totalDeathsInLevel, string _timeStamp, float _time) : base(_name) {
            levelName = _levelName;
            currentCheckpointNum = _checkpointNum;
            totalDeaths = _totalDeathsInLevel;
            totalDeathsThisCheckpoint = _totalDeathsInCheckpoint;
            timeStamp = _timeStamp;
        	timeSinceLoad = _time;
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
        
        public ApplicationEvent(string _name, string _timeStamp, float _time) : base(_name){
        	timeStamp = _timeStamp;
        	timeSinceLoad = _time;
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
        
        public CheckpointEvent(string _name, string _levelName, int _checkpointNum, string _timeStamp, float _time) : base(_name){
        	levelName = _levelName;
            	checkpointNum = _checkpointNum;
        	timeStamp = _timeStamp;
        	timeSinceLoad = _time;
        }

        public override string ToString() {
            return base.ToString() + "\t" + levelName + "\t" + checkpointNum;
        }
    }

    public class CheatingEvent : EventData {

        public CheatingEvent(string _name) : base(_name) {
            
        }
        
        public CheatingEvent(string _name, string _timeStamp, float _time) : base(_name){
        	timeStamp = _timeStamp;
        	timeSinceLoad = _time;
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
            LoadData();
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
                //PTData.checkpointEvents.Add(new CheckpointEvent("No", SceneManager.GetActiveScene().name, PTData.currentCheckpoint));
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

    public static string UserID {
        get {
            return PTData.userID;
        }
    }

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
    
    public static void LoadData() {
        if (PTData != null) {
            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\My Games\\Metacube\\TestData";
            if (!Directory.Exists(directoryPath)) {
                Directory.CreateDirectory(directoryPath);
            }
            if(!File.Exists(directoryPath + "\\User" + PTData.userID + ".txt")){
            	return;	
            }
            
            StreamReader sr = new StreamReader(directoryPath + "\\User" + PTData.userID + ".txt");
            string line = sr.ReadLine();
            float totalTimeLastSession = 0;
            
            if(sr.ReadLine() == "Application Events"){
            	sr.ReadLine(); //Timestamp Time Event
            	line = sr.ReadLine();
            	while(line.Length > 0){
            		string[] parts = line.Split("\t")
            		PTData.events.Add(new ApplicationEvent(parts[2], parts[0], float.Parse(parts[1])));
            		totalTimeLastSession = float.Parse(parts[1]);
            		line = sr.ReadLine();
            	}
            }
            
            if(sr.ReadLine() == "Checkpoint Events"){
            	sr.ReadLine(); //Timestamp Time Progressed Level Name Checkpoint
            	line = sr.ReadLine();
            	while(line.Length > 0){
            		string[] parts = line.Split("\t");
            		PTData.events.Add(new CheckpointEvent(parts[2], parts[3], int.Parse(parts[4]), parts[0], float.Parse(parts[1])));
            		line = ReadLine();
            	}
            }
            
            if(sr.ReadLine() == "Death Events"){
            	sr.ReadLine(); //Timestamp	Time	Event	LevelName	Checkpoint	Deaths this Checkpoint
            	line = sr.ReadLine();
            	while(line.Length > 0){
            		string[] parts = line.Split("\t");
            		PTData.events.Add(new DeathEvent(parts[2], parts[3], int.Parse(parts[4]), int.Parse(parts[5]), int.Parse(parts[6]), parts[0], float.Parse(parts[1])));
            		line = ReadLine();
            	}
            }
            
            if(sr.ReadLine() == "Cheat Events"){
            	sr.ReadLine(); //Timestamp	Time	Cheat Used
            	line = sr.ReadLine();
            	while(!sr.EndOfStream && line.Length > 0){
            		string[] parts = line.Split("\t");
            		PTData.events.Add(new CheatingEvent(parts[2], parts[0], float.Parse(parts[1])));
            		line = ReadLine();
            	}
            }

	
	    timeLastSession = totalTimeLastSession;
            sr.Close();
        }
    }
}
