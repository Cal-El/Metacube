using UnityEngine;
using System.Collections;

public class DestroyParticle : MonoBehaviour {

    public float maxTimer = 0;
    private float timer;

	// Use this for initialization
	void Start () {
        timer = maxTimer;
	}
	
	// Update is called once per frame
	void Update () {
        if (maxTimer <= 0) {
            if (!GetComponent<ParticleSystem>().isPlaying) { Destroy(this.gameObject); }
        } else { timer -= Time.deltaTime; if (timer <= 0) Destroy(this.gameObject); }
            
	}
}
