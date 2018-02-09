using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour {
// Component for character specific (rather than player specific) stuff
// Handles character physics and animations

	public Camera cam;
    public MoveSettings moveSettings;
    public CameraSettings camSettings;
    //Transform goalCamTransform;
    Vector3 prevCamPos;
    Quaternion prevCamRot;
    Vector3 goalCamPos;
    Quaternion goalCamRot;
	[HideInInspector] public Vector3 velocity;
	CharacterController charCtrl;
	Vector2 moveAxis;
	bool isJumping = false;
	Vector3 groundNormal;

    //Vector3 prevPos;
	void Start () {
		charCtrl = gameObject.GetComponent<CharacterController>();
		groundNormal = Vector3.up;
        //prevPos = transform.position;
        goalCamPos = cam.transform.position;
        goalCamRot = cam.transform.rotation;
	}

	void Update () {
        //charCtrl.Move(velocity*Time.deltaTime);
		Vector3 horizontalVel = velocity - Vector3.Dot(groundNormal, velocity)*groundNormal;
		if(horizontalVel.sqrMagnitude > 0.001f) {
			transform.rotation = Quaternion.LookRotation(horizontalVel, groundNormal);	
		}




	}

	bool isGrounded;
	RaycastHit groundHit;
	void checkGrounded() {
		if(Vector3.Dot(groundNormal, velocity) <= 0f) {
            int lm = gameObject.layer;
            lm = ~(1<<(lm-1));
			isGrounded = Physics.SphereCast(transform.position, charCtrl.radius - 0.01f, -groundNormal, 
                                            out groundHit, charCtrl.height/2f - charCtrl.radius + charCtrl.skinWidth + 0.05f, lm);
			// For directional gravity, let's not mess with it yet
//			if(isGrounded) {
//				groundNormal = groundHit.normal.normalized;
//			}
		}
		else {
			isGrounded = false;
		}
	}

    bool switchingDir = false;
	void FixedUpdate() {
		checkGrounded();

		
		//Find forwards direction
		Vector3 forwardDir = Vector3.Cross(cam.transform.right, groundNormal).normalized;

		//Split velocity into up, right, and forward components
		float up = Vector3.Dot(groundNormal, velocity); // Amount parallel to ground normal
        float right = Vector3.Dot(cam.transform.right, velocity); // Amount to the right;
		float forward = Vector3.Dot(forwardDir, velocity); // Amount forwards

        //Camera Calculations
        prevCamPos = cam.transform.position;
        prevCamRot = cam.transform.rotation;
        cam.transform.position = goalCamPos;
        cam.transform.rotation = goalCamRot;
        float angle = right*Time.fixedDeltaTime / moveSettings.turningRadius;
        Vector3 center = transform.position - forwardDir*moveSettings.turningRadius;

        cam.transform.RotateAround(center, groundNormal, Mathf.Rad2Deg*angle);
        goalCamPos = transform.position - cam.transform.forward*camSettings.distance;
        goalCamRot = cam.transform.rotation;
        cam.transform.position = Vector3.Lerp(prevCamPos, goalCamPos, .3f);
        cam.transform.rotation = Quaternion.Slerp(prevCamRot, goalCamRot, .3f);


        //Walking Physics
        Vector2 horizontalVel = new Vector2(right, forward); 
		Vector2 goalVel = moveAxis*moveSettings.maxWalkSpeed;
		Vector2 parVel = Vector2.Dot(horizontalVel, moveAxis.normalized)*moveAxis.normalized; // Parallel to goal
		Vector2 perpVel = horizontalVel - parVel; //Perpendicular to goal

        float control = isGrounded ? 1f : moveSettings.airControl;

        // Figure out whether we're slowing down, speeding up, or switching directions and apply the correct acceleration
        // Only apply it to parallel velocity, the perpendicular velocity is cancelled out by friction

        if (Vector2.Dot(goalVel, parVel) < 0f || switchingDir) // Switching directions
        {
            Vector2 diff = goalVel - parVel;
            float dVel = moveSettings.switchDirAcc * control * Time.fixedDeltaTime;
            if (dVel * dVel > diff.sqrMagnitude)
            {
                switchingDir = false;
                parVel = goalVel;
            }
            else
            {
                switchingDir = true;
                parVel += moveAxis.normalized * dVel;
            }

        }
        else
        {
            if (goalVel.sqrMagnitude >= parVel.sqrMagnitude) // Speeding up
            {
                parVel += moveAxis.normalized * moveSettings.walkAcc * control * Time.fixedDeltaTime;
                parVel = Vector2.ClampMagnitude(parVel, goalVel.magnitude);
            }
            else // Slowing down
            {
                float dVel = moveSettings.stopAcc * control * Time.fixedDeltaTime;
                if (dVel * dVel < (parVel - goalVel).sqrMagnitude) // Check for overcorrection
                {
                    parVel += moveAxis.normalized * dVel;
                }
                else parVel = goalVel; // Prevent overcorrection;

            }
        }

        if (perpVel.sqrMagnitude > 0.01f) // Let friction sort out the perpendicular velocity
        {
            float dPerp = moveSettings.stopAcc * control * Time.fixedDeltaTime;
            if (dPerp * dPerp < perpVel.sqrMagnitude) // Check for overcorrection
            {
                perpVel -= dPerp * perpVel.normalized;
            }
            else
            { 
                perpVel = Vector2.zero; // Prevent overcorrection
            }
        }
        else perpVel = Vector2.zero;

        horizontalVel = parVel + perpVel; // Combine parallel and perpendicular


        //Jumping
		if(!isGrounded) {
			if(up > 0f) {
				if(isJumping) {
					up -= moveSettings.jumpGravity*Time.fixedDeltaTime;
				}
				else {
					up -= moveSettings.endJumpGravity*Time.fixedDeltaTime;
				}
			}
			else {
				up -= moveSettings.fallingGravity*Time.fixedDeltaTime;
			}
		}
		else {
			up = 0f;
		}


		//Set velocity;
		velocity = horizontalVel.x*cam.transform.right + horizontalVel.y*forwardDir + up*groundNormal;





		charCtrl.Move(velocity*Time.fixedDeltaTime);
        //rig.MovePosition(rig.position + velocity*Time.fixedDeltaTime);



	}
		

	public void jump(string buttonName) {
		if(Input.GetButtonDown(buttonName) && isGrounded) {
			isJumping = true;
			velocity += groundNormal*moveSettings.jumpVelocity;
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

    public void setCam(float x, float y) {
        
    }

	public void switchPlayers() {

	}
}



[System.Serializable]
public struct MoveSettings {
	public float maxWalkSpeed;
	public float walkAcc;
	public float switchDirAcc;
	public float stopAcc;
    public float turningRadius;

	public float jumpVelocity;
	public float jumpGravity;
	public float endJumpGravity;
	public float fallingGravity;
    public float airControl;
}

[System.Serializable]
public struct CameraSettings {
    public float sensitivity;
    public float distance;
}

