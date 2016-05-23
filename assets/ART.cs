using UnityEngine;
using System.Collections;

public class ART : MonoBehaviour {

    public string unlockKey;
    private Renderer r;

	// Use this for initialization
	void Start () {
        if(unlockKey.Length == 0)
        {
            unlockKey = "Level"+name;
        }
        r = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (PlayerPrefs.HasKey(unlockKey))
            if (PlayerPrefs.GetInt(unlockKey) > 0)
                r.enabled = true;
            else
                r.enabled = false;
        else
            r.enabled = false;
    }
}
