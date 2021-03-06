using UnityEngine;
using System.Collections;

public class activationObj : Interactable {

	public Vector3 around = Vector3.right;
	public float degrees = 90;
	public int ProShow;
	public int myCheckpoint =0;

    public Renderer r;
    public GameObject particle;
	
    void Start() {
        myColour = GameManager.GM.checkpoints[myCheckpoint].colour;
    }

	void Update (){
		//rotates the object in the way the world is about to
		transform.Rotate(around, 100*Time.deltaTime);
		//makes the cube only appear when progression through the level mandates it.
		if(ProShow !=GameManager.GM.progression){
            transform.GetChild(0).GetComponent<ParticleSystem>().enableEmission = false;
			GetComponent<Collider>().enabled = false;
            r.enabled = false;
            active = false;

            r.material.color = Color.black;
		}
		else{
            transform.GetChild(0).GetComponent<ParticleSystem>().enableEmission = true;
			GetComponent<Collider>().enabled = true;
            r.enabled = true;
            active = true;

            r.material.color = myColour;
		}
	}
}
