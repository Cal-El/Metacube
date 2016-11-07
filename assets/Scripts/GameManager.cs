using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

	public static GameManager GM;
    public string LevelID = "1-1";
    public GameObject optionsMenu;
    public GameObject EndLevelSound;
    public GameObject LoadCheckpointSound;
	public int progression;
	private int checkpointNum;
	public Transform player;
	public float deadspacePoint = 100;
    [HideInInspector]
    public float reloadTimer = 0;
    public const float TIMETORELOAD = 0.5f;

    private bool timerOn = false;
    [HideInInspector]
    public float fullLevelTimer = 0;

	public bool finishedLevel = false;
	private float endTimer = 0;
    private float fallTimer = 0;
    private const float MAXFALLTIME = 5;
    private int totalDeaths = 0;
    

	/*
	 * The Checkpoint class is a set of values 
	 * for use in loading checkpoints.
	 */
	[System.Serializable]
	public class CheckPoint
	{
		public Vector3 playerPos;
        public Vector3 playerRot;
		public Vector3 level;
		public Quaternion levelRot;
		public int progression;
		public Color colour;
	}
    [Header("Checkpoints")]
	public CheckPoint[] checkpoints = new CheckPoint[1];


	// Use this for initialization
	void Awake ()
	{
		GM = this;

        checkpoints[0].level = transform.position;
		checkpoints[0].levelRot = transform.rotation;
        checkpoints[0].playerPos = player.position;
        checkpoints[0].playerRot = player.rotation.eulerAngles;
        checkpoints[0].progression = progression;
		CheckpointNum = DataManager.GetInt("Level " + LevelID + " Checkpoint");
        
        if(DataManager.GetBool("Level " + LevelID + " Finished")) {
            timerOn = true;
        }
        if (DataManager.GetInt("Level " + LevelID + " Checkpoint") > 0) {
            fullLevelTimer = DataManager.GetFloat("Level " + LevelID + " Saved Timer");
        }

		GetCheckpoint(false);
	}

	// Update is called once per frame
	void Update ()
	{
        if (Input.GetKey(KeyCode.M)) {
            string s = "Checkpoint Data: ";
            s += "\nCheckpoint Info:";
            s += "\nPlayer Position: " + (player.position - transform.position);
            s += "\nPlayer Rotation: " + player.rotation.eulerAngles;
            s += "\nLevel Position: " + Vector3.zero;
            s += "\nLevel Rotation: " + transform.rotation;
            s += "\nLevel Progression: " + progression;
            Debug.Log(s);
        }

        if (player.GetComponent<CharacterMotorC>().canControl) {
            fullLevelTimer += Time.deltaTime;
            CurveRotator.CurveRotationManager.Update();
        }

        if (Input.GetButtonDown("Reload")) {
            reloadTimer += Time.deltaTime;
        } else if (reloadTimer > 0) {
            if (Input.GetButton("Reload") && !finishedLevel)
                reloadTimer += Time.deltaTime;
            else {
                reloadTimer = 0;
            }
            if(reloadTimer >= TIMETORELOAD) {
                DollyCam dc = FindObjectOfType<DollyCam>();
                if (dc != null) {
                    dc.EndCutscene();
                } else {
                    GetCheckpoint();
                    PlaytestData.LogDeath(CheckpointNum, false);
                }
            }
        }
        
        if (Input.GetButtonDown("Cancel") && optionsMenu != null)
            if(FindObjectOfType<PausedMenu>() == null)
                Instantiate(optionsMenu);
	}

	void FixedUpdate ()
	{
        if (!finishedLevel){
            if (!player.GetComponent<CharacterMotorC>().grounded) {
                fallTimer += Time.fixedDeltaTime;
            } else {
                fallTimer = 0;
            }
			if (player.position.y < transform.position.y - deadspacePoint - 50){
				GetCheckpoint ();
                PlaytestData.LogDeath(checkpointNum);
            } else if (fallTimer >= MAXFALLTIME){
                GetCheckpoint();
                PlaytestData.LogDeath(checkpointNum);
            }
		}else{
            player.GetComponent<CharacterMotorC>().canControl = false;
            if (endTimer <= 0) {
                Spawn2DSound(EndLevelSound);
				faceWhite.FadeToWhite(4);
            }
            player.GetComponent<CharacterMotorC>().movement.velocity = EndLevelScript.endTrans.position - player.position;
			endTimer += Time.fixedDeltaTime;
            if (endTimer >= 5) {
                DataManager.SetInt("Level " + LevelID + " Checkpoint", 0);
                DataManager.SetBool("Level " + LevelID + " Finished", true);
                DataManager.SetFloat("Level " + LevelID + " Saved Timer", 0);
                if(DataManager.GetFloat("Level " + LevelID + " Finished Timer") == 0 || fullLevelTimer < DataManager.GetFloat("Level " + LevelID + " Finished Timer")) {
                    DataManager.SetFloat("Level " + LevelID + " Finished Timer", fullLevelTimer);
                }
                PlaytestData.LogApplicationEvent("Level Finished");
                SceneManager.LoadScene("Museum");
            }
		}
	}

	//set a checkpoint
	public void SetCheckpoint (Vector3 _player)
	{
		checkpoints[checkpointNum].playerPos = player.position;
        checkpoints[checkpointNum].playerRot = player.rotation.eulerAngles;
		checkpoints[checkpointNum].level = transform.position;
		checkpoints[checkpointNum].levelRot = transform.rotation;
		checkpoints[checkpointNum].progression = progression;
		checkpoints[checkpointNum].colour = transform.GetComponent<changeColour> ().colour;
	}
	
	//Reset to a checkpoint
	public void GetCheckpoint ()
	{
        GetCheckpoint(true);
    }

    public void GetCheckpoint(bool playSound) {
        if (playSound) {
            Spawn2DSound(LoadCheckpointSound);
            faceWhite.FadeFromWhite(0.5f);
        }
        transform.rotation = checkpoints[checkpointNum].levelRot;
        transform.position = checkpoints[checkpointNum].level;
        player.position = checkpoints[checkpointNum].playerPos;
        player.GetComponent<ModdedMouseLook>().xRot = (checkpoints[checkpointNum].playerRot).y;
        Camera.main.GetComponent<ModdedMouseLook>().rotationY = 0;
        player.GetComponent<CharacterMotorC>().grounded = false;
        progression = checkpoints[checkpointNum].progression;
        transform.GetComponent<changeColour>().colour = checkpoints[checkpointNum].colour;
        reloadTimer = 0;
        if (transform.GetComponent<WorldRotation>() != null)
            transform.GetComponent<WorldRotation>().ClearList();
        else {

        }

        //This solves the fall through world bug
        player.GetComponent<CharacterMotorC>().movement.velocity = Vector3.zero;

        fallTimer = 0;
    }

    public static void Spawn2DSound(GameObject g) {
        if(g != null) {
            GameObject go = Instantiate(g) as GameObject;
            go.transform.parent = DataManager.DM.transform;
        }
    }

    public int CheckpointNum {
        get {
            return checkpointNum;
        }
        set {
            checkpointNum = value;
            PlaytestData.LogCheckpoint(value);
        }
    }
}
