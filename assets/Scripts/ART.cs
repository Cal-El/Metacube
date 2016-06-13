using UnityEngine;
using System.Collections;

public class ART : MonoBehaviour {

    public string unlockKey;
    private Renderer r;
    
	// Use this for initialization
	void Start () {
        if(unlockKey.Length == 0)
        {
            unlockKey = "Art "+name;
        }
        r = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (unlockKey.EndsWith("5"))
        {
            string chapter = unlockKey.TrimEnd('5');
            if (DataManager.GetBool(chapter + "1") && DataManager.GetBool(chapter + "2") && DataManager.GetBool(chapter + "3") && DataManager.GetBool(chapter + "4"))
                r.enabled = true;
            else
                r.enabled = false;
        }
        else
        {
            if (DataManager.GetBool(unlockKey))
                r.enabled = true;
            else
                r.enabled = false;
        }
    }
}
