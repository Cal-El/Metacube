using UnityEngine;
using System.Collections;

public class LevelMusic : MonoBehaviour {

    [System.Serializable]
    public struct MusicSection {
    	[Tooltip("Clip of music to play.")]
        public AudioClip clip;
        [Tooltip("Track volume at level progression x. Cannot exceed 1.")]
        public AnimationCurve volumeCurve;
        [HideInInspector]
        public AudioSource player;
    }

    public MusicSection[] tracks;
    private float progressionFloat = 0.0f;

	// Use this for initialization
	void Start () {
        //AudioSource[] ads = GetComponents<AudioSource>();
        progressionFloat = GameManager.GM.progression;
        
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
    	progressionFloat = Mathf.Lerp(progressionFloat, GameManager.GM.progression, Time.deltaTime);
    	
    	for (int i = 0; i < tracks.Length; i++) {
        	tracks[i].player.volume = AudioManager.GetVolume(AudioManager.AUDIO_TYPES.Music) * tracks[i].volumeCurve.Evaluate(progressionFloat);
        }
    }
}
