using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

    public static AudioManager AM;
    private static List<AudioSource> pausedAudio;
    public enum AUDIO_TYPES { Master, Sound, Music};
    public static bool isPaused = false;

	// Use this for initialization
	void Awake () {
        if (AM != null)
            Destroy(this.gameObject);
        else {
            AM = this;
            DontDestroyOnLoad(transform.gameObject);

            pausedAudio = new List<AudioSource>();

            if (!PlayerPrefs.HasKey("Master Volume")) {
                SetDefaultPrefs();
            }
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public static void SetDefaultPrefs() {
        PlayerPrefs.SetFloat("Master Volume", 1);
        PlayerPrefs.SetFloat("Sound Volume", 1);
        PlayerPrefs.SetFloat("Music Volume", 0.5f);

        PlayerPrefs.Save();
    }

    public static void SetVolume(AUDIO_TYPES type, float volume) {
        PlayerPrefs.SetFloat(type + " Volume", volume);
    }

    public static float GetVolume(AUDIO_TYPES type) {
        if (type == AUDIO_TYPES.Master)
            return PlayerPrefs.GetFloat(type + " Volume");
        else
            return PlayerPrefs.GetFloat(type + " Volume") * PlayerPrefs.GetFloat("Master Volume");
    }

    public static float GetRawVolume(AUDIO_TYPES type) {
        return PlayerPrefs.GetFloat(type + " Volume");
    }

    public static void PauseAll() {
        AudioSource[] temp = FindObjectsOfType<AudioSource>();
        foreach (AudioSource ads in temp) {
            if (ads.isPlaying) {
                ads.Pause();
                pausedAudio.Add(ads);
            }
        }
        isPaused = true;
    }

    public static void UnpauseAll() {
        foreach (AudioSource ads in pausedAudio) {
            if (ads != null) {
                    ads.UnPause();
            }
            
        }
        pausedAudio.Clear();
        isPaused = false;
    }
}
