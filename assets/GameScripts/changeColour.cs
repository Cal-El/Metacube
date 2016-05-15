using UnityEngine;
using System.Collections;

public class changeColour : MonoBehaviour {
	
	//public Material[] materialArray = new Material[7];
	public Material changingMat;
	public Color colour;
	public float speed = 1;
	public float pulseSpeed = 0.5f;
	public bool acceptNewColor = true;
	
	
	void Start ()
	{
		changingMat.color = colour;
	}
	// Update is called once per frame
	void Update ()
	{
		changingMat.color = Color.Lerp(changingMat.color, colour, speed*0.01f);
        //transform.renderer.material.mainTexture
        changingMat.mainTextureOffset = new Vector2(0, (changingMat.mainTextureOffset.y+(Time.deltaTime*pulseSpeed))%1);
        
	}
	
	public void SetColour (Color newColor)
	{
		if(acceptNewColor)
			colour = newColor;
	}
}
