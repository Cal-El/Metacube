using UnityEngine;
using System.Collections;

public class EndPlaytestTrigger : MonoBehaviour {

    private BoxCollider bxCol;
    [SerializeField]
    GameObject TouchedLevelModel;
    bool activated = false;

    // Use this for initialization
    void Start () {
        bxCol = GetComponent<BoxCollider>();

    }
	
	// Update is called once per frame
	void Update () {
        if (!activated) {
            if (bxCol.bounds.Contains(Camera.main.transform.position)) {
                activated = true;
                MuseumManager.MM.LoadLevel("EndScene");
                if (TouchedLevelModel != null) {
                    GameObject go = Instantiate(TouchedLevelModel) as GameObject;
                    go.transform.parent = DataManager.DM.transform;
                }
            }
        }
	}
}
