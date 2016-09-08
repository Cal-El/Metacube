using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class faceWhite : MonoBehaviour {

	public static faceWhite instance;
	private float timer = 0;
	private float lerpSpeed;
	private bool toWhite;
	private Image visual;
   	[SerializeField] private bool ignoreStart = false;

	void Awake(){
		instance = this;
		visual = GetComponent<Image>();
	}

	void Start(){
        	if(!ignoreStart)
        	    FadeFromWhite(2);
	}

	// Update is called once per frame
	void Update () {
		if(timer >0 && toWhite){
			visual.color = Color.Lerp(visual.color, Color.white, 3/lerpSpeed*Time.deltaTime);
		}else if (timer > 0 && !toWhite){
			visual.color = Color.Lerp(visual.color, Color.clear, 3/lerpSpeed *Time.deltaTime);
		}else if(toWhite){
			visual.color = Color.white;
		}else{
			visual.sprite = null;
			visual.color = Color.clear;
		}

		timer -= Time.deltaTime;
	}

	public static void FadeToWhite(float time){
		instance.timer = time;
		instance.lerpSpeed = time;
		instance.toWhite = true;
	}

	public static void FadeFromWhite (float time){
		visual.color = Color.white;
		instance.timer = time;
		instance.lerpSpeed = time;
		instance.toWhite = false;
	}
	
	public static void FadeFromImage (Sprite img, float time){
		visual.sprite = img;
		FadeFromWhite(time);
	}
}
