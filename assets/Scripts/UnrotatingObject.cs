using UnityEngine;
using System.Collections;

public class UnrotatingObject : MonoBehaviour {

	public Transform pointInLevel;
	
	// Update is called once per frame
	void Update () {
		transform.position = pointInLevel.position;
	}
}
