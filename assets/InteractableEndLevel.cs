using UnityEngine;
using System.Collections;
[RequireComponent(typeof(EndLevelScript))]
public class InteractableEndLevel : Interactable {

    // Use this for initialization
    void Start () {
        	
	}
	
	// Update is called once per frame
	void Update () {
        active = (GetComponent<EndLevelScript>().showUpProgress == GameManager.GM.progression);

	}
}
