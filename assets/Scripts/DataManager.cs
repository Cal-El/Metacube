using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class DataManager : MonoBehaviour {

	public static DataManager DM;
    public static Dictionary<string, float> saveData;

    private static StreamWriter sw;
    private static StreamReader sr;
    private static string directoryPath;
    private static string saveFileName;

    public static Vector3 playerPos;
    public static Quaternion playerRot;

    // Use this for initialization
    void Awake () {
        if (DM != null)
            Destroy(this.gameObject);
        else {
            DM = this;
            DontDestroyOnLoad(transform.gameObject);
            string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            directoryPath = docs + "\\My Games\\Metacube";
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            if (!Directory.Exists(directoryPath + "\\Screenshots"))
                Directory.CreateDirectory(directoryPath + "\\Screenshots");

            if(PlaytestData.PTData != null) {
                saveFileName = "\\SaveData" + PlaytestData.UserID + ".txt";
            } else {
                saveFileName = "\\SaveData.txt";
            }

            saveData = new Dictionary<string, float>();
            ReadSaveData();

            if (!PlayerPrefs.HasKey("Prefs Set") || PlayerPrefs.GetString("Prefs Set") == "False") {
                SetDefaultPrefs();
            }
        }
	}

    public void LateUpdate() {
        if (Input.GetKeyDown(KeyCode.K))
            CaptureScreenshot();
    }

    public void WriteSaveData() {
        sw = new StreamWriter(directoryPath + saveFileName);
        foreach(string k in saveData.Keys) {
            sw.WriteLine(k + ":" + saveData[k]);
        }
        sw.Close();
    }

    public void WriteInitialSaveData() {
        sw = new StreamWriter(directoryPath + saveFileName);
        sw.WriteLine("Level 1-1 Checkpoint:0");
        sw.WriteLine("Level 1-2 Checkpoint:0");
        sw.WriteLine("Level 1-3 Checkpoint:0");
        sw.WriteLine("Level 1-4 Checkpoint:0");
        sw.WriteLine("Level 1-1 Saved Timer:0");
        sw.WriteLine("Level 1-2 Saved Timer:0");
        sw.WriteLine("Level 1-3 Saved Timer:0");
        sw.WriteLine("Level 1-4 Saved Timer:0");
        sw.WriteLine("Level 1-1 Finished Timer:0");
        sw.WriteLine("Level 1-2 Finished Timer:0");
        sw.WriteLine("Level 1-3 Finished Timer:0");
        sw.WriteLine("Level 1-4 Finished Timer:0");
        sw.WriteLine("Level 1-1 Finished:0");
        sw.WriteLine("Level 1-2 Finished:0");
        sw.WriteLine("Level 1-3 Finished:0");
        sw.WriteLine("Level 1-4 Finished:0");
        sw.WriteLine("Art 1-1-1:0");
        sw.WriteLine("Art 1-1-2:0");
        sw.WriteLine("Art 1-1-3:0");
        sw.WriteLine("Art 1-1-4:0");
        sw.WriteLine("Art 1-2-1:0");
        sw.WriteLine("Art 1-2-2:0");
        sw.WriteLine("Art 1-2-3:0");
        sw.WriteLine("Art 1-2-4:0");
        sw.WriteLine("Art 1-3-1:0");
        sw.WriteLine("Art 1-3-2:0");
        sw.WriteLine("Art 1-3-3:0");
        sw.WriteLine("Art 1-3-4:0");
        sw.WriteLine("Art 1-4-1:0");
        sw.WriteLine("Art 1-4-2:0");
        sw.WriteLine("Art 1-4-3:0");
        sw.WriteLine("Art 1-4-4:0");
        sw.Close();
    }

    public void ReadSaveData() {
        saveData.Clear();
        if (!File.Exists(directoryPath + saveFileName)) {
            WriteInitialSaveData();
        }
        sr = new StreamReader(directoryPath + saveFileName);
        while (!sr.EndOfStream) {
            string[] parts = sr.ReadLine().Split(':');
            saveData.Add(parts[0], float.Parse(parts[1]));
        }
        sr.Close();
    }

    public static void SetDefaultPrefs() {
        PlayerPrefs.SetString("Prefs Set", "True");
        PlayerPrefs.SetFloat("FoV", 80.0f);
        PlayerPrefs.SetString("FoVShift On", "True");
        PlayerPrefs.SetFloat("Sensitivity", 4.0f);
        QualitySettings.SetQualityLevel(QualitySettings.names.Length-1);

        PlayerPrefs.Save();
    }

    public static bool GetBool(string name) {
        if (saveData.ContainsKey(name)) {
            int temp = (int)saveData[name];
            if (temp == 1)
                return true;
            else
                return false;
        } else {
            return false;
        }
    }

    public static int GetInt(string name) {
        if (saveData.ContainsKey(name)) {
            return (int)saveData[name];
        } else {
            return 0;
        }
    }

    public static float GetFloat(string name) {
        if (saveData.ContainsKey(name)) {
            return saveData[name];
        } else {
            return 0;
        }
    }

    public static void SetBool(string name, bool data) {
        if (saveData.ContainsKey(name)) {
            if (data) saveData[name] = 1;
            else saveData[name] = 0;
        } else {
            if (data) saveData.Add(name, 1);
            else saveData.Add(name, 0);
        }
        DM.WriteSaveData();
    }

    public static void SetInt(string name, int data) {
        if (saveData.ContainsKey(name)) {
            saveData[name] = (float)data;
        } else {
            saveData.Add(name, (float)data);
        }
        DM.WriteSaveData();
    }

    public static void SetFloat(string name, float data) {
        if (saveData.ContainsKey(name)) {
            saveData[name] = data;
        } else {
            saveData.Add(name, data);
        }
        DM.WriteSaveData();
    }

    public static int GetProgress() {
        int i = 0;
        int level = 1;
        int chapter = 1;
        while(GetBool("Level " + chapter + "-" + level + " Finished")) {
            i++;
            level++;
            if(level > 4) {
                level = 1;
                chapter++;
            }
        }
        return i;
    }

    public static void CaptureScreenshot() {
        string[] currentFileNames = Directory.GetFiles(directoryPath + "\\Screenshots");
        int i = 0;
        if(currentFileNames != null) {
            i = currentFileNames.Length;
        }
        while (File.Exists(directoryPath + "\\Screenshots\\Screenshot " + i + ".png")) {
            i++;
        }
        Application.CaptureScreenshot(directoryPath + "\\Screenshots\\Screenshot "+ i +".png", 2);
    }

    public static void ResetEverything() {
        if (File.Exists(directoryPath + saveFileName)) {
            File.Delete(directoryPath + saveFileName);
        }
        if (DM != null) {
            DM.ReadSaveData();
        }
    }

    public static void UnlockEverything() {
        int level = 1;
        int chapter = 1;
        int art = 1;
        while (saveData.ContainsKey("Level " + chapter + "-" + level + " Finished")) { 
            if(saveData.ContainsKey("Art " + chapter + "-" + level + "-" + art))
                saveData["Art " + chapter + "-" + level + "-" + art] = 1;
            art++;
            if (art > 4) {
                saveData["Level " + chapter + "-" + level + " Finished"] = 1;
                art = 1;
                level++;
                if (level > 4) {
                    level = 1;
                    chapter++;
                }
            }
        }
    }
}
