using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour {

    bool whiteFade = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(Time.timeSinceLevelLoad > 5 && !whiteFade) {
            whiteFade = true;
            GetComponentInChildren<faceWhite>().FadeWhite(1.5f);
        }
	    if(Time.timeSinceLevelLoad > 7) {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Museum");
        }
	}
}
