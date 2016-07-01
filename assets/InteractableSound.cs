using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class InteractableSound : MonoBehaviour {

    AudioSource ads;
    float personalVolume = 0;

	// Use this for initialization
	void Start () {
        ads = GetComponent<AudioSource>();
        ads.loop = true;	    
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<Interactable>() != null) {
            if (GetComponent<Interactable>().active) {
                personalVolume = Mathf.Clamp(personalVolume+Time.deltaTime,0,1);
            } else {
                personalVolume = Mathf.Clamp(personalVolume-Time.deltaTime, 0, 1);
            }
        } else if(GetComponent<EndLevelScript>() != null) {
            if(GetComponent<EndLevelScript>().showUpProgress == GameManager.GM.progression && !GameManager.GM.finishedLevel) {
                personalVolume = Mathf.Clamp(personalVolume + Time.deltaTime, 0, 1);
            } else {
                personalVolume = Mathf.Clamp(personalVolume - Time.deltaTime, 0, 1);
            }
        }
        ads.volume = AudioManager.GetVolume(AudioManager.AUDIO_TYPES.Sound) * personalVolume;
	}
}
