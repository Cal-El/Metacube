using UnityEngine;
using System.Collections;

public class activationObj : MonoBehaviour {

	public Vector3 around = Vector3.right;
	public float degrees = 90;
	public Color worldColour;
	public int ProShow;
	public int myCheckpoint =0;
	
	void Update (){
		//rotates the object in the way the world is about to
		transform.Rotate(around, 100*Time.deltaTime);
		//makes the cube only appear when progression through the level mandates it.
		if(ProShow !=GameManager.GM.progression){
			transform.GetChild(0).GetComponent<ParticleSystem>().enableEmission=false;
			GetComponent<Collider>().enabled = false;
			transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = Color.black;
		}
		else{
			transform.GetChild(0).GetComponent<ParticleSystem>().enableEmission=true;
			GetComponent<Collider>().enabled = true;
			transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material.color = worldColour;
		}
	}
}
