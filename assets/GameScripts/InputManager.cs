using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	
	public InputManager IM;
	
	public bool InvertMouseX;
	public bool InvertMouseY;
	public bool InvertHorizontal;
	public bool InvertVertical;
	
	public KeyCode Jump;
	public KeyCode Crouch;
	public KeyCode Use;
	public KeyCode Sprint;
	public KeyCode Attack;
	public KeyCode SecAttack;
	
	void Awake (){
		IM = this;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ChangeBinding(KeyCode changingKey /*Must be InputManager.IM.variable*/, KeyCode newKey /*Must be a KeyCode.key*/){
		changingKey = newKey;
	}
}
