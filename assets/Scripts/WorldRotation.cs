using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldRotation : MonoBehaviour {
	 
	public Transform player;

    public float defaultTurnAmount = 90;
    public float defaultDegreesPerSecond = 18;

    public class WorldRot {
        protected float fullRotationAmount = 0;
        protected Vector3 aroundAxis;
        protected float rotSpeed = 1;
        private float sumRotation = 0;
        private GameObject world;

        public WorldRot (float rotation, Vector3 axis, float radialSpeed, GameObject rotationObject) {
            fullRotationAmount = rotation;
            aroundAxis = axis;
            rotSpeed = radialSpeed;
            world = rotationObject;
        }

        public bool Tick() {
            if(sumRotation + Time.deltaTime*rotSpeed < fullRotationAmount) {
                world.transform.RotateAround(GameManager.GM.player.position, aroundAxis, Time.deltaTime * rotSpeed);
                sumRotation += Time.deltaTime * rotSpeed;
                return false;
            } else {
                world.transform.RotateAround(GameManager.GM.player.position, aroundAxis, fullRotationAmount-sumRotation);
                sumRotation = fullRotationAmount;
                return true;
            }
        }
    }
    List<WorldRot> rotationList;
	
	// Update is called once per frame
	void Update () {
        if(rotationList != null && rotationList.Count > 0) {
            if (rotationList[0].Tick()) {
                rotationList.RemoveAt(0);
            }
        }	
	}

    public void startRotation(Vector3 around) {
        if(rotationList == null) {
            rotationList = new List<WorldRot>();
        }

        rotationList.Add(new WorldRot(defaultTurnAmount, around, defaultDegreesPerSecond, this.gameObject));
        GameManager.GM.progression++;
    }
    public void startRotation(float rotationAmount, Vector3 around) {
        if (rotationList == null) {
            rotationList = new List<WorldRot>();
        }

        rotationList.Add(new WorldRot((rotationAmount != 0) ? rotationAmount : defaultTurnAmount, around, defaultDegreesPerSecond, this.gameObject));
        GameManager.GM.progression++;
    }
    public void startRotation(Vector3 around, float rotationSpeed) {
        if (rotationList == null) {
            rotationList = new List<WorldRot>();
        }

        rotationList.Add(new WorldRot(defaultTurnAmount, around, (rotationSpeed != 0) ? rotationSpeed : defaultDegreesPerSecond, this.gameObject));
        GameManager.GM.progression++;
    }
    public void startRotation(float rotationAmount, Vector3 around, float rotationSpeed){
        if (rotationList == null) {
            rotationList = new List<WorldRot>();
        }

        rotationList.Add(new WorldRot((rotationAmount != 0) ? rotationAmount:defaultTurnAmount, around, (rotationSpeed != 0) ? rotationSpeed : defaultDegreesPerSecond, this.gameObject));
		GameManager.GM.progression ++;
	}

    public void ClearList() {
        if (rotationList == null) {
            rotationList = new List<WorldRot>();
        }

        rotationList.Clear();
    }
}
