using UnityEngine;
using System.Collections;

public class collectable : MonoBehaviour {

    public int availableInStage = 0;
    public string collectableName;
    public GameObject particleEffect;
    private Renderer r;
    private BoxCollider bc;
    private ParticleSystem ps;



	// Use this for initialization
	void Start () {
        r = GetComponent<Renderer>();
        bc = GetComponent<BoxCollider>();
        ps = GetComponent<ParticleSystem>();
        if (PlayerPrefs.HasKey(collectableName))
        {
            if(PlayerPrefs.GetInt(collectableName) > 0)
            {
                CollectedMe();
            }
        }
        else
        {
            PlayerPrefs.SetInt(collectableName, 0);
        }
	}
	
	// Update is called once per frame
	void Update () {
	    if(GameManager.GM.progression != availableInStage)
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
            PlayerPrefs.SetInt(collectableName, 1);
        }
        Destroy(this.gameObject);
    }
}
