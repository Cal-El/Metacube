using UnityEngine;
using System.Collections;

public class MuseumManager : MonoBehaviour {

	public static MuseumManager MM;
	public Transform player;
    public GameObject optionsMenu;

	// Use this for initialization
	void Awake () {
		MM = this;
        if(!PlayerPrefs.HasKey("Prefs Set") || PlayerPrefs.GetString("Prefs Set") == "False") {
            SetDefaultPrefs();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Escape) && FindObjectOfType<OptionsMenu>() == null) {
            Instantiate(optionsMenu);
        }
	}

    void SetDefaultPrefs() {
        PlayerPrefs.SetString("Prefs Set", "True");
        PlayerPrefs.SetFloat("FoV", 60.0f);
        PlayerPrefs.SetString("FoVShift On", "True");
        PlayerPrefs.SetFloat("Sensitivity", 5.0f);

        PlayerPrefs.Save();
    }
}
