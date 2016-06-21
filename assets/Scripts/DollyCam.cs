using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DollyCam : MonoBehaviour {

    public AnimationCurve[] positions = new AnimationCurve[3];
    public AnimationCurve[] rotations = new AnimationCurve[3];

    public float totalTime;
    private float timer = 0;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
            SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1)%SceneManager.sceneCount+1);
        if (Input.GetKeyDown(KeyCode.RightControl))
            timer = 0;
        else if (Input.GetKey(KeyCode.RightArrow))
            timer += Time.deltaTime * 5;
        else if (Input.GetKey(KeyCode.LeftArrow))
            timer = Mathf.Max(timer - Time.deltaTime * 5, 0);
        else
            timer += Time.deltaTime;
        transform.position = PositionAtTime(timer);
        transform.rotation = RotationAtTime(timer);
    }

    Vector3 PositionAtTime(float t) {
        return new Vector3(positions[0].Evaluate(t), positions[1].Evaluate(t), positions[2].Evaluate(t));
    }

    Quaternion RotationAtTime(float t) {
        return Quaternion.Euler(new Vector3(rotations[0].Evaluate(t), rotations[1].Evaluate(t), rotations[2].Evaluate(t)));
    }
}
