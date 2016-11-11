using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour {

    public Transform platform;
    public Transform startTransform;
    public Transform endTransform;
    public AnimationCurve transitionAnimation;

    private float lerper = 0;
    private int direction = 1;
	
	// Update is called once per frame
	void FixedUpdate () {
        lerper = Mathf.Clamp(lerper + Time.fixedDeltaTime * direction, 0, transitionAnimation.keys[transitionAnimation.keys.Length-1].time);

        if (lerper <= 0 || lerper >= transitionAnimation.keys[transitionAnimation.keys.Length - 1].time)
            direction *= -1;

        platform.position = Vector3.Lerp(startTransform.position, endTransform.position, transitionAnimation.Evaluate(lerper));
        platform.rotation = Quaternion.Lerp(startTransform.rotation, endTransform.rotation, transitionAnimation.Evaluate(lerper));
        platform.localScale = Vector3.Lerp(startTransform.localScale, endTransform.localScale, transitionAnimation.Evaluate(lerper));
    }
}
