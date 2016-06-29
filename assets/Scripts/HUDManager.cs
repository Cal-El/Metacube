using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {

    const float TIMEBEFOREPROMPT = 3;

    public Material colourToMimic;
    public Image WASDPrompt;
    public Image MousePrompt;

    private bool displayingHUD = true;
    private Color colVis;
    private Color colInvis;
    private bool playerHasMoved = false;
    private float noMovementTimer = 0;
    private bool playerInRangeOfInteractable = false;
    private float withinAreaOfInteractable = 0;

    private GameObject player;

    // Use this for initialization
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");

        noMovementTimer = 0;
        colVis = colourToMimic.GetColor("_EmissionColor");
        colVis.a = 1;
        colInvis = colourToMimic.GetColor("_EmissionColor");
        colInvis.a = 0;
        WASDPrompt.color = Color.Lerp(colInvis, colVis, noMovementTimer - TIMEBEFOREPROMPT);

        MousePrompt.color = Color.Lerp(colInvis, colVis, withinAreaOfInteractable - TIMEBEFOREPROMPT);
    }

    // Update is called once per frame
    void Update() {
        if (FindObjectOfType<DollyCam>() != null || FindObjectOfType<PausedMenu>() != null) {
            if (displayingHUD) {
                foreach (Image i in GetComponentsInChildren<Image>()) {
                    i.enabled = false;
                }
                displayingHUD = false;
            }
        } else {

            colVis = colInvis = colourToMimic.GetColor("_EmissionColor");
            if (colVis == Color.black) {
                colVis = colInvis = Color.white;
            }
            colVis.a = 1;
            colInvis.a = 0;

            if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && player.GetComponent<CharacterMotorC>().canControl) {
                playerHasMoved = true;
            }
            if (!playerHasMoved) {
                noMovementTimer = Mathf.Clamp(noMovementTimer + Time.deltaTime, 0, TIMEBEFOREPROMPT + 1);
                WASDPrompt.color = Color.Lerp(colInvis, colVis, noMovementTimer - TIMEBEFOREPROMPT);
            } else if (noMovementTimer > 0) {
                noMovementTimer = Mathf.Clamp(noMovementTimer - Time.deltaTime, 0, TIMEBEFOREPROMPT + 1);
                WASDPrompt.color = Color.Lerp(colInvis, colVis, noMovementTimer - TIMEBEFOREPROMPT);
            }

            bool nearToActiveInter = false;
            foreach(Interactable g in FindObjectsOfType<Interactable>()) {
                if (g.active && Vector3.Distance(g.transform.position, player.transform.position) < 5f) {
                    nearToActiveInter = true;
                }
            }
            if(FindObjectOfType<CharacterMotorC>().canControl && nearToActiveInter) {
                withinAreaOfInteractable = Mathf.Clamp(withinAreaOfInteractable + Time.deltaTime, 0, TIMEBEFOREPROMPT + 1);
                MousePrompt.color = Color.Lerp(colInvis, colVis, withinAreaOfInteractable - TIMEBEFOREPROMPT);
            } else if(withinAreaOfInteractable > 0) {
                withinAreaOfInteractable = Mathf.Clamp(withinAreaOfInteractable - Time.deltaTime, 0, TIMEBEFOREPROMPT + 1);
                MousePrompt.color = Color.Lerp(colInvis, colVis, withinAreaOfInteractable - TIMEBEFOREPROMPT);
            }


            if (!displayingHUD) {
                foreach (Image i in GetComponentsInChildren<Image>()) {
                    i.enabled = true;
                }
                displayingHUD = true;
            }
        }
    }
}
