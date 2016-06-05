using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

	public static GameManager GM;
    public GameObject optionsMenu;
	public int progression;
	public int checkpointNum;
	public Transform player;
	public float deadspacePoint = 100;

	public bool finishedLevel = false;
	private float endTimer = 0;
    private float fallTimer = 0;
    private const float MAXFALLTIME = 5;

	/*
	 * The Checkpoint class is a set of values 
	 * for use in loading checkpoints.
	 */
	[System.Serializable]
	public class CheckPoint
	{
		public Vector3 playerPos;
		public Vector3 level;
		public Quaternion levelRot;
		public int progression;
		public Color colour;
	}
	public CheckPoint[] checkpoints = new CheckPoint[1];


	// Use this for initialization
	void Awake ()
	{
		GM = this;
		checkpoints[0].playerPos = player.position;
		checkpoints[0].level = transform.position;
		checkpoints[0].levelRot = transform.rotation;
		checkpoints[0].progression = progression;
		if(PlayerPrefs.HasKey("CHECKPOINTNUM DELETE THIS BIT"))
			checkpointNum = PlayerPrefs.GetInt("CHECKPOINTNUM");
		GetCheckpoint();
		if (!PlayerPrefs.HasKey ("Prefs Set"))
				SetDefaultPrefs ();
		Camera.main.fieldOfView = PlayerPrefs.GetFloat ("FoV");
	}

	void Start ()
	{
		
	}

	// Update is called once per frame
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.M))
			Debug.Log(transform.rotation + "----Rotation\n" + (player.position-transform.position) + "----Player");
        if (Input.GetKeyDown(KeyCode.Escape) && optionsMenu != null)
            if(FindObjectOfType<OptionsMenu>() == null)
                Instantiate(optionsMenu);
        if (Input.GetKeyDown(KeyCode.H))
            Debug.Log("Progression: " + progression + "\n" +
                "Player Pos: " + player.position + "\n" +
                "Level Pos: " + transform.position + "\n" +
                "Level Rot: " + transform.rotation);
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
				Camera.main.transform.GetChild(0).GetComponent<faceWhite>().FadeFromWhite(2);
                fallTimer = 0;
            } else if (fallTimer >= MAXFALLTIME){
                GetCheckpoint();
                Camera.main.transform.GetChild(0).GetComponent<faceWhite>().FadeFromWhite(2);
                fallTimer = 0;
            }
		}else{
			if(endTimer <= 0)
				Camera.main.transform.GetChild(0).GetComponent<faceWhite>().FadeWhite(5);
			endTimer += Time.deltaTime;
			if(endTimer >= 6)
				SceneManager.LoadScene("Museum");
		}
	}

	//set a checkpoint
	public void SetCheckpoint (Vector3 _player)
	{
		checkpoints[checkpointNum].playerPos = _player;
		checkpoints[checkpointNum].level = transform.position;
		checkpoints[checkpointNum].levelRot = transform.rotation;
		checkpoints[checkpointNum].progression = progression;
		checkpoints[checkpointNum].colour = transform.GetComponent<changeColour> ().colour;
	}
	
	//Reset to a checkpoint
	void GetCheckpoint ()
	{
		transform.rotation = checkpoints[checkpointNum].levelRot;
		transform.position = checkpoints[checkpointNum].level;
		player.position = checkpoints[checkpointNum].playerPos;
		progression = checkpoints[checkpointNum].progression;
		transform.GetComponent<changeColour> ().colour = checkpoints[checkpointNum].colour;
		transform.GetComponent<WorldRotation> ().rotTimer = 0;
		
		//This solves the fall through world bug
		player.GetComponent<CharacterMotorC> ().movement.velocity = Vector3.zero;
	}

	void SetDefaultPrefs ()
	{
			PlayerPrefs.SetString ("Prefs Set", "True");
			PlayerPrefs.SetFloat ("FoV", 60.0f);

			PlayerPrefs.Save ();
	}
}



/*CHANGELOG
 * 12-03-14: I adjusted the checkpointing system, now all checkpoints are hard-set in the editor. This is
 * to aid loading save games mid-level.
 * 
 * 
 * 
 * 
 * 
 */
