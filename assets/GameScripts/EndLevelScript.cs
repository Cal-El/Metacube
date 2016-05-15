using UnityEngine;
using System.Collections;

public class EndLevelScript : MonoBehaviour {

	public int showUpProgress;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(GameManager.GM.progression == showUpProgress){
			GetComponent<ParticleSystem>().Play();
			if((GameManager.GM.player.position-transform.position).magnitude<5)
				GameManager.GM.finishedLevel = true;
		}else 
			GetComponent<ParticleSystem>().Stop();

	}
}
