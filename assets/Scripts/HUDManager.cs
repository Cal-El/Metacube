using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {

    const float TIMEBEFOREPROMPT = 3;

    public Material colourToMimic;
    public Image WASDPrompt;
    public Image MousePrompt;
    public Image ReloadCircle;

    private bool displayingHUD = true;
    private Color keysColVis;
    private Color keysColInvis;
    private Color mouseColVis;
    private Color mouseColInvis;
    private bool playerHasMoved = false;
    private float noMovementTimer = 0;
    private bool playerInRangeOfInteractable = false;
    private float withinAreaOfInteractable = 0;

    private GameObject player;

    // Use this for initialization
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");

        noMovementTimer = 0;
        keysColVis = colourToMimic.GetColor("_EmissionColor");
        keysColVis.a = 1;
        keysColInvis = colourToMimic.GetColor("_EmissionColor");
        keysColInvis.a = 0;
        WASDPrompt.color = Color.Lerp(keysColInvis, keysColVis, noMovementTimer - TIMEBEFOREPROMPT);

        MousePrompt.color = Color.Lerp(keysColInvis, keysColVis, withinAreaOfInteractable - TIMEBEFOREPROMPT);
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

            keysColVis = keysColInvis = colourToMimic.GetColor("_EmissionColor");
            if (keysColVis == Color.black) {
                keysColVis = keysColInvis = Color.white;
            }
            keysColVis.a = 1;
            keysColInvis.a = 0;

            if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && player.GetComponent<CharacterMotorC>().canControl) {
                playerHasMoved = true;
            }
            if (!playerHasMoved) {
                noMovementTimer = Mathf.Clamp(noMovementTimer + Time.deltaTime, 0, TIMEBEFOREPROMPT + 1);
                WASDPrompt.color = Color.Lerp(keysColInvis, keysColVis, noMovementTimer - TIMEBEFOREPROMPT);
            } else if (noMovementTimer > 0) {
                noMovementTimer = Mathf.Clamp(noMovementTimer - Time.deltaTime, 0, TIMEBEFOREPROMPT + 1);
                WASDPrompt.color = Color.Lerp(keysColInvis, keysColVis, noMovementTimer - TIMEBEFOREPROMPT);
            }

            bool nearToActiveInter = false;
            foreach(Interactable g in FindObjectsOfType<Interactable>()) {
                if (g.active && Vector3.Distance(g.transform.position, player.transform.position) < 5f) {
                    nearToActiveInter = true;
                    mouseColVis = mouseColInvis = g.myColour;
                    mouseColVis.a = 1;
                    mouseColInvis.a = 0;
                }
            }
            if(FindObjectOfType<CharacterMotorC>().canControl && nearToActiveInter) {
                withinAreaOfInteractable = Mathf.Clamp(withinAreaOfInteractable + Time.deltaTime, 0, TIMEBEFOREPROMPT + 1);
                MousePrompt.color = Color.Lerp(mouseColInvis, mouseColVis, withinAreaOfInteractable - TIMEBEFOREPROMPT);
            } else if(withinAreaOfInteractable > 0) {
                withinAreaOfInteractable = Mathf.Clamp(withinAreaOfInteractable - Time.deltaTime, 0, TIMEBEFOREPROMPT + 1);
                MousePrompt.color = Color.Lerp(mouseColInvis, mouseColVis, withinAreaOfInteractable - TIMEBEFOREPROMPT);
            }

            if (GameManager.GM != null)
                ReloadCircle.fillAmount = GameManager.GM.reloadTimer / GameManager.TIMETORELOAD;
            else
                ReloadCircle.fillAmount = 0;


            if (!displayingHUD) {
                foreach (Image i in GetComponentsInChildren<Image>()) {
                    i.enabled = true;
                }
                displayingHUD = true;
            }
        }
    }
}
