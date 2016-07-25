using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour {

    [Range(1,4)]
    public int ChapterID = 1;
    [Range(1, 4)]
    public int LevelID = 1;
    private bool firstDoor = false;
    private bool playerHasMoved = false;
    private enum STATES { closed, opening, open };
    private STATES doorState = STATES.closed;
    private const float TIME_TO_OPEN = 5f;
    private float timer = 0;
    private Vector3 openPosition;
    private Vector3 closedPosition;
    private const float DOOR_HEIGHT = 8;
    private AudioSource ads;
    public AudioClip grinding;
    public GameObject boomStop;

	// Use this for initialization
	void Start () {
        closedPosition = transform.position;
        openPosition = transform.position + Vector3.up * DOOR_HEIGHT;

        if (DataManager.GetBool("Level " + ChapterID + "-" + LevelID + " Finished")) {
            doorState = STATES.open;
        } else {
            if (ChapterID == 1 && LevelID == 1) {
                firstDoor = true;
                doorState = STATES.opening;
            } else {
                int preChapterID = ChapterID;
                int preLevelID = LevelID-1;
                if (preLevelID == 0) {
                    preLevelID = 4;
                    preChapterID--;
                }

                if(DataManager.GetBool("Level " + preChapterID + "-" + preLevelID + " Finished")) {
                    doorState = STATES.opening;
                } else {
                    doorState = STATES.closed;
                }
            }
        }
        if (!firstDoor) timer = -0.5f;
        if (doorState == STATES.open) transform.position = openPosition;

        ads = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (doorState == STATES.opening) {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                playerHasMoved = true;
            if (playerHasMoved) {
                if (timer <= 0) {
                    ads.clip = grinding;
                    ads.Play();
                }
                ads.volume = AudioManager.GetVolume(AudioManager.AUDIO_TYPES.Sound);
                timer += Time.deltaTime;
                transform.position = Vector3.Lerp(closedPosition, openPosition, timer / TIME_TO_OPEN);
                if (timer >= TIME_TO_OPEN) {
                    doorState = STATES.open;
                    Instantiate(boomStop, transform.position, transform.rotation);
                    ads.Stop();
                }
            }
        }
	}
}
