using UnityEngine;
using System.Collections;

public class LandingBobble : MonoBehaviour {
	
	[Tooltip("Animation curve representing the vertical movement of the head bobble on landing. Normalised between 0-1")]
	public AnimationCurve landingAnimation;
    public AnimationCurve jumpingAnimation;
    public bool wasGrounded = true;
	public CharacterMotorC chaCon;
	
	private float timer = 0.0f;
	private float magnitude = 0.0f;
	private float landingAnimationTime;
    private float jumpingAnimationTime;
    private bool isPlaying;
    private bool isJumping;
	private Camera myCam;
	private Vector3 localCamPos;
	
	void Start(){
		if(chaCon == null) chaCon = FindObjectOfType<CharacterMotorC>();

        jumpingAnimationTime = jumpingAnimation.keys[jumpingAnimation.length - 1].time;
        landingAnimationTime = landingAnimation.keys[landingAnimation.length - 1].time;

        myCam = GetComponent<Camera>();
		localCamPos = myCam.transform.localPosition;
	}
	
	void Update(){
        if (chaCon.canControl && chaCon.grounded && !wasGrounded) {
            magnitude = -chaCon.movement.velocity.y / chaCon.movement.maxFallSpeed;
            timer = 0;
            isPlaying = true;
            isJumping = false;
        } else if (chaCon.canControl && wasGrounded && Input.GetButtonDown("Jump")) {
            magnitude = 1;
            timer = 0;
            isPlaying = true;
            isJumping = true;
        }


		if(isPlaying){
			if(isJumping && timer <= jumpingAnimationTime){
                timer += Time.deltaTime;
                myCam.transform.localPosition = new Vector3(localCamPos.x, localCamPos.y + jumpingAnimation.Evaluate(timer) * magnitude, localCamPos.z);
            } else if(!isJumping && timer <= landingAnimationTime) {
				timer += Time.deltaTime;
				myCam.transform.localPosition = new Vector3(localCamPos.x, localCamPos.y + landingAnimation.Evaluate(timer)*magnitude, localCamPos.z);
			} else {
                myCam.transform.localPosition = localCamPos;
                isPlaying = false;
                magnitude = 0.0f;
            }
		}
		wasGrounded = chaCon.grounded;
	}
}