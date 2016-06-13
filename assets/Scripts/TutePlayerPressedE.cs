using UnityEngine;
using System.Collections;

public class TutePlayerPressedE : MonoBehaviour {
	
	public Transform level;
	
	// Use this for initialization
	void Start () {
	
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButtonDown("Use"))
		{
            Ray ray = Camera.main.ScreenPointToRay (new Vector3(Screen.width*0.5f, Screen.height*0.5f,0));
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit, 5)) 
			{
                Debug.DrawLine (ray.origin, hit.point);

                if (hit.transform.GetComponent<tuteActivationObj>())
                {
					hit.transform.parent.GetComponent<TuteRotation>().startRotation(hit.transform.GetComponent<tuteActivationObj>().around, hit.transform);
                }
			}
		}
	}
}
