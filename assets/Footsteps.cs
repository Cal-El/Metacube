using UnityEngine;
using System.Collections;

public class Footsteps : MonoBehaviour {

    private CharacterMotorC cmc;
    [SerializeField]
    private GameObject footstepPrefab;
    [SerializeField]
    private GameObject landedPrefab;

    private const float DISTANCE_PER_FOOTSTEP = 3;
    private float distanceCounter = 0;
    private bool wasGrounded = true;


	// Use this for initialization
	void Start () {
        cmc = GetComponent<CharacterMotorC>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (cmc.canControl) {
            distanceCounter += cmc.movement.distanceMoved;
            if (distanceCounter >= DISTANCE_PER_FOOTSTEP) {
                SpawnFootstep(footstepPrefab);
            }
            if (!wasGrounded && cmc.grounded) {
                SpawnFootstep(landedPrefab);
            }
            wasGrounded = cmc.grounded;
        }
    }

    void SpawnFootstep(GameObject prefab) {
        distanceCounter = 0;
        Spawn2DSound(prefab);
    }

    public static void Spawn2DSound(GameObject g) {
        if (g != null) {
            GameObject go = Instantiate(g) as GameObject;
            go.transform.parent = DataManager.DM.transform;
        }
    }
}
