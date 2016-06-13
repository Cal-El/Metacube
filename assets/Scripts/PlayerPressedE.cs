using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerPressedE : MonoBehaviour {
	
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
		if(Input.GetButtonDown("Use") || Input.GetMouseButtonDown(0))
		{
            Ray ray = Camera.main.ScreenPointToRay (new Vector3(Screen.width*0.5f, Screen.height*0.5f,0));
            RaycastHit[] hits = Physics.RaycastAll(ray, 5);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction*5);
            if (hits.Length > 0) 
			{
                foreach (RaycastHit hit in hits) {
                    if (hit.transform.GetComponent<activationObj>()) {
                        GameManager.GM.checkpointNum = hit.transform.GetComponent<activationObj>().myCheckpoint;
                        if (Boom != null)
                            Instantiate(Boom);
                        Instantiate(hit.transform.GetComponent<activationObj>().particle, hit.transform.position, hit.transform.rotation);
                        level.GetComponent<WorldRotation>().startRotation(hit.transform.GetComponent<activationObj>().around, hit.transform.GetComponent<activationObj>().degrees, hit.transform, hit.transform.GetComponent<activationObj>().worldColour);
                    } else if (hit.transform.GetComponent<collectable>()) {
                        hit.transform.GetComponent<collectable>().CollectedMe();
                    } else if (hit.transform.parent != null) {
                        if (hit.transform.parent.GetComponent<LevelModelScript>()) {
                            DataManager.playerPos = transform.parent.position;
                            DataManager.playerRot = transform.parent.rotation;
                            SceneManager.LoadScene(hit.transform.parent.GetComponent<LevelModelScript>().levelNo);
                        }
                    } else if (transform.tag == "Player") {
                        //Do nothing
                    } else {
                        //Hit something that is not the player, activation stone, museum element, or collectible
                        break;
                    }
                }
			}
		}
	}
}
