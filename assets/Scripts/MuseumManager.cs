using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MuseumManager : MonoBehaviour {

	public static MuseumManager MM;
	public Transform player;
    public GameObject optionsMenu;

    private bool loadingLevel = false;
    private float levelTimer = 0;
    private string levelName;

	// Use this for initialization
	void Awake () {
		MM = this;
        if(DataManager.playerPos != Vector3.zero) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = DataManager.playerPos;
            player.transform.rotation = DataManager.playerRot;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Escape) && FindObjectOfType<PausedMenu>() == null) {
            Instantiate(optionsMenu);
        }
        if (loadingLevel) {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 10.0f, 5*Time.deltaTime);
            player.GetComponent<CharacterMotorC>().canControl = false;
            levelTimer += Time.deltaTime;
            if(levelTimer >= 1) {
                SceneManager.LoadScene(levelName);
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
