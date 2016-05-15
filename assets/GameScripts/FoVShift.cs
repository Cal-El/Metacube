using UnityEngine;
using System.Collections;

public class FoVShift : MonoBehaviour {

	public CharacterMotorC playerMotor;
	public bool fovshiftOn = true;

	// Use this for initialization
	void Start () {
		playerMotor = transform.parent.GetComponent<CharacterMotorC>();
	}
	
	// Update is called once per frame
	void Update () {
		if(fovshiftOn){
			//float lookWhereYouGo = 1-(transform.forward.normalized-playerMotor.movement.velocity.normalized).magnitude*0.5f;
			if (playerMotor.movement.velocity.y<0 && !playerMotor.grounded) {
				Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, PlayerPrefs.GetFloat("FoV") + 100.0f*(-playerMotor.movement.velocity.y/playerMotor.movement.maxFallSpeed), 1.0f*Time.deltaTime);
			} else
				Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, PlayerPrefs.GetFloat("FoV"), 5f*Time.deltaTime);
		} else
			Camera.main.fieldOfView = PlayerPrefs.GetFloat("FoV");
	}
}
