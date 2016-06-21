using UnityEngine;
using System.Collections;

public class SplashElement : MonoBehaviour {

    public Vector3 FinalPosition = Vector3.zero;
    public Vector3 StartPosition;
    private static float TimeQueue1 = 5;
    private static float TimeQueue2 = 10;


    // Use this for initialization
    void Start () {
        //StartPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.timeSinceLevelLoad < TimeQueue1) {
            transform.position = Vector3.Lerp(transform.position, FinalPosition, Time.deltaTime);
        } else{
            transform.position = Vector3.Lerp(transform.position, StartPosition, Time.deltaTime);
        }
	}
}
