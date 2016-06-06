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
        if (unlockKey.EndsWith("5"))
        {
            string chapter = unlockKey.TrimEnd('5');
            if (PlayerPrefs.HasKey(chapter + "1") && PlayerPrefs.HasKey(chapter + "2") && PlayerPrefs.HasKey(chapter + "3") && PlayerPrefs.HasKey(chapter + "4"))
                if (PlayerPrefs.GetInt(chapter + "1") > 0 && PlayerPrefs.GetInt(chapter + "2") > 0 && PlayerPrefs.GetInt(chapter + "3") > 0 && PlayerPrefs.GetInt(chapter + "4") > 0)
                    r.enabled = true;
                else
                    r.enabled = false;
            else
                r.enabled = false;
        }
        else
        {
            if (PlayerPrefs.HasKey(unlockKey))
                if (PlayerPrefs.GetInt(unlockKey) > 0)
                    r.enabled = true;
                else
                    r.enabled = false;
            else
                r.enabled = false;
        }
    }
}
