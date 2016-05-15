using UnityEngine;
using System.Collections;

public class trefoilScript : MonoBehaviour {
	
	//public Material[] materialArray = new Material[7];
	public Material changingMat;
	public Color colour;
	public bool acceptNewColor = true;
	
	
	void Start ()
	{
		SetRandColour();
	}
	// Update is called once per frame
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.B) && Input.GetKey(KeyCode.Q))
			SetRandColour();
		//transform.renderer.material.mainTexture
		changingMat.mainTextureOffset = new Vector2(0, (changingMat.mainTextureOffset.y+(Time.deltaTime*0.5f))%1);
	}
	void SetRandColour(){
		int C = Random.Range(1,8);
		switch (C){
		default: 
			colour = new Color(0,0.6f,1,1); 
			break;
		case 2: 
			colour = new Color(0,1,1,1);
			break;
		case 3: 
			colour = new Color(0.5f,1,0.5f,1);
			break;
		case 4: 
			colour = new Color(1,0.95f,0.95f,1);
			break;
		case 5:
			colour = Color.red;
			break;
		case 7:
			colour = new Color(1,0.55f,0,1);
			break;
		}
		changingMat.color = colour;
	}
}
