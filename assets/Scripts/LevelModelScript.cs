using UnityEngine;
using System.Collections;

public class LevelModelScript : Interactable {

	public float speed;
	public string levelNo;

    void Start() {
        myColour = GetComponentInChildren<Renderer>().material.GetColor("_EmissionColor");
    }

	void Update(){
		transform.RotateAround (transform.position, Vector3.up, speed * Time.deltaTime);
        active = true;
	}
}
