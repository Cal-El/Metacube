using UnityEngine;
using System.Collections;

public class audioOneShot : MonoBehaviour {

    private AudioSource ads;
    public AudioClip[] clips;

	// Use this for initialization
	void Start () {
        ads = GetComponent<AudioSource>();
        ads.clip = clips[Random.Range(0, clips.Length)];
        ads.Play();
	}
	
	// Update is called once per frame
	void Update () {
        ads.volume = AudioManager.GetVolume(AudioManager.AUDIO_TYPES.Sound);
        if (!ads.isPlaying && !AudioManager.isPaused) {
            Destroy(this.gameObject);
        }
	}
}
