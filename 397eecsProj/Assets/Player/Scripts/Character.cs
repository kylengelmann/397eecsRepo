using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour {
// Handles character physics and animations

	[HideInInspector] public Vector3 velocity;
	CharacterController charCtrl;
	void Start () {
		charCtrl = gameObject.GetComponent<CharacterController>();
	}

	void Update () {
		
	}

	void FixedUpdate() {
		charCtrl.Move(velocity);
	}

	public void switchPlayers() {
		
	}

	public void jump(string buttonName) {
		
	}

	public void setMove(float x, float y) {
		
	}
}
