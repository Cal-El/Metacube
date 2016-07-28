using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

    public static AudioManager AM;
    private static List<AudioSource> pausedAudio;
    public enum AUDIO_TYPES { Master, Sound, Music};
    public static bool isPaused = false;

    private float[] volumes = { 0, 0, 0 };

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
        volumes[0] = PlayerPrefs.GetFloat(AUDIO_TYPES.Master + " Volume");
        volumes[1] = PlayerPrefs.GetFloat(AUDIO_TYPES.Sound + " Volume");
        volumes[2] = PlayerPrefs.GetFloat(AUDIO_TYPES.Music + " Volume");
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
            return AM.volumes[0];
        else
            return AM.volumes[0] * AM.volumes[(int)type];
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
