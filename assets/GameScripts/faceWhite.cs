using UnityEngine;
using System.Collections;

public class faceWhite : MonoBehaviour {

	float timer = 0;
	float lerpSpeed;
	bool toWhite;

	void Start(){
		FadeFromWhite(2);
	}

	// Update is called once per frame
	void Update () {
		if(timer >0 && toWhite){
			GetComponent<Renderer>().material.color = Color.Lerp(GetComponent<Renderer>().material.color, Color.white, 3/lerpSpeed*Time.deltaTime);
		}else if (timer > 0 && !toWhite){
			GetComponent<Renderer>().material.color = Color.Lerp(GetComponent<Renderer>().material.color, Color.clear, 3/lerpSpeed *Time.deltaTime);
		}else if(toWhite){
			GetComponent<Renderer>().material.color = Color.white;
		}else{
			GetComponent<Renderer>().material.color = Color.clear;
		}

		timer -= Time.deltaTime;
	}

	public void FadeWhite(float time){
		timer = time;
		lerpSpeed = time;
		toWhite = true;
	}

	public void FadeFromWhite (float time){
		GetComponent<Renderer>().material.color = Color.white;
		timer = time;
		lerpSpeed = time;
		toWhite = false;
	}
}
