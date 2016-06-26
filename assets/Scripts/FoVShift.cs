using UnityEngine;
using System.Collections;

public class FoVShift : MonoBehaviour {

	private CharacterMotorC playerMotor;
    private Camera cam;
	public bool fovshiftOn = true;

	// Use this for initialization
	void Start () {
		playerMotor = FindObjectOfType<CharacterMotorC>();
        cam = GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        fovshiftOn = (PlayerPrefs.GetString("FoVShift On") == "True");
        if (Input.GetMouseButton(1))
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 10.0f, 5f * Time.deltaTime);
        }
        else {
            if (fovshiftOn)
            {
                //float lookWhereYouGo = 1-(transform.forward.normalized-playerMotor.movement.velocity.normalized).magnitude*0.5f;
                if (playerMotor.movement.velocity.y < 0 && !playerMotor.grounded)
                {
                    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, PlayerPrefs.GetFloat("FoV") + 80.0f * (-playerMotor.movement.velocity.y / playerMotor.movement.maxFallSpeed), 1.0f * Time.deltaTime);
                }
                else
                    cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, PlayerPrefs.GetFloat("FoV"), 5f * Time.deltaTime);
            }
            else
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, PlayerPrefs.GetFloat("FoV"), 5f * Time.deltaTime);
        }
	}
}
