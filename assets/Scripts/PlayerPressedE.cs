using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerPressedE : MonoBehaviour {
	
	Transform level;
	public GameObject TouchedWorldCore;
    public GameObject TouchedCollectable;
    public GameObject TouchedLevelModel;
    public GameObject TouchedEndLevel;
	
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
		if(Input.GetButtonDown("Use")&&Time.timeScale > 0)
		{
            Ray ray = Camera.main.ScreenPointToRay (new Vector3(Screen.width*0.5f, Screen.height*0.5f,0));
            RaycastHit[] hits = Physics.RaycastAll(ray, 5);
            Debug.DrawLine(ray.origin, ray.origin + ray.direction*5);
            if (hits.Length > 0) 
			{
                foreach (RaycastHit hit in hits) {
                    if (hit.transform.GetComponent<InteractableEndLevel>()) {
                        GameManager.GM.finishedLevel = true;
                        Spawn2DSound(TouchedEndLevel);


                    } else if (hit.transform.GetComponent<activationObj>()) {
                        GameManager.GM.CheckpointNum = hit.transform.GetComponent<activationObj>().myCheckpoint;
                        Spawn2DSound(TouchedWorldCore);
                        Instantiate(hit.transform.GetComponent<activationObj>().particle, hit.transform.position, hit.transform.rotation);
                        if (hit.transform.parent.GetComponent<WorldRotation>() != null) {
                            hit.transform.parent.GetComponent<WorldRotation>().startRotation(hit.transform.GetComponent<activationObj>().around, hit.transform.GetComponent<activationObj>().degrees, hit.transform, hit.transform.GetComponent<activationObj>().myColour);
                        } else {
                            level.GetComponent<WorldRotation>().startRotation(hit.transform.GetComponent<activationObj>().around, hit.transform.GetComponent<activationObj>().degrees, hit.transform, hit.transform.GetComponent<activationObj>().myColour);
                        }


                    } else if (hit.transform.GetComponent<collectable>()) {
                        hit.transform.GetComponent<collectable>().CollectedMe();
                        Spawn2DSound(TouchedCollectable);


                    } else if (hit.transform.parent != null) {
                        if (hit.transform.parent.GetComponent<LevelModelScript>()) {
                            MuseumManager.MM.LoadLevel(hit.transform.parent.GetComponent<LevelModelScript>().levelNo);
                            Spawn2DSound(TouchedLevelModel);
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

    public static void Spawn2DSound(GameObject g) {
        if (g != null) {
            GameObject go = Instantiate(g) as GameObject;
            go.transform.parent = DataManager.DM.transform;
        }
    }
}
