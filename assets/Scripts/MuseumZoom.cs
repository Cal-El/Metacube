using UnityEngine;
using System.Collections;

public class MuseumZoom : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Zoom"))
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 10.0f, 5f * Time.deltaTime);
        } else { 
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, PlayerPrefs.GetFloat("FoV"), 5f * Time.deltaTime);
        }
	}
}
