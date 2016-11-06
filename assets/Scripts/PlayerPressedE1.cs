using UnityEngine;
using System.Collections;

public class PlayerPressedE1 : MonoBehaviour {
	
	Transform level;
	public GameObject Boom;
	
	// Use this for initialization
	void Start () {
		if (GameManager.GM != null)
						level = GameManager.GM.transform;
				else if (MuseumManager.MM != null)
						level = MuseumManager.MM.transform;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButtonDown("Use"))
		{
            Ray ray = Camera.main.ScreenPointToRay (new Vector3(Screen.width*0.5f, Screen.height*0.5f,0));
            RaycastHit[] hits = Physics.RaycastAll(ray, 5);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction*5);
            if (hits.Length > 0) 
			{
                foreach (RaycastHit hit in hits) {
                    if (!hit.transform.GetComponent<activationObj>() && !hit.transform.GetComponent<collectable>() && !(transform.tag == "Player")) {
                        float degrees = Vector3.Angle(hit.normal, Vector3.up);
                        if (degrees == 180) {
                            //level.GetComponent<WorldRotation>().startRotation(transform.right, degrees, null, Color.yellow);
                        } else {
                            Vector3 around = Vector3.Cross(hit.normal, Vector3.up);
                            //level.GetComponent<WorldRotation>().startRotation(around, degrees, null, Color.yellow);
                        }
                        break;
                    }
                }
			}
		}
	}
}
