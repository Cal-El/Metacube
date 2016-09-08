using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PausedMenu : MonoBehaviour {

    public OptionsMenu op;
    public Image[] grayScaleImages;

	// Use this for initialization
	void Start () {
        FindObjectOfType<CharacterMotorC>().canControl = false;
        Time.timeScale = 0.0f;
        AudioManager.PauseAll();
        foreach(Image s in grayScaleImages)
        {
            s.color = FindObjectOfType<changeColour>().colour + Color.black;
        }
	}
	
	// Update is called once per frame
	void Update () {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (Input.GetButtonDown("Cancel") && FindObjectOfType<OptionsMenu>() == null)
            ResumeGame();
	}

    public void ResumeGame() {
        FindObjectOfType<CharacterMotorC>().canControl = true;
        Time.timeScale = 1.0f;
        AudioManager.UnpauseAll();
        Destroy(this.gameObject);
    }

    public void OptionsMenu() {
        Instantiate(op);
    }

    public void Reset() {
        if (GameManager.GM != null) {
            DataManager.SetInt("Level " + GameManager.GM.LevelID + " Checkpoint", 0);
            DataManager.SetFloat("Level " + GameManager.GM.LevelID + " Saved Timer", 0);
            GameManager.GM.fullLevelTimer = 0;
            GameManager.GM.CheckpointNum = 0;
            GameManager.GM.GetCheckpoint();
        } else {

            DataManager.ResetEverything();
            DataManager.playerPos = Vector3.zero;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        faceWhite.FadeFromWhite(2);
        ResumeGame();
    }

    public void Quit() {
        if (GameManager.GM != null) {
            Time.timeScale = 1.0f;
            AudioManager.UnpauseAll();
            DataManager.SetInt("Level " + GameManager.GM.LevelID + " Checkpoint", GameManager.GM.CheckpointNum);
            DataManager.SetFloat("Level " + GameManager.GM.LevelID + " Saved Timer", GameManager.GM.fullLevelTimer);

            SceneManager.LoadScene("Museum");
        } else {
            Time.timeScale = 1.0f;
            Application.Quit();
        }
    }
}
