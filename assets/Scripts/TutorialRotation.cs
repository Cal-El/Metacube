using UnityEngine;
using System.Collections;

public class TutorialRotation : WorldRotation {
	
	// Update is called once per frame
	void Update () {
        Debug.Log(rotTimer);

		if(rotTimer>0){
            base.totalRotation += base.speed*Time.deltaTime;
			if(base.totalRotation < base.rotDegrees)
				transform.RotateAround(transform.position, base.rotationAround, base.speed *Time.deltaTime);
			else 
				transform.RotateAround(transform.position, base.rotationAround, base.speed *Time.deltaTime-(base.totalRotation - base.rotDegrees));
			rotTimer-= Time.deltaTime;
		}
		
	}
}
