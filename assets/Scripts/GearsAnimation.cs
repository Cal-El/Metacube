using UnityEngine;
using System.Collections;

public class GearsAnimation : MonoBehaviour {
	
	public int gearSize;
	public int direction = 1;
	public float speed = 10;
	
	private float spinSpeed;
	
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate(new Vector3(0.0f, 0.0f, spinSpeed*Time.timeScale*Time.deltaTime));
		spinSpeed = (direction*speed)/(gearSize * 2.0f);
	}
}
