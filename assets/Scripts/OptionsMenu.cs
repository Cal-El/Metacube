using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour {

    public Slider fovSlider;
    public Slider sensitivity;
    public Toggle fovShift;
    public Image[] grayScaleImages;

	// Use this for initialization
	void Start () {
        FindObjectOfType<CharacterMotorC>().canControl = false;
        Time.timeScale = 0.0f;
        foreach(Image s in grayScaleImages)
        {
            s.color = FindObjectOfType<changeColour>().colour + Color.black;
        }
	}
	
	// Update is called once per frame
	void Update () {
        sensitivity.value = PlayerPrefs.GetFloat("Sensitivity");
        if(!Input.GetMouseButton(0))
        fovSlider.value = PlayerPrefs.GetFloat("FoV");
        fovShift.isOn = (PlayerPrefs.GetString("FoVShift On") == "True");
	}

    public void ResumeGame() {
        FindObjectOfType<CharacterMotorC>().canControl = true;
        Time.timeScale = 1.0f;
        Destroy(this.gameObject);
    }

    public void UpdateMouseSensitivity(float value) {
        PlayerPrefs.SetFloat("Sensitivity", value);
    }

    public void UpdateFoV(float value) {
        PlayerPrefs.SetFloat("FoV", value);
    }

    public void UpdateFoVShift(bool option) {
        if (option)
            PlayerPrefs.SetString("FoVShift On", "True");
        else
            PlayerPrefs.SetString("FoVShift On", "False");
    }

    public void Reset() {
        if (GameManager.GM != null) {
            DataManager.SetInt("Level " + GameManager.GM.LevelID + " Checkpoint", 0);
            GameManager.GM.checkpointNum = 0;
            GameManager.GM.GetCheckpoint();
        } else {
            for (int i = 1; i < 5; i++) {
                DataManager.SetInt("Level 1-" + i + " Checkpoint", 0);
                for (int j = 1; j < 5; j++) {
                    DataManager.SetBool("Art 1-" + i + "-" + j, false);
                }
            }
            DataManager.playerPos = Vector3.zero;
            SceneManager.LoadScene(0);
        }
        FindObjectOfType<faceWhite>().FadeFromWhite(2);
        FindObjectOfType<CharacterMotorC>().canControl = true;
        Time.timeScale = 1.0f;
        Destroy(this.gameObject);
    }

    public void Quit() {
        if (GameManager.GM != null) {
            Time.timeScale = 1.0f;
            DataManager.SetInt("Level " + GameManager.GM.LevelID + " Checkpoint", GameManager.GM.checkpointNum);
            SceneManager.LoadScene(0);
        } else {
            Time.timeScale = 1.0f;
            Application.Quit();
        }
    }
}
