using UnityEngine;
using System.Collections;

public class MuseumManager : MonoBehaviour {

	public static MuseumManager MM;
	public Transform player;
    public GameObject optionsMenu;

	// Use this for initialization
	void Awake () {
		MM = this;
        if(DataManager.playerPos != Vector3.zero) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = DataManager.playerPos;
            player.transform.rotation = DataManager.playerRot;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Escape) && FindObjectOfType<PausedMenu>() == null) {
            Instantiate(optionsMenu);
        }
	}
}
