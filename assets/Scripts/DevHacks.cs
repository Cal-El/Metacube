using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class DevHacks : MonoBehaviour {

    public static DevHacks HAXOR;

    [SerializeField]
    private string password = "i have the power";
    private string typedMessage = "";
    protected bool hacksEnabled;
    private GameObject child;
    public Text debugLog;

    // Use this for initialization
    void Start () {
        if (HAXOR == null) {
            HAXOR = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        child = transform.GetChild(0).gameObject;
        child.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (!hacksEnabled && Input.anyKeyDown) {
            string nextRequiredLetter = password[typedMessage.Length].ToString().ToLower();
            if (nextRequiredLetter == " ") {
                nextRequiredLetter = "space";
            } else if (nextRequiredLetter == "4") {
                nextRequiredLetter = "left";
            } else if (nextRequiredLetter == "8") {
                nextRequiredLetter = "up";
            } else if (nextRequiredLetter == "6") {
                nextRequiredLetter = "right";
            } else if (nextRequiredLetter == "2") {
                nextRequiredLetter = "down";
            }

            if (Input.GetKeyDown(nextRequiredLetter)) {
                typedMessage += password[typedMessage.Length].ToString();
                if (typedMessage == password) {
                    child.SetActive(true);
                    hacksEnabled = true;
                    PlaytestData.LogCheat("Hacks Enabled");
                }
            } else {
                typedMessage = "";
            }
        }
    }

    public static bool DevHacksEnabled {
        get {
            if (HAXOR != null)
                return HAXOR.hacksEnabled;
            else
                return false;
        }
    }

    public void ConsoleInput(string s) {
        string[] brokenUp = s.Split(' ');
        debugLog.text = "";

        switch (brokenUp[0]) {
            case "CheckpointInfo":
                GetCheckpointInfo();
                break;
            case "Reload":
                if (brokenUp.Length == 1) {
                    ReloadLevel();
                } else {
                    debugLog.text += "\nNot a valid input. Input '?' for a list of commands.";
                }
                break;
            case "LoadCheckpoint":
                if(brokenUp.Length == 2) {
                    try { SkipToCheckpoint(int.Parse(brokenUp[1])); }
                    catch { debugLog.text += "\nCheckpoint is NaN"; }
                } else {
                    debugLog.text += "\nNot a valid input. Input '?' for a list of commands.";
                }
                break;
            case "FinishLevel":
                if (brokenUp.Length == 1) {
                    FinishLevel();
                } else {
                    debugLog.text += "\nNot a valid input. Input '?' for a list of commands.";
                }
                break;
            case "DisableHacks":
                child.SetActive(false);
                hacksEnabled = false;
                typedMessage = "";
                PlaytestData.LogCheat("Disabled");
                break;
            case "UnlockEverything":
                UnlockEverything();
                break;
            case "?":
                Help();
                break;
            default:
                debugLog.text += "\nNot a valid input. Input '?' for a list of commands.";
                break;
        }
    }

    public void UnlockEverything() {
        DataManager.UnlockEverything();
        PlaytestData.LogCheat("Everything Unlocked");
        ReloadLevel();
    }

    public void SkipToCheckpoint(int index) {
        if(GameManager.GM != null) {
            if (index >= 0 && index < GameManager.GM.checkpoints.Length) {
                debugLog.text += "\nSkipping to checkpoint #" + index;
                PausedMenu pMenu = FindObjectOfType<PausedMenu>();
                if (pMenu != null) {
                    pMenu.ResumeGame();
                }
                GameManager.GM.CheckpointNum = index;
                GameManager.GM.GetCheckpoint();
                PlaytestData.LogCheat("Skipped to checkpoint " + index);
            } else {
                debugLog.text += "\nCheckpoint #" + " doesn't exist";
            }
        } else {
            debugLog.text += "\nGameManager not assigned.";
        }
    }

    public void ReloadLevel() {
        debugLog.text += "\nReloading Level...";
        PausedMenu pMenu = FindObjectOfType<PausedMenu>();
        if (pMenu != null) {
            pMenu.ResumeGame();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GetCheckpointInfo() {
        GameManager gm = GameManager.GM;
        if (gm != null) {
            debugLog.text += "\nCheckpoint Info:";
            debugLog.text += "\nPlayer Position: " + (gm.player.position - gm.transform.position);
            debugLog.text += "\nPlayer Rotation: " + gm.player.rotation.eulerAngles;
            debugLog.text += "\nLevel Position: " + Vector3.zero;
            debugLog.text += "\nLevel Rotation: " + gm.transform.rotation;
            debugLog.text += "\nLevel Progression: " + gm.progression;
        } else {
            debugLog.text += "\nGameManager not assigned.";
        }
    }

    public void FinishLevel() {
        if (GameManager.GM != null) {
            PausedMenu pMenu = FindObjectOfType<PausedMenu>();
            if (pMenu != null) {
                pMenu.ResumeGame();
            }
            GameManager.GM.finishedLevel = true;
            PlaytestData.LogCheat("Level Finished");
        } else {
            debugLog.text += "\nGameManager not assigned.";
        }
    }

    public void Help() {
        debugLog.text += "\nLoadCheckpoint - Loads a checkpoint";
        debugLog.text += "\nUnlockEverything - Unlocks all levels and art";
        debugLog.text += "\nCheckpointInfo - Displays current game information required to make a checkpoint";
        debugLog.text += "\nReload - Reload the level using the DataManager saveData";
        debugLog.text += "\nFinishLevel - Finishs the current level instantly";

        debugLog.text += "\nDisableHacks - Disables DevHack Mode";
        debugLog.text += "\n? - Lists implemented commands";
    }
}
