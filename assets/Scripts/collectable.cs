using UnityEngine;
using System.Collections;

public class collectable : MonoBehaviour {

    public int availableInStage = 0;
    public string collectableName;
    public GameObject particleEffect;
    private Renderer r;
    private BoxCollider bc;
    private ParticleSystem ps;
    private bool collected = false;



	// Use this for initialization
	void Start () {
        r = GetComponent<Renderer>();
        bc = GetComponent<BoxCollider>();
        ps = GetComponent<ParticleSystem>();
        if (DataManager.GetBool("Art " + collectableName)) {
            collected = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
	    if(GameManager.GM.progression != availableInStage || collected)
        {
            r.enabled = false;
            bc.enabled = false;
            ps.enableEmission = false;
        }
        else
        {
            r.enabled = true;
            bc.enabled = true;
            ps.enableEmission = true;

            transform.Rotate(Vector3.one * 10 *Time.deltaTime);
        }
	}

    public void CollectedMe()
    {
        Instantiate(particleEffect, transform.position, transform.rotation);
        if (collectableName.Length > 0)
        {
            DataManager.SetBool("Art " + collectableName, true);
        }
        collected = true;
    }
}
