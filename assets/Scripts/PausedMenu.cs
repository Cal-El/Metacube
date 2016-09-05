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
            GameManager.GM.Checkpoint = 0;
            GameManager.GM.GetCheckpoint();
        } else {
            for (int i = 1; i < 5; i++) {
                DataManager.SetInt("Level 1-" + i + " Checkpoint", 0);
                DataManager.SetBool("Level 1-" + i + " Finished", false);
                for (int j = 1; j < 5; j++) {
                    DataManager.SetBool("Art 1-" + i + "-" + j, false);
                }
            }
            DataManager.playerPos = Vector3.zero;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        FindObjectOfType<faceWhite>().FadeFromWhite(2);
        ResumeGame();
    }

    public void Quit() {
        if (GameManager.GM != null) {
            Time.timeScale = 1.0f;
            AudioManager.UnpauseAll();
            DataManager.SetInt("Level " + GameManager.GM.LevelID + " Checkpoint", GameManager.GM.Checkpoint);
            SceneManager.LoadScene("Museum");
        } else {
            Time.timeScale = 1.0f;
            Application.Quit();
        }
    }
}
