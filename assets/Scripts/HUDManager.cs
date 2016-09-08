﻿using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {

    const float TIMEBEFOREPROMPT = 3;
    
    public Material colourToMimic;
	public Image crosshair;
    public Image WASDPrompt;
    public Image MousePrompt;
    public Image ReloadCircle;
    public Text TimerText;
    public Image ProgressTimer;
    public Text PreviousTimer;

    private CharacterMotorC playerMotor;
    private Interactable[] interactables;
    private int indexingValue = 0;
    private bool closeToInteractable = false;
    private bool displayingHUD = true;
    private Color keysColVis;
    private Color keysColInvis;
    private Color mouseColVis;
    private Color mouseColInvis;
    private bool playerHasMoved = false;
    private float noMovementTimer = 0;
    private bool playerInRangeOfInteractable = false;
    private float withinAreaOfInteractable = 0;

    private bool showTimer = false;
    private float previousTime;

    private GameObject player;

    // Use this for initialization
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMotor = FindObjectOfType<CharacterMotorC>();
        interactables = FindObjectsOfType<Interactable>();

        noMovementTimer = 0;
        keysColVis = colourToMimic.GetColor("_EmissionColor");
        keysColVis.a = 1;
        keysColInvis = colourToMimic.GetColor("_EmissionColor");
        keysColInvis.a = 0;
        WASDPrompt.color = Color.Lerp(keysColInvis, keysColVis, noMovementTimer - TIMEBEFOREPROMPT);

        MousePrompt.color = Color.Lerp(keysColInvis, keysColVis, withinAreaOfInteractable - TIMEBEFOREPROMPT);

        if (GameManager.GM != null) {
            showTimer = DataManager.GetBool("Level " + GameManager.GM.LevelID + " Finished");
            previousTime = DataManager.GetFloat("Level " + GameManager.GM.LevelID + " Finished Timer");
            TimeSpan t = TimeSpan.FromSeconds(previousTime);
            PreviousTimer.text = string.Format("Best: {0:D2}:{1:D2}.{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);
        }
        TimerText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if (!playerMotor.canControl) {
            if (displayingHUD) {
				crosshair.enabled = false;
				ReloadCircle.enabled = false;
				WASDPrompt.enabled = false;
				MousePrompt.enabled = false;

                displayingHUD = false;
            }
        } else {
            

            keysColVis = keysColInvis = colourToMimic.GetColor("_EmissionColor");
            if (keysColVis == Color.black) {
                keysColVis = keysColInvis = Color.white;
            }
            keysColVis.a = 1;
            keysColInvis.a = 0;

            if (!playerHasMoved) {
                if (playerMotor.PlayerPressingDirections()) {
                    playerHasMoved = true;
                }
                noMovementTimer = Mathf.Clamp(noMovementTimer + Time.deltaTime, 0, TIMEBEFOREPROMPT + 1);
                WASDPrompt.color = Color.Lerp(keysColInvis, keysColVis, noMovementTimer - TIMEBEFOREPROMPT);
            } else if (noMovementTimer > 0) {
                noMovementTimer = Mathf.Clamp(noMovementTimer - Time.deltaTime, 0, TIMEBEFOREPROMPT + 1);
                WASDPrompt.color = Color.Lerp(keysColInvis, keysColVis, noMovementTimer - TIMEBEFOREPROMPT);
            }

            Interactable g = interactables[indexingValue];
            if (g.active && Vector3.Distance(g.transform.position, player.transform.position) < 5f) {
                if (!closeToInteractable) {
                    mouseColVis = mouseColInvis = g.myColour;
                    mouseColVis.a = 1;
                    mouseColInvis.a = 0;
                }
                closeToInteractable = true;
            } else {
                closeToInteractable = false;
            }
            if (!closeToInteractable) {
                indexingValue++;
                if (indexingValue >= interactables.Length) indexingValue = 0;
            }

            if(closeToInteractable) {
                withinAreaOfInteractable = Mathf.Clamp(withinAreaOfInteractable + Time.deltaTime, 0, TIMEBEFOREPROMPT + 1);
                MousePrompt.color = Color.Lerp(mouseColInvis, mouseColVis, withinAreaOfInteractable - TIMEBEFOREPROMPT);
            } else if(withinAreaOfInteractable > 0) {
                withinAreaOfInteractable = Mathf.Clamp(withinAreaOfInteractable - Time.deltaTime, 0, TIMEBEFOREPROMPT + 1);
                MousePrompt.color = Color.Lerp(mouseColInvis, mouseColVis, withinAreaOfInteractable - TIMEBEFOREPROMPT);
            }

            if (GameManager.GM != null) {
                ReloadCircle.fillAmount = GameManager.GM.reloadTimer / GameManager.TIMETORELOAD;
                if (showTimer) {
                    TimeSpan t = TimeSpan.FromSeconds(GameManager.GM.fullLevelTimer);
                    TimerText.text = string.Format("{0:D2}:{1:D2}.{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);
                    if (GameManager.GM.fullLevelTimer <= previousTime) {
                        ProgressTimer.fillAmount = GameManager.GM.fullLevelTimer / previousTime;
                        ProgressTimer.color = new Color(1, 0.85f, 0, 1);
                    } else { 
                        ProgressTimer.color = Color.red;
                    }
                }
            } else {
                ReloadCircle.fillAmount = 0;
            }

            if (!displayingHUD) {
				crosshair.enabled = true;
				ReloadCircle.enabled = true;
				WASDPrompt.enabled = true;
				MousePrompt.enabled = true;

                if (showTimer) {
                    TimerText.gameObject.SetActive(true);
                }
                displayingHUD = true;
            }
        }
    }
}
