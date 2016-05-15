using UnityEngine;
using System.Collections;

public class PortfolioRelease : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Cursor.visible = true;
		Cursor.lockState  = CursorLockMode.None;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI(){
		GUI.Box (new Rect (20, 20, Screen.width - 40, Screen.height - 40), "" +
						"\nThis is a prototype of a project that's currently on the backburner. " +
						"\nAll Programming and Level Design is by myself. " +
						"\n" +
						"\nThe game is a first person exploritive platformer, focusing on world rotation" +
						"\nControls:" +
						"\n WASD - Move" +
						"\n E - Use/Interact" +
						"\n Sapce - Jump" +
						"\n" +
						"\n \"New Game\" will take you to the unfinished level 1" +
						"\n \"Continue\" will take you to the (mostly) completed level 2");
		if (GUI.Button (new Rect (Screen.width * 0.25f, Screen.height - 140, Screen.width * 0.5f, 80), "Continue")) {
			Application.LoadLevel(Application.loadedLevel +1);
		}
	}
}
