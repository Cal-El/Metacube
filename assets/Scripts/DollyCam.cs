using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(SplineController))]

public class DollyCam : MonoBehaviour {

    // Use this for initialization
    void Start() {
        Camera.main.GetComponent<AudioListener>().enabled = false;
        GetComponent<AudioListener>().enabled = true;
        if (DataManager.GetInt("Level " + GameManager.GM.LevelID + " Checkpoint") != 0) {
            EndCutscene();
        }
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            EndCutscene();
        else if (GetComponent<SplineInterpolator>().mState == "Stopped") {
            EndCutscene();
        } else {
            GameManager.GM.player.GetComponent<CharacterMotorC>().canControl = false;
        }
    }

    void EndCutscene() {
        GameManager.GM.player.GetComponent<CharacterMotorC>().canControl = true;
        foreach (faceWhite fw in FindObjectsOfType<faceWhite>()) {
            fw.FadeFromWhite(2);
        }
        GetComponent<AudioListener>().enabled = false;
        Camera.main.GetComponent<AudioListener>().enabled = true;
        Destroy(this.gameObject);
    }
}
