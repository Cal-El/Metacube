using UnityEngine;
using System.Collections;

public class CurveRotator : MonoBehaviour {

    public static class CurveRotationManager {

        private const float ROTATION_SPEED = 90;
        private const float FINISHING_THRESHOLD = 0.5f;
        public static bool isRotating = false;
        private static bool initialized = false;

        //private static Vector3 currentWorldUp = Vector3.up;
        private static float remainingRotation = 0;
        private static Vector3 spinAxis = Vector3.right;

        private static Transform Player;
        private static WorldRotation World;

        public static void Init() {
            if (!initialized) {
                Player = GameManager.GM.player;
                World = GameManager.GM.GetComponent<WorldRotation>();
            }
            initialized = true;
        }

        public static void Update() {
            if (!initialized || !isRotating || World.IsRotating) {
                return;
            }
            if(remainingRotation- FINISHING_THRESHOLD > 0) {
                float rotationAmount = Mathf.Lerp(0, remainingRotation, Time.deltaTime *5);
                World.transform.RotateAround(Player.position, spinAxis, rotationAmount);
                remainingRotation -= rotationAmount;
            } else {
                World.transform.RotateAround(Player.position, spinAxis, remainingRotation);
                remainingRotation = 0;
                isRotating = false;
            }
        }

        public static bool SetNewWorldUp(Vector3 newWorldUp, bool isGround) {
            if(!initialized || Vector3.Angle(newWorldUp, Vector3.up) >= (isGround?45:35) || World.IsRotating) {
                return false;
            }

            spinAxis = Vector3.Cross(newWorldUp, Vector3.up);
            remainingRotation = Vector3.Angle(newWorldUp, Vector3.up);
            isRotating = true;
            return true;
        }
    }

    private Vector3 aroundAxis;

	// Use this for initialization
	void Start () {
        aroundAxis = transform.right;
        CurveRotationManager.Init();
	}

    public void SetNewWorldUp(Vector3 newUp) {
        CurveRotationManager.SetNewWorldUp(newUp, false);
    }
}
