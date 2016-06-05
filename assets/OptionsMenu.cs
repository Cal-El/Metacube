using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour {

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
        fovShift.isOn = FindObjectOfType<FoVShift>().fovshiftOn;
	}

    public void ResumeGame() {
        FindObjectOfType<CharacterMotorC>().canControl = true;
        Time.timeScale = 1.0f;
        Destroy(this.gameObject);
    }

    public void UpdateMouseSensitivity(float value) {
        PlayerPrefs.SetFloat("Sensitivity", value);
    }

    public void UpdateFoVShift(bool option) {
        FindObjectOfType<FoVShift>().fovshiftOn = option;
    }

    public void Quit() {
        if (GameManager.GM != null) {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(0);
        } else {
            Time.timeScale = 1.0f;
            Application.Quit();
        }
    }
}
