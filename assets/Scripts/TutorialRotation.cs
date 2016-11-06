using UnityEngine;
using System.Collections;

public class TutorialRotation : WorldRotation {
	
    void Update() {

    }

	// Update is called once per frame
	void FixedUpdate () {
        /*
        Debug.Log(rotTimer);

		if(rotTimer>0){
            base.totalRotation += base.speed*Time.fixedDeltaTime;
			if(base.totalRotation < base.rotDegrees)
				transform.RotateAround(transform.position, base.rotationAround, base.speed *Time.fixedDeltaTime);
			else 
				transform.RotateAround(transform.position, base.rotationAround, base.speed *Time.fixedDeltaTime - (base.totalRotation - base.rotDegrees));
			rotTimer-= Time.fixedDeltaTime;
		}
		*/
	}
}
