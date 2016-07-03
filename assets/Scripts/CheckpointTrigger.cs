using UnityEngine;
using System.Collections;

public class CheckpointTrigger : MonoBehaviour {

    public int activeInProgress = 0;
    public int myCheckpoint = 0;
    public bool addToProgress = false;

    void OnTriggerEnter(Collider c) {
        if(GameManager.GM.progression == activeInProgress) {
            if(c.tag == "Player") {
                GameManager.GM.checkpointNum = myCheckpoint;
                if (addToProgress) {
                    GameManager.GM.progression++;
                }
                Destroy(this.gameObject);
            }
        }
    }
}
