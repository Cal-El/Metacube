using UnityEngine;
using System.Collections;

public class TutorialTrigger : MonoBehaviour {

    public enum BUTTONPROMPTTYPE { Space, Shift }
    public BUTTONPROMPTTYPE type = BUTTONPROMPTTYPE.Space;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider c) {
        if(c.tag == "Player") {
            if (type == BUTTONPROMPTTYPE.Space) {
                FindObjectOfType<HUDManager>().DisplaySpacePrompt();
            } else {
                FindObjectOfType<HUDManager>().DisplayShiftPrompt();
            }
            Destroy(this.gameObject);
        }
    }
}
