using UnityEngine;
using System.Collections;

public class TuteRotation : MonoBehaviour {
	 
	public Transform point;
	public float timePerRotation = 5;
	private float speed;
	private float rotTimer = 0;
	private float totalRotation;
	private float rotDegrees;
	private Vector3 rotationAround;
	
	// Update is called once per frame
	void Update () {
		if(rotTimer>0){
			totalRotation += speed*Time.deltaTime;
			if(totalRotation<rotDegrees)
				transform.RotateAround(point.position, rotationAround, speed*Time.deltaTime);
			else 
				transform.RotateAround(point.position, rotationAround, speed*Time.deltaTime-(totalRotation-rotDegrees));
			rotTimer-= Time.deltaTime;
		}
	}
	
	public void startRotation(Vector3 around, Transform destroy)
	{
		rotationAround = around;
		speed = 90/timePerRotation;
		rotTimer = timePerRotation;
		totalRotation = 0;
		rotDegrees = 90;
		Destroy(destroy.gameObject);
	}
}
