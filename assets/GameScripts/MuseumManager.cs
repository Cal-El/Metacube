using UnityEngine;
using System.Collections;

public class MuseumManager : MonoBehaviour {

	public static MuseumManager MM;
	public Transform player;

	public GameObject[] cornerPoints;

	// Use this for initialization
	void Awake () {
		MM = this;
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < cornerPoints.Length; i++) {
			float angle = Vector2.Angle(cornerPoints[i].transform.up, player.position-cornerPoints[i].transform.position);
			Debug.Log(i+", "+angle);
		}
        if(Input.GetKeyDown(KeyCode.Escape) && FindObjectOfType<OptionsMenu>() == null) {

        }
	}

	/// <summary>
	/// Finds the angle .
	/// </summary>
	/// <returns>The angle.</returns>
	/// <param name="point">Point.</param>
	float FindAngle(Transform point){
		if (Vector2.Angle (point.up, player.position - point.position) < 45) {

		}
		return 0;
	}
}
