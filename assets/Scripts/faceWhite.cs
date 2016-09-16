using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class faceWhite : MonoBehaviour {

	public static faceWhite instance;
	private float startTime;
	private float fadeLength;
	private bool toWhite;
    [SerializeField]
     private Image visual;
	[SerializeField] private bool ignoreStart = false;

	void Awake(){
        if (instance != null) {
            faceWhite.FadeFromImage(instance.visual.sprite, 2);
            Destroy(this.gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(transform.parent.gameObject);
            faceWhite.FadeFromImage(instance.visual.sprite, 2);
        }
		visual = GetComponent<Image>();
	}

	void Start(){
		
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

    public static void FadeToImage(Sprite img, float time) {
        instance.visual.sprite = img;
        FadeToWhite(time);
    }

    public static void FadeFromWhite (float time){
        if(instance == null) {
            instance = FindObjectOfType<faceWhite>();
            instance.Start();
        }
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
