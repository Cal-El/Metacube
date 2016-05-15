using UnityEngine;
using System.Collections;

public class Blindsight : MonoBehaviour {

    public Color thisColour = Color.white;
    public float speed;
    public Transform player;
    public float lineFalloff;
    private float lineDist;
 
    void Start() {
        foreach (Material m in GetComponent<Renderer>().materials) {
            m.SetVector("_OriginPoint", player.position);
        }
        lineDist = 0;
    }

	// Update is called once per frame
	void Update () {
        lineDist += Time.deltaTime*speed;
        foreach (Material m in GetComponent<Renderer>().materials) {
            
                GetComponent<Renderer>().material.SetVector("_OriginPoint", player.position);
                lineDist = 0;
            
            GetComponent<Renderer>().material.SetFloat("_LineDistance", lineDist);
            GetComponent<Renderer>().material.SetFloat("_LineThickness", lineFalloff);
            GetComponent<Renderer>().material.SetColor("_ThisColour", thisColour);
        }
    }
}
