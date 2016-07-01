using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour {

    public Slider fovSlider;
    public Slider sensitivity;
    public Slider maVol;
    public Slider soVol;
    public Slider muVol;

    public Toggle fovShift;
    public Dropdown quality;
    public Image[] grayScaleImages;

	// Use this for initialization
	void Start () {
        foreach(Image s in grayScaleImages)
        {
            s.color = FindObjectOfType<changeColour>().colour + Color.black;
        }
	}
	
	// Update is called once per frame
	void Update () {
        sensitivity.value = PlayerPrefs.GetFloat("Sensitivity");
        if (!Input.GetMouseButton(0)) {
            fovSlider.value = PlayerPrefs.GetFloat("FoV");
            maVol.value = AudioManager.GetRawVolume(AudioManager.AUDIO_TYPES.Master);
            soVol.value = AudioManager.GetRawVolume(AudioManager.AUDIO_TYPES.Sound);
            muVol.value = AudioManager.GetRawVolume(AudioManager.AUDIO_TYPES.Music);
        }
        fovShift.isOn = (PlayerPrefs.GetString("FoVShift On") == "True");

        List<Dropdown.OptionData> ops = new List<Dropdown.OptionData>();
        foreach(string s in QualitySettings.names) {
            ops.Add(new Dropdown.OptionData(s));
        }
        quality.options = ops;
        quality.value = QualitySettings.GetQualityLevel();

        if (Input.GetKeyDown(KeyCode.Escape))
            BackButton();
	}

    public void BackButton() {
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

    public void SetMasterVolume(float value) {
        AudioManager.SetVolume(AudioManager.AUDIO_TYPES.Master, value);
    }
    public void SetSoundVolume(float value) {
        AudioManager.SetVolume(AudioManager.AUDIO_TYPES.Sound, value);
    }
    public void SetMusicVolume(float value) {
        AudioManager.SetVolume(AudioManager.AUDIO_TYPES.Music, value);
    }

    public void UpdateQualitySettings(int q) {
        QualitySettings.SetQualityLevel(q);
    }

    public void RestoreDefaults() {
        PlayerPrefs.SetString("Prefs Set", "True");
        PlayerPrefs.SetFloat("FoV", 60.0f);
        PlayerPrefs.SetString("FoVShift On", "True");
        PlayerPrefs.SetFloat("Sensitivity", 5.0f);
        QualitySettings.SetQualityLevel(QualitySettings.names.Length / 2);
        AudioManager.SetVolume(AudioManager.AUDIO_TYPES.Master, 100);
        AudioManager.SetVolume(AudioManager.AUDIO_TYPES.Sound, 100);
        AudioManager.SetVolume(AudioManager.AUDIO_TYPES.Music, 100);

        PlayerPrefs.Save();
    }
}
