using UnityEngine;
using System.Collections;

public class MenuCamera : MonoBehaviour {

	Quaternion startRot;

	Vector3 targetRot;
	public Transform target;

	public Vector3[] position = new Vector3[]{Vector3.zero, Vector3.zero, new Vector3(-10,0,0)};
	public Vector3[] rotation = new Vector3[]{Vector3.zero, new Vector3(0,180,0),new Vector3(0,180,0)};
	
	public float xRotRange = 30;
	public float yRotRange = 30;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		targetRot = rotation[MenuManager.MM.menuPosition]
			+Vector3.up*((Input.mousePosition.x - Screen.width*0.5f)/Screen.width *xRotRange)
			+Vector3.left*((Input.mousePosition.y - Screen.height*0.5f)/Screen.height *yRotRange);
		transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation,Quaternion.Euler(targetRot),Time.deltaTime);

		transform.parent.position = Vector3.Lerp(transform.parent.position, position[MenuManager.MM.menuPosition], Time.deltaTime);
	
		RaycastHit hit = new RaycastHit();
		if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 9999)){
			if(hit.transform.name.StartsWith("Butt_")){
				MenuManager.MM.mouseOverButt = hit.transform.gameObject;
			}else{
				MenuManager.MM.mouseOverButt = null;
			}
		}else{
			MenuManager.MM.mouseOverButt = null;
		}
	}
}
