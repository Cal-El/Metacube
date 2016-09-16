using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(SplineController))]

public class DollyCam : MonoBehaviour {

    const float TIMETORELOAD = 0.5f;
    float reloadTimer = 0;

    // Use this for initialization
    void Start() {
        Camera.main.GetComponent<AudioListener>().enabled = false;
        GetComponent<AudioListener>().enabled = true;
        if (DataManager.GetInt("Level " + GameManager.GM.LevelID + " Checkpoint") != 0) {
            GetComponent<SplineController>().DisableTransforms();
            EndCutscene();
        }
    }

    // Update is called once per frame
    void Update() {
        
        if (GetComponent<SplineInterpolator>().mState == "Stopped") {
            EndCutscene();
        } else {
            GameManager.GM.player.GetComponent<CharacterMotorC>().canControl = false;
        }
    }

    public void EndCutscene() {
        GameManager.GM.player.GetComponent<CharacterMotorC>().canControl = true;
		faceWhite.FadeFromWhite (1);
        GetComponent<AudioListener>().enabled = false;
        Camera.main.GetComponent<AudioListener>().enabled = true;
        Destroy(this.gameObject);
    }
}
