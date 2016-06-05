using UnityEngine;
using System.Collections;

public class MuseumManager : MonoBehaviour {

	public static MuseumManager MM;
	public Transform player;
    public GameObject optionsMenu;

	// Use this for initialization
	void Awake () {
		MM = this;
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Escape) && FindObjectOfType<OptionsMenu>() == null) {
            Instantiate(optionsMenu);
        }
	}
}
