using UnityEngine;
using System.Collections;

public class PlayerPressedE : MonoBehaviour {
	
	Transform level;
	public AudioSource Boom;
	
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
		if(Input.GetButtonDown("Use") || Input.GetMouseButtonDown(0))
		{
            Ray ray = Camera.main.ScreenPointToRay (new Vector3(Screen.width*0.5f, Screen.height*0.5f,0));
            RaycastHit hit;
            if (Physics.Raycast (ray, out hit, 5)) 
			{
                if (hit.transform.GetComponent<activationObj>())
                {
					GameManager.GM.checkpointNum = hit.transform.GetComponent<activationObj>().myCheckpoint;
					if(Boom!=null)
						Boom.Play();
					level.GetComponent<WorldRotation>().startRotation(hit.transform.GetComponent<activationObj>().around, hit.transform.GetComponent<activationObj>().degrees, hit.transform, hit.transform.GetComponent<activationObj>().worldColour);
                }
				else if (hit.transform.parent!= null){
					if(hit.transform.parent.GetComponent<LevelModelScript>())
                        UnityEngine.SceneManagement.SceneManager.LoadScene(hit.transform.parent.GetComponent<LevelModelScript>().levelNo);
				}
			}
		}
	}
}
