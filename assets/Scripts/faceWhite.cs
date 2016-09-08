using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class faceWhite : MonoBehaviour {

	public static faceWhite instance;
	private float startTime;
	private float fadeLength;
	private bool toWhite;
	private Image visual;
	[SerializeField] private bool ignoreStart = false;

	void Awake(){
		instance = this;
		visual = GetComponent<Image>();
	}

	void Start(){
		if(MuseumManager.MM != null || GameManager.GM != null)
			FadeFromWhite(2);
	}

	// Update is called once per frame
	void Update () {
		if(Time.time < startTime + fadeLength && toWhite){
			visual.color = Color.Lerp(Color.clear, Color.white, (Time.time - startTime) / fadeLength);
		}else if (Time.time < startTime + fadeLength && !toWhite){
			visual.color = Color.Lerp (Color.white, Color.clear, (Time.time - startTime) / fadeLength);
		}else if(toWhite){
			visual.color = Color.white;
		}else{
			visual.sprite = null;
			visual.color = Color.clear;
		}
	}

	public static void FadeToWhite(float time){
		instance.startTime = Time.time;
		instance.fadeLength = time;
		instance.toWhite = true;
	}

	public static void FadeFromWhite (float time){
		instance.visual.color = Color.white;
		instance.startTime = Time.time;
		instance.fadeLength = time;
		instance.toWhite = false;
	}

	public static void FadeFromImage (Sprite img, float time){
		instance.visual.sprite = img;
		FadeFromWhite(time);
	}
}
