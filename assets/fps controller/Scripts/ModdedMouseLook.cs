using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a transform. Make the transform a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the transform.
///   -> Set the mouse look to use LookY. (You want the transform to tilt up and down like a head. The character already turns.)
[AddComponentMenu("transform-Control/Mouse Look")]
public class ModdedMouseLook : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;
	
	float rotationY = 0F;
	public float xRot;

    private CharacterMotorC cc;

    void Start()
    {
        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
        cc = FindObjectOfType<CharacterMotorC>();
        xRot = transform.localEulerAngles.y;

        if (PlayerPrefs.HasKey("Sensitivity")) {
            sensitivityX = PlayerPrefs.GetFloat("Sensitivity");
            sensitivityY = PlayerPrefs.GetFloat("Sensitivity");
        } else {
            PlayerPrefs.SetFloat("Sensitivity", 5f);
        }
    }

    void Update ()
	{
		//transform.parent.eulerAngles = new Vector3(0,transform.localRotation.eulerAngles.y,0);
		if(!cc.canControl)
		{
			
		}
		else
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

            if (PlayerPrefs.HasKey("Sensitivity")) {
                sensitivityX = PlayerPrefs.GetFloat("Sensitivity");
                sensitivityY = PlayerPrefs.GetFloat("Sensitivity");
            } else {
                PlayerPrefs.SetFloat("Sensitivity", 5f);
            }
            if (Input.GetButton("Zoom"))
            {
                sensitivityX *= 0.1f;
                sensitivityY *= 0.1f;
            }

            if (axes == RotationAxes.MouseXAndY)
			{
				float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
				
				transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
			}
			else if (axes == RotationAxes.MouseX)
			{
				xRot += Input.GetAxis("Mouse X") * sensitivityX;
				if (xRot<=-180f){
					xRot+=360f;	
				} else if (xRot>=180f){
					xRot-=360f;	
				}
                //transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
                if (PlayerPrefs.GetString("Mouse Smoothing") == "True") {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, xRot, transform.rotation.eulerAngles.z)), Time.deltaTime * 20);
                } else {
                    transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, xRot, transform.rotation.eulerAngles.z));
                }
			}
			else
			{
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
				
				//transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
                if (PlayerPrefs.GetString("Mouse Smoothing") == "True") {
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3(-rotationY, transform.localEulerAngles.y, 0)), Time.deltaTime*20 );
                } else {
                    transform.localRotation = Quaternion.Euler(new Vector3(-rotationY, transform.localEulerAngles.y, 0));
                }
            }
		}
	}
}