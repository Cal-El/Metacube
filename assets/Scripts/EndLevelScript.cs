using UnityEngine;
using System.Collections;

public class EndLevelScript : MonoBehaviour {

	public int showUpProgress;
    private Collider bc;
    private Renderer r;
    
    public static Transform endTrans;
    public bool interactable = false;

    private float scale = 1;
	// Use this for initialization
	void Start () {
        endTrans = transform;
        scale = transform.localScale.x;
        bc = GetComponent<Collider>();
        bc.isTrigger = true;
        r = GetComponent<MeshRenderer>();
	}

    // Update is called once per frame
    void Update() {
        if (GameManager.GM.progression >= showUpProgress) {
            GetComponent<ParticleSystem>().enableEmission = true;
            r.enabled = true;
            bc.enabled = true;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one*(scale+Mathf.Sin(Time.timeSinceLevelLoad)/10*scale), Time.deltaTime);
            Component halo = GetComponent("Halo"); halo.GetType().GetProperty("enabled").SetValue(halo, true, null);
            if (bc.bounds.Contains(Camera.main.transform.position))
                GameManager.GM.finishedLevel = true;
        } else {
            GetComponent<ParticleSystem>().enableEmission = false;
            r.enabled = false;
            bc.enabled = false;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime);
            Component halo = GetComponent("Halo"); halo.GetType().GetProperty("enabled").SetValue(halo, false, null);
        }
    }
}
