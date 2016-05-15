using UnityEngine;
using System.Collections;

public class BoxLifting : MonoBehaviour {
		
	bool holdingBox;
	Transform heldBox;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		/*if(holdingBox){
			Ray ray = new Ray(transform.position,transform.forward);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 5, 8)) {

			}
		}*/
		if(Input.GetButtonDown("Use") && !holdingBox) {
			PickupBox();
		}
		else if(Input.GetButtonDown("Use") && holdingBox) {
			ReleaseBox();
		}
	}
	
	void PickupBox() {
		Ray ray = new Ray(transform.position,transform.forward);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 5)) {
			if(hit.transform.tag == "PhysicsObj") {
				hit.transform.position = transform.position + transform.forward*3;
				hit.transform.parent = transform;
				heldBox = hit.transform;
				heldBox.GetComponent<Collider>().isTrigger = true;
				holdingBox = true;
			}
		}
	}
	
	void ReleaseBox() {
		if(heldBox != null){
			GameObject Level = GameObject.Find("Level");
			heldBox.parent = Level.transform;
			heldBox.GetComponent<Collider>().isTrigger = false;
			heldBox = null;
			holdingBox = false;
		}
	}
}
