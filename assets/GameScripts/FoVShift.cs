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
        fovshiftOn = (PlayerPrefs.GetString("FoVShift On") == "True");
        if (Input.GetMouseButton(1))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 10.0f, 5f * Time.deltaTime);
        }
        else {
            if (fovshiftOn)
            {
                //float lookWhereYouGo = 1-(transform.forward.normalized-playerMotor.movement.velocity.normalized).magnitude*0.5f;
                if (playerMotor.movement.velocity.y < 0 && !playerMotor.grounded)
                {
                    Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, PlayerPrefs.GetFloat("FoV") + 80.0f * (-playerMotor.movement.velocity.y / playerMotor.movement.maxFallSpeed), 1.0f * Time.deltaTime);
                }
                else
                    Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, PlayerPrefs.GetFloat("FoV"), 5f * Time.deltaTime);
            }
            else
                Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, PlayerPrefs.GetFloat("FoV"), 5f * Time.deltaTime);
        }
	}
}
