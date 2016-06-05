using UnityEngine;
using System.Collections;

public class DollyCam : MonoBehaviour {

    public Vector3[] Positions;
    public Vector3[] Rotations;
    public float[] TimeStamps;
    private float timer = 0;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
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
        int index = 0;
        while (index < TimeStamps.Length - 1 && t > TimeStamps[index+1]) {
            index++;
        }
        if (index >= TimeStamps.Length - 1) {
            return Positions[index];
        } else {
            float lerper = (timer - TimeStamps[index]) / (TimeStamps[index + 1] - TimeStamps[index]);
            return Vector3.Lerp(Positions[index], Positions[index + 1], lerper);
        }
    }

    Quaternion RotationAtTime(float t) {
        int index = 0;
        while (index < TimeStamps.Length - 1 && t > TimeStamps[index + 1]) {
            index++;
        }
        if (index >= TimeStamps.Length - 1) {
            return Quaternion.Euler(Rotations[index]);
        } else {
            float lerper = (timer - TimeStamps[index]) / (TimeStamps[index + 1] - TimeStamps[index]);
            return (Quaternion.Lerp(Quaternion.Euler(Rotations[index]), Quaternion.Euler(Rotations[index + 1]), lerper));
        }
    }
}
