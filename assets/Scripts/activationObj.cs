using UnityEngine;
using System.Collections;

public class activationObj : Interactable {

    public enum AXIS { pos_x, pos_y, pos_z, neg_x, neg_y, neg_z}
    public AXIS around = AXIS.pos_x;
	public float degrees = 0;
    public float rotationSpeed;
	public int ProShow;
	public int myCheckpoint =0;

    public Renderer r;
    public GameObject particle;
	
    void Start() {
        myColour = GameManager.GM.checkpoints[myCheckpoint].colour;
    }

	void Update (){
		//rotates the object in the way the world is about to
		//makes the cube only appear when progression through the level mandates it.
		if(ProShow !=GameManager.GM.progression){
            transform.GetChild(0).GetComponent<ParticleSystem>().enableEmission = false;
			GetComponent<Collider>().enabled = false;
            r.enabled = false;
            active = false;

            r.material.color = Color.black;
		}
		else{
            Vector3 axis = Vector3.up;
            switch (around) {
                case AXIS.pos_x:
                    axis = GameManager.GM.transform.right;
                    break;
                case AXIS.pos_y:
                    axis = GameManager.GM.transform.up;
                    break;
                case AXIS.pos_z:
                    axis = GameManager.GM.transform.forward;
                    break;
                case AXIS.neg_x:
                    axis = -GameManager.GM.transform.right;
                    break;
                case AXIS.neg_y:
                    axis = -GameManager.GM.transform.up;
                    break;
                case AXIS.neg_z:
                    axis = -GameManager.GM.transform.forward;
                    break;
            }
            transform.Rotate(axis, 100 * Time.deltaTime, Space.World);
            transform.GetChild(0).GetComponent<ParticleSystem>().enableEmission = true;
			GetComponent<Collider>().enabled = true;
            r.enabled = true;
            active = true;

            r.material.color = myColour;
		}
	}

    public void Activate() {
        GameManager.GM.CheckpointNum = myCheckpoint;
        Instantiate(particle, transform.position, transform.rotation);
        if (GameManager.GM.GetComponent<WorldRotation>() != null) {
            Vector3 axis = Vector3.up;
            switch (around) {
                case AXIS.pos_x:
                    axis = GameManager.GM.transform.right;
                    break;
                case AXIS.pos_y:
                    axis = GameManager.GM.transform.up;
                    break;
                case AXIS.pos_z:
                    axis = GameManager.GM.transform.forward;
                    break;
                case AXIS.neg_x:
                    axis = -GameManager.GM.transform.right;
                    break;
                case AXIS.neg_y:
                    axis = -GameManager.GM.transform.up;
                    break;
                case AXIS.neg_z:
                    axis = -GameManager.GM.transform.forward;
                    break;
            }
            GameManager.GM.GetComponent<WorldRotation>().startRotation(degrees, axis, rotationSpeed);
        }
    }
}
