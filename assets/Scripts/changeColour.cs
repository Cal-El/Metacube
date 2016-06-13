using UnityEngine;
using System.Collections;

public class changeColour : MonoBehaviour {
	
	//public Material[] materialArray = new Material[7];
	public Material changingMat;
	public Color colour;
    private Color currColour;
	public float speed = 1;
	public float pulseSpeed = 0.5f;
	public bool acceptNewColor = true;
	
	
	void Start ()
	{
        if (changingMat != null) 
            changingMat.SetColor("_EmissionColor", colour);
        currColour = colour;

    }
	// Update is called once per frame
	void Update ()
	{
		currColour = Color.Lerp(currColour, colour, speed*0.01f);
        if (changingMat != null) {
            changingMat.SetColor("_EmissionColor", currColour);
            //transform.renderer.material.mainTexture
            changingMat.mainTextureOffset = new Vector2(0, (changingMat.mainTextureOffset.y + (0.01f * pulseSpeed)) % 1);
        }
	}
	
	public void SetColour (Color newColor)
	{
		if(acceptNewColor)
			colour = newColor;
	}
}
