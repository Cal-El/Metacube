using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class DataManager : MonoBehaviour {

	public static DataManager DM;
    public static Dictionary<string, int> saveData;

    private static StreamWriter sw;
    private static StreamReader sr;
    private static string directoryPath;

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

            saveData = new Dictionary<string, int>();
            ReadSaveData();

            if (!PlayerPrefs.HasKey("Prefs Set") || PlayerPrefs.GetString("Prefs Set") == "False") {
                SetDefaultPrefs();
            }
        }
	}

    public void WriteSaveData() {
        sw = new StreamWriter(directoryPath + "\\SaveData.txt");
        foreach(string k in saveData.Keys) {
            sw.WriteLine(k + ":" + saveData[k]);
        }
        sw.Close();
    }

    public void WriteInitialSaveData() {
        sw = new StreamWriter(directoryPath + "\\SaveData.txt");
        sw.WriteLine("Level 1-1 Checkpoint:0");
        sw.WriteLine("Level 1-2 Checkpoint:0");
        sw.WriteLine("Level 1-3 Checkpoint:0");
        sw.WriteLine("Level 1-4 Checkpoint:0");
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
        if(!File.Exists(directoryPath + "\\SaveData.txt")) {
            WriteInitialSaveData();
        }
        sr = new StreamReader(directoryPath + "\\SaveData.txt");
        while (!sr.EndOfStream) {
            string[] parts = sr.ReadLine().Split(':');
            saveData.Add(parts[0], int.Parse(parts[1]));
        }
        sr.Close();
    }

    void SetDefaultPrefs() {
        PlayerPrefs.SetString("Prefs Set", "True");
        PlayerPrefs.SetFloat("FoV", 60.0f);
        PlayerPrefs.SetString("FoVShift On", "True");
        PlayerPrefs.SetFloat("Sensitivity", 5.0f);
        QualitySettings.SetQualityLevel(QualitySettings.names.Length / 2);

        PlayerPrefs.Save();
    }

    public static bool GetBool(string name) {
        if (saveData.ContainsKey(name)) {
            int temp = saveData[name];
            if (temp == 1)
                return true;
            else
                return false;
        } else {
            SetBool(name, false);
            return false;
        }
    }

    public static void SetBool (string name, bool data) {
        if (saveData.ContainsKey(name)) {
            if (data) saveData[name] = 1;
            else saveData[name] = 0;
        } else {
            if (data) saveData.Add(name, 1);
            else saveData.Add(name, 0);
        }
        DM.WriteSaveData();
    }

    public static int GetInt(string name) {
        if (saveData.ContainsKey(name)) {
            return saveData[name];
        } else {
            SetInt(name, 0);
            return 0;
        }
    }

    public static void SetInt(string name, int data) {
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
}
