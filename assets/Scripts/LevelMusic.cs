using UnityEngine;
using System.Collections;

public class LevelMusic : MonoBehaviour {

    [System.Serializable]
    public struct MusicSection {
        public AudioClip clip;
        public float volume;
        public AudioSource player;
    }

    public MusicSection[] tracks;

	// Use this for initialization
	void Start () {
        //AudioSource[] ads = GetComponents<AudioSource>();
        for (int i = 0; i < tracks.Length; i++) {
            tracks[i].player = gameObject.AddComponent<AudioSource>();
            tracks[i].player.clip = tracks[i].clip;
            tracks[i].player.loop = true;
            tracks[i].player.volume = 0;
            tracks[i].player.Play();
            tracks[i].volume = 0;
        }
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.GM.checkpointNum < GameManager.GM.checkpoints.Length - 1) {
            for (int i = 0; i < tracks.Length; i++) {
                tracks[i].volume = Mathf.Lerp(tracks[i].volume, ConditionToInt(GameManager.GM.progression >= i && !GameManager.GM.finishedLevel), Time.deltaTime);
                tracks[i].player.volume = AudioManager.GetVolume(AudioManager.AUDIO_TYPES.Music) * tracks[i].volume;
            }
        } else if(!GameManager.GM.finishedLevel) {
            for (int i = 1; i < tracks.Length; i++) {
                tracks[i].volume = Mathf.Lerp(tracks[i].volume, 0, Time.deltaTime);
                tracks[i].player.volume = AudioManager.GetVolume(AudioManager.AUDIO_TYPES.Music) * tracks[i].volume;
            }
        } else {
            for (int i = 0; i < tracks.Length; i++) {
                tracks[i].volume = Mathf.Lerp(tracks[i].volume, 0, Time.deltaTime);
                tracks[i].player.volume = AudioManager.GetVolume(AudioManager.AUDIO_TYPES.Music) * tracks[i].volume;
            }
        }
    }

    int ConditionToInt(bool b) {
        if (b)
            return 1;
        else
            return 0;
    }
}
