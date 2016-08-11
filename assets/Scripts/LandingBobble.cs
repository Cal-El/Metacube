using UnityEngine;
using System.Collections;

public class LandingBobble : MonoBehaviour {
	
	[Tooltip("Animation curve representing the vertical movement of the head bobble on landing. Normalised between 0-1")]
	public AnimationCurve bobbleAnimation;
	public bool wasGrounded = true;
	public CharacterMotorC chaCon;
	
	private float timer = 0.0f;
	private float magnitude = 0.0f;
	private float fullAnimationTime;
	private bool isPlaying;
	private Camera myCam;
	private Vector3 localCamPos;
	
	void Start(){
		if(chaCon == null) chaCon = FindObjectOfType<CharacterMotorC>();
		fullAnimationTime = bobbleAnimation.keys[bobbleAnimation.length-1].time;
		
		myCam = GetComponent<Camera>();
		localCamPos = myCam.transform.localPosition;
	}
	
	void Update(){
		if(chaCon.grounded && !wasGrounded){
			magnitude = -chaCon.movement.velocity.y/chaCon.movement.maxFallSpeed;
			timer = 0;
			isPlaying = true;
		}
		if(isPlaying){
			if(timer >= fullAnimationTime){
				myCam.transform.localPosition = localCamPos;
				isPlaying = false;
				magnitude = 0.0f;
			}else{
				timer += Time.deltaTime;
				myCam.transform.localPosition = new Vector3(localCamPos.x, localCamPos.y + bobbleAnimation.Evaluate(timer)*magnitude, localCamPos.z);
			}
		}
		wasGrounded = chaCon.grounded;
	}
}