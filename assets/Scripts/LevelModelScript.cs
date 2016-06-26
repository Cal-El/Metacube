using UnityEngine;
using System.Collections;

public class LevelModelScript : Interactable {

	public float speed;
	public string levelNo;

	void Update(){
		transform.RotateAround (transform.position, Vector3.up, speed * Time.deltaTime);
        active = true;
	}
}
