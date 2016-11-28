using UnityEngine;
using System.Collections;

public class TriggerPlatform : MonoBehaviour {

    public enum BEHAVIOURS { Twoway, Oneway, ProgressionLocked }
    public BEHAVIOURS behaviour = BEHAVIOURS.Twoway;
    public int unlockProgression = 0;

    public Transform activeTransform;
    public Transform inactiveTransform;
    public Transform platform;
    public AnimationCurve transistionAnimation;

    private bool isTransitioning = true;
    private bool isActive;
    private float timer = 0;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        if (isTransitioning) {
            timer = Mathf.Clamp((timer + (isActive ? Time.deltaTime : -Time.deltaTime)), 0, transistionAnimation.keys[transistionAnimation.keys.Length - 1].time);
            platform.position = Vector3.Lerp(inactiveTransform.position, activeTransform.position, transistionAnimation.Evaluate(timer));
            platform.rotation = Quaternion.Lerp(inactiveTransform.rotation, activeTransform.rotation, transistionAnimation.Evaluate(timer));
            platform.localScale = Vector3.Lerp(inactiveTransform.localScale, activeTransform.localScale, transistionAnimation.Evaluate(timer));
            if (timer <= 0 || timer >= transistionAnimation.keys[transistionAnimation.keys.Length - 1].time)
                isTransitioning = false;
        }
    }

    void OnTriggerEnter(Collider e) {
        //if(-p AND q AND r IMPLIES s)
        if(!isActive && e.tag == "Player" && !(behaviour == BEHAVIOURS.ProgressionLocked && GameManager.GM.progression < unlockProgression)) {
            isActive = true;
            isTransitioning = true;
        }
    }

    void OnTriggerExit(Collider e) {
        if(isActive && e.tag == "Player" && behaviour == BEHAVIOURS.Twoway) {
            isActive = false;
            isTransitioning = true;
        }
    }
}
