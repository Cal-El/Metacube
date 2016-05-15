using UnityEngine;
using System.Collections;

public class MyCubeMap : MonoBehaviour 
{

	// Use this for initialization
	void Start ()
	{

		//		Author
		//
		//		Textures are the work of Emil Persson, aka Humus. http://www.humus.name
		//				
		//		License
		//				
		//		This work is licensed under a Creative Commons Attribution 3.0 Unported License.
		//		http://creativecommons.org/licenses/by/3.0/

		Cubemap texture = (Cubemap)Resources.Load("MyCubeMap", typeof(Cubemap));
        Texture2D texture2 = (Texture2D)Resources.Load("oildrum_col", typeof(Texture));
        Texture2D texture3 = (Texture2D)Resources.Load("oildrum_nor", typeof(Texture));
        Texture2D texture4 = (Texture2D)Resources.Load("oildrum_gloss", typeof(Texture));

        GetComponent<Renderer>().material.SetTexture("_CubeMap", (Cubemap)texture);
        GetComponent<Renderer>().material.SetTexture("_ColourTex", texture2);
        GetComponent<Renderer>().material.SetTexture("_NormalMap", texture3);
        GetComponent<Renderer>().material.SetTexture("_GlossMap", texture4);

    }
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
}
