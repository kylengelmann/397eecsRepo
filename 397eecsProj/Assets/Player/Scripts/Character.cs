using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour {
// Component for character specific (rather than player specific) stuff
// Handles character physics and animations

	public Camera cam;
	public moveSettings settings;
	[HideInInspector] public Vector3 velocity;
	CharacterController charCtrl;
	Vector2 moveAxis;
	bool isJumping = false;
	Vector3 groundNormal;
	void Start () {
		charCtrl = gameObject.GetComponent<CharacterController>();
		groundNormal = Vector3.up;
	}

	void Update () {
	}

	bool isGrounded;
	RaycastHit groundHit;
	void checkGrounded() {
		if(Vector3.Dot(groundNormal, velocity) <= 0f) {
			isGrounded = Physics.SphereCast(transform.position, charCtrl.radius - 0.01f, -groundNormal, out groundHit, charCtrl.height/2f + 0.01f);
			// For directional gravity, let's not mess with it yet
//			if(isGrounded) {
//				groundNormal = groundHit.normal.normalized;
//			}
		}
	}

	void FixedUpdate() {
		checkGrounded();

		//Walking Physics
		//Find forwards direction
		Vector3 forwardDir = Vector3.Cross(cam.transform.right, groundNormal).normalized;

		//Split velocity into up, right, and forward components
		float up = Vector3.Dot(groundNormal, velocity);
		float right = Vector3.Dot(cam.transform.right, velocity);
		float forward = Vector3.Dot(forwardDir, velocity);

		//Figure out whether we're slowing down, speeding up, or switching directions and apply the correct acceleration
		Vector2 horizontalVel = new Vector2(right, forward);
		Vector2 accDir = moveAxis - (horizontalVel/settings.maxWalkSpeed);
		accDir.Normalize();
		if(Vector2.Dot(moveAxis, horizontalVel) >= 0f) { //not switching directions
			if(moveAxis.sqrMagnitude >= (horizontalVel/settings.maxWalkSpeed).sqrMagnitude) {//speeding up
				horizontalVel += accDir*settings.walkAcc*Time.fixedDeltaTime;
				horizontalVel = Vector2.ClampMagnitude(horizontalVel, moveAxis.magnitude*settings.maxWalkSpeed);
			}
			else {
				Vector2 acc = accDir*settings.stopAcc*Time.fixedDeltaTime;
				if(acc.sqrMagnitude >= horizontalVel.sqrMagnitude) {
					horizontalVel = Vector2.zero;
				}
				else {
					horizontalVel += acc;
				}
			}
		}
		else {
			horizontalVel += accDir*settings.switchDirAcc*Time.fixedDeltaTime;
			Vector2.ClampMagnitude(horizontalVel, moveAxis.magnitude*settings.maxWalkSpeed);
		}


		//Set velocity;
		velocity = horizontalVel.x*cam.transform.right + horizontalVel.y*forwardDir + up*groundNormal;





		charCtrl.Move(velocity*Time.fixedDeltaTime);
	}
		

	public void jump(string buttonName) {
		if(Input.GetButtonDown(buttonName)) {
			isJumping = true;
			velocity += gameObject.transform.up*settings.jumpVelocity;
		}
		else if(Input.GetButton(buttonName)) {
			isJumping = true;
		}
		else {
			isJumping = false;
		}
	}

	public void setMove(float x, float y) {
		moveAxis.x = x;
		moveAxis.y = y;
		if(moveAxis.sqrMagnitude > 1f) moveAxis.Normalize();
	}

	public void switchPlayers() {

	}
}



[System.Serializable]
public struct moveSettings {
	public float maxWalkSpeed;
	public float walkAcc;
	public float switchDirAcc;
	public float stopAcc;

	public float jumpVelocity;
	public float jumpGravity;
	public float endJumpGravity;
	public float fallingGravity;
}
