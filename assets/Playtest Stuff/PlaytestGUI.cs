using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlaytestGUI : MonoBehaviour {

    [SerializeField]
    private Button startButton;
    private string currentInput;

	// Use this for initialization
	void Start () {
        startButton.interactable = false;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StringEdited(string s) {
        currentInput = s;
        if(s.Length > 0) {
            startButton.interactable = true;
        } else {
            startButton.interactable = false;
        }
    }

    public void StartPlaytest() {
        PlaytestData.SetUserID(currentInput);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
