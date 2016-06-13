using UnityEngine;
using System.Collections;

public class WorldRotation : MonoBehaviour {
	 
	public Transform player;
	public float timePerRotation = 5;
	private float speed;
	public float rotTimer = 0;
	private float totalRotation;
	private float rotDegrees;
	private Vector3 rotationAround;
	
	// Update is called once per frame
	void Update () {
		if(rotTimer>0){
			totalRotation += speed*Time.deltaTime;
			if(totalRotation<rotDegrees)
				transform.RotateAround(player.position, rotationAround, speed*Time.deltaTime);
			else 
				transform.RotateAround(player.position, rotationAround, speed*Time.deltaTime-(totalRotation-rotDegrees));
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
	public void startRotation(Vector3 around, float degrees, Transform destroy)
	{
		rotationAround = around;
		speed = degrees/timePerRotation;
		rotTimer = timePerRotation;
		totalRotation = 0;
		rotDegrees = degrees;
		
		GameManager.GM.progression ++;
		
	}
	public void startRotation(Vector3 around, float degrees, Transform destroy, Color passColor)
	{
		rotationAround = around;
		speed = degrees/timePerRotation;
		rotTimer = timePerRotation;
		totalRotation = 0;
		rotDegrees = degrees;
		
		GameManager.GM.progression ++;
		
		GetComponent<changeColour>().SetColour(passColor);
		
	}
}
