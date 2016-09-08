using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour {

    bool whiteFade = false;
    float targetTime = 5;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(!whiteFade && Input.GetButtonDown("Jump")) {
            targetTime = Time.timeSinceLevelLoad;
        }

        if(Time.timeSinceLevelLoad > targetTime && !whiteFade) {
            whiteFade = true;
            faceWhite.FadeToWhite(1.5f);
        }
	    if(Time.timeSinceLevelLoad > targetTime+2) {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Museum");
        }
	}
}
