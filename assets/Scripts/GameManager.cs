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
	public int progression;
	public int checkpointNum;
	public Transform player;
	public float deadspacePoint = 100;

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
		checkpoints[0].playerPos = player.position;
        checkpoints[0].playerRot = player.rotation.eulerAngles;
		checkpoints[0].level = transform.position;
		checkpoints[0].levelRot = transform.rotation;
		checkpoints[0].progression = progression;
		checkpointNum = DataManager.GetInt("Level " + LevelID + " Checkpoint");

		GetCheckpoint();
	}

	// Update is called once per frame
	void Update ()
	{
        if (Input.GetKeyDown(KeyCode.R) && !finishedLevel && FindObjectOfType<DollyCam>() == null)
            GetCheckpoint();
        if (Input.GetKeyDown(KeyCode.M)) {
            Debug.Log((player.position - transform.position) + "----PlayerPos\n"
                + (player.rotation) + "----PlayerRot\n"
                + Vector3.zero + "----LevelPos\n"
                + transform.rotation + "----LevelRot\n"
                + progression + "----LevelProgression\n");
        }
        if (Input.GetKeyDown(KeyCode.Escape) && optionsMenu != null)
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
            } else if (fallTimer >= MAXFALLTIME){
                GetCheckpoint();
            }
		}else{
			if(endTimer <= 0)
				Camera.main.transform.GetChild(0).GetComponent<faceWhite>().FadeWhite(5);
			endTimer += Time.fixedDeltaTime;
            if (endTimer >= 6) {
                DataManager.SetInt("Level " + LevelID + " Checkpoint", 0);
                DataManager.SetBool("Level " + LevelID + " Finished", true);
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
		transform.rotation = checkpoints[checkpointNum].levelRot;
		transform.position = checkpoints[checkpointNum].level;
		player.position = checkpoints[checkpointNum].playerPos;
        player.GetComponent<ModdedMouseLook>().xRot = (checkpoints[checkpointNum].playerRot).y;
		progression = checkpoints[checkpointNum].progression;
		transform.GetComponent<changeColour> ().colour = checkpoints[checkpointNum].colour;
        if(transform.GetComponent<WorldRotation>() != null)
            transform.GetComponent<WorldRotation> ().rotTimer = 0;
        else {

        }
		
		//This solves the fall through world bug
		player.GetComponent<CharacterMotorC> ().movement.velocity = Vector3.zero;

        totalDeaths++;
        Analytics.CustomEvent("Deaths", new Dictionary<string, object>
        {
                    { "Level", SceneManager.GetActiveScene().name },
                    { "Progress Value", progression },
                    { "Total Deaths", totalDeaths }
                });
        Camera.main.transform.GetChild(0).GetComponent<faceWhite>().FadeFromWhite(2);
        fallTimer = 0;
    }
}
