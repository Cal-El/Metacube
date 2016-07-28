using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class InteractableSound : MonoBehaviour {

    AudioSource ads;
    Interactable inter;
    EndLevelScript endInter;

    float personalVolume = 0;
    public float volumeModifier = 1;

	// Use this for initialization
	void Start () {
        ads = GetComponent<AudioSource>();
        inter = GetComponent<Interactable>();
        endInter = GetComponent<EndLevelScript>();

        ads.loop = true;	    
	}
	
	// Update is called once per frame
	void Update () {
        if (inter != null) {
            if (inter.active) {
                personalVolume = Mathf.Clamp(personalVolume+Time.deltaTime,0,1);
            } else {
                personalVolume = Mathf.Clamp(personalVolume-Time.deltaTime, 0, 1);
            }
        } else {
            if(endInter.showUpProgress == GameManager.GM.progression && !GameManager.GM.finishedLevel) {
                personalVolume = Mathf.Clamp(personalVolume + Time.deltaTime, 0, 1);
            } else {
                personalVolume = Mathf.Clamp(personalVolume - Time.deltaTime, 0, 1);
            }
        }
        ads.volume = AudioManager.GetVolume(AudioManager.AUDIO_TYPES.Sound) * personalVolume * volumeModifier;
	}
}
