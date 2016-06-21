using UnityEngine;
using System.Collections;

public class AtomScript : MonoBehaviour {

    public Transform[] children;

	// Use this for initialization
	void Start () {
        children = GetComponentsInChildren<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < children.Length; i++) {
            int sign = 1;
            if(i%2 > 0) {
                sign = -1;
            }
            children[i].Rotate(0,0,Time.deltaTime*10*Mathf.Pow(i,2)* sign);
        }
	}
}
