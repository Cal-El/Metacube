﻿using UnityEngine;
using System.Collections;

public class TriggerPlatform : MonoBehaviour {

    public Transform activeTransform;
    public Transform inactiveTransform;
    public Transform platform;
    public AnimationCurve transistionAnimation;

    private bool isActive;
    private float timer = 0;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        timer = Mathf.Clamp((timer + (isActive ? Time.deltaTime : -Time.deltaTime)), 0, transistionAnimation.keys[transistionAnimation.keys.Length-1].time);
        platform.position = Vector3.Lerp(inactiveTransform.position, activeTransform.position, transistionAnimation.Evaluate(timer));
        platform.rotation = Quaternion.Lerp(inactiveTransform.rotation, activeTransform.rotation, transistionAnimation.Evaluate(timer));
        platform.localScale = Vector3.Lerp(inactiveTransform.localScale, activeTransform.localScale, transistionAnimation.Evaluate(timer));
    }

    void OnTriggerEnter(Collider e) {
        if(!isActive && e.tag == "Player") {
            isActive = true;
        }
    }

    void OnTriggerExit(Collider e) {
        if(isActive && e.tag == "Player") {
            isActive = false;
        }
    }
}
