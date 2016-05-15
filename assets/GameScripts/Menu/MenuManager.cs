using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public static MenuManager MM;

	GameObject[] buttons;
	public GameObject mouseOverButt;

	public int menuPosition = 0;

	// Use this for initialization
	void Awake () {
		MM = this;
		buttons = GameObject.FindGameObjectsWithTag("3dButton");
		Cursor.lockState = CursorLockMode.None;
	}
	
	// Update is called once per frame
	void Update () {
		//Make only the moused over button highlight
		for(int i = 0; i<buttons.Length; i++){
			if(buttons[i] != mouseOverButt){
				buttons[i].GetComponent<Renderer>().material.color = Color.grey;
			}else{
				buttons[i].GetComponent<Renderer>().material.color = Color.white;
			}//end if
		}//end for loop

		if(Input.GetKeyDown(KeyCode.Space))
			menuPosition = 1;
		if(Input.GetKeyDown(KeyCode.Escape))
			menuPosition = 0;
		if(Input.GetMouseButtonDown(0) && mouseOverButt!= null){
			switch (mouseOverButt.name){
			case "Butt_NewGame":
				//Load level 1, checkpoint 1
				Application.LoadLevel(Application.loadedLevel +1);
				break;
			case "Butt_Continue":
				//Find a list of saves
				//Find the most recent in the list
				//Load that save file
				Application.LoadLevel(Application.loadedLevel +2);
				break;
			case "Butt_Load":
				//Find all saves
				//Display from most recent
				//If the player clicks one, load that save file
				break;
			case "Butt_Quit":
				//Ask if player wants to quit
				Application.Quit();
				break;
			case "Butt_Options":
				//Display options menu
				break;
			case "Butt_Extras":
				//Display the extras menu
				break;
			}
		}
	}//end update
}
