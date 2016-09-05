using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MuseumManager : MonoBehaviour {

	public static MuseumManager MM;
	public Transform player;
    public GameObject pauseMenu;

    [System.Serializable]
    public struct Checkpoint {
        public Vector3 position;
        public Vector3 rotation;
    }
    public Checkpoint[] checkpoints;

    private bool loadingLevel = false;
    private float levelTimer = 0;
    private string levelName;

	// Use this for initialization
	void Awake () {
		MM = this;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (DataManager.playerPos != Vector3.zero) {
            player.transform.position = DataManager.playerPos;
            player.transform.rotation = DataManager.playerRot;
        } else {
            if (checkpoints.Length > DataManager.GetProgress()) {
                player.transform.position = checkpoints[DataManager.GetProgress()].position;
                player.transform.rotation = Quaternion.Euler(checkpoints[DataManager.GetProgress()].rotation);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetButtonDown("Cancel") && FindObjectOfType<PausedMenu>() == null) {
            Instantiate(pauseMenu);
        }
        if (loadingLevel) {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 10.0f, 5*Time.deltaTime);
            player.GetComponent<CharacterMotorC>().canControl = false;
            levelTimer += Time.deltaTime;
            if(levelTimer >= 1) {
                SceneManager.LoadScene(levelName);
                PlaytestData.AddEvent(new PlaytestData.EventData("Starting Scene: " + levelName));
            }
        }
	}

    public void LoadLevel(string s) {
        DataManager.playerPos = player.position;
        DataManager.playerRot = player.rotation;
        FindObjectOfType<faceWhite>().FadeWhite(1);
        levelName = s;
        loadingLevel = true;
        Camera.main.GetComponent<MuseumZoom>().enabled = false;
    }
}
