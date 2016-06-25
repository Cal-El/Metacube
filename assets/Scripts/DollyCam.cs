using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DollyCam : MonoBehaviour {

    // Use this for initialization
    void Start() {
        if(DataManager.GetInt("Level " + GameManager.GM.LevelID + " Checkpoint") != 0) {
            GameManager.GM.player.GetComponent<CharacterMotorC>().canControl = true;
            foreach (faceWhite fw in FindObjectsOfType<faceWhite>()) {
                fw.FadeFromWhite(2);
            }
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update() {
        if(GetComponent<SplineInterpolator>().mState == "Stopped") {
            GameManager.GM.player.GetComponent<CharacterMotorC>().canControl = true;
            foreach (faceWhite fw in FindObjectsOfType<faceWhite>()) {
                fw.FadeFromWhite(2);
            }
            Destroy(this.gameObject);
        } else {
            GameManager.GM.player.GetComponent<CharacterMotorC>().canControl = false;
        }
    }
}
