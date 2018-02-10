using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour {
// Component for character specific (rather than player specific) stuff
// Handles character physics and animations, as well as camera motion

	public Camera cam;
    Animator anim;

    //Settings
    public MoveSettings moveSettings; //See bottom of script
    public CameraSettings camSettings; //See bottom of script

    //For camera calculations
    Vector3 prevCamPos;
    Quaternion prevCamRot;
    Vector3 goalCamPos;
    Quaternion goalCamRot;
    Vector2 camAxis;
    float camRotY = 30f;

    //For physics calculations
	[HideInInspector] public Vector3 velocity;
	CharacterController charCtrl;
    Vector2 moveAxis; // The values of the input axes e.g moveAxis.x = Input.GetAxis("Horizontal")
	bool isJumping = false; // Is the jumping button being held down?
	Vector3 groundNormal; 

	void Start () {
		charCtrl = gameObject.GetComponent<CharacterController>();
		groundNormal = Vector3.up;
        //goalCamPos = cam.transform.position;
        //goalCamRot = cam.transform.rotation;

        anim = gameObject.GetComponent<Animator>();
        anim.SetBool("isP1Moving", true);
	}

	void Update () {
        // Checks the velocity parallel to the ground plane and rotates the character
        // to face that direction
		Vector3 horizontalVel = velocity - Vector3.Dot(groundNormal, velocity)*groundNormal;
		if(horizontalVel.sqrMagnitude > 0.001f) {
			transform.rotation = Quaternion.LookRotation(horizontalVel, groundNormal);	
		}
	}


    //*************************************************
    //Physics and Camera
    //*************************************************

	bool isGrounded; // Is the player on the ground?
    RaycastHit groundHit; // Stores information about the ground


    // Checks if the player is on the ground
	void checkGrounded() {
		if(Vector3.Dot(groundNormal, velocity) <= 0.1f) {
            int lm = gameObject.layer;
            lm = ~(1<<(lm-1));
			isGrounded = Physics.SphereCast(transform.position, charCtrl.radius - 0.01f, -groundNormal, 
                                            out groundHit, charCtrl.height/2f - charCtrl.radius + charCtrl.skinWidth + 0.08f, lm);
			// For directional gravity, let's not mess with it yet
//			if(isGrounded) {
//				groundNormal = groundHit.normal.normalized;
//			}
		}
		else {
			isGrounded = false;
		}
        anim.SetBool("isJumping", !isGrounded);
	}




    bool switchingDir = false; // Used for walking physics. Stores whether the character is switching directions

	void FixedUpdate() {

        //Check if the player is on the ground
		checkGrounded();

		
        //Find forwards direction (relative to camera)
		Vector3 forwardDir = Vector3.Cross(cam.transform.right, groundNormal).normalized;

        //Split velocity into up, right, and forward components (relative to camera)
        // Walking input moves the player relative to the camera, so this is needed for walking,
        // And it is also useful for camera calculations
		float up = Vector3.Dot(groundNormal, velocity); // Amount parallel to ground normal
        float right = Vector3.Dot(cam.transform.right, velocity);
		float forward = Vector3.Dot(forwardDir, velocity);


        ////////////////////////
        //Camera Calculations

        //I can't make a dummy transform cause unity's a dummy, so I'll use the camera's for calculation
        cam.transform.position = goalCamPos;
        cam.transform.rotation = goalCamRot;

        //Angle (in radians) the player has moved in a frame
        float angle = right*Time.fixedDeltaTime / moveSettings.turningRadius;
        //Center of the player's rotation
        Vector3 center = transform.position - forwardDir*moveSettings.turningRadius;
        //Calculate the camera's goal rotation using the transform because unity decided not to let you
        //do an axis angle rotation of a quaternion (wtf unity)
        cam.transform.RotateAround(center, groundNormal, Mathf.Rad2Deg*angle);

        float xAngle = camAxis.x*Time.fixedDeltaTime; //Amount to rotate from player input
        cam.transform.RotateAround(transform.position,groundNormal, xAngle); //Rotate for player input

        //Store the result into the goal pos/rot
        goalCamPos = transform.position - cam.transform.forward*camSettings.distance;
        goalCamRot = cam.transform.rotation;

        cam.transform.position = prevCamPos;
        cam.transform.rotation = prevCamRot;
        cam.transform.RotateAround(transform.position,groundNormal ,xAngle); //Rotate from player input
        //Move the camera towards the goal pos/rot
        cam.transform.position = Vector3.Lerp(prevCamPos, goalCamPos, camSettings.stiffness);
        cam.transform.rotation = Quaternion.Slerp(prevCamRot, goalCamRot, camSettings.stiffness);

        prevCamPos = cam.transform.position;
        prevCamRot = cam.transform.rotation;

        camRotY += camAxis.y*Time.fixedDeltaTime;//Handle vertical separately to keep camRotY accurate and clamped
        camRotY = Mathf.Clamp(camRotY, -camSettings.maxAngle, camSettings.maxAngle);
        cam.transform.RotateAround(transform.position, cam.transform.right, camRotY);



        /////////////////////
        //Walking Physics
        Vector2 horizontalVel = new Vector2(right, forward); // Horizontal component of velocity relative to camera
        float maxSpeed = moveSettings.maxWalkSpeed;
        if(isRunning) maxSpeed = moveSettings.maxRunSpeed;
		Vector2 goalVel = moveAxis*maxSpeed; // Goal velocity relative to camera
		Vector2 parVel = Vector2.Dot(horizontalVel, moveAxis.normalized)*moveAxis.normalized; // Parallel to goal
		Vector2 perpVel = horizontalVel - parVel; //Perpendicular to goal

        float control = isGrounded ? 1f : moveSettings.airControl; //This allows for more floaty controls in the air by 
                                                                    // modifying acceleration

        // Figure out whether we're slowing down, speeding up, or switching directions and apply the correct acceleration
        // Only apply it to parallel velocity, the perpendicular velocity is cancelled out by friction
        if (Vector2.Dot(goalVel, parVel) < 0f || switchingDir) // Switching directions
        {
            Vector2 diff = goalVel - parVel;
            float dVel = moveSettings.switchDirAcc * control * Time.fixedDeltaTime;

            if (dVel * dVel > diff.sqrMagnitude) // Have we finished the switch?
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
                    parVel -= moveAxis.normalized * dVel;
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

        /////////////////
        //Jumping
		if(!isGrounded) {
			if(up > 0f) {
				if(isJumping) { //is the jump button held down?
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
            up = -moveSettings.fallingGravity*Time.fixedDeltaTime; // if we are grounded, add a bit of gravity
                                                                    // when moveing the character to keep them 
                                                                    // grounded, but later this velocity will be
                                                                    // set to zero
		}

        ////////////////////////////////////
		//Set velocity and move character
		velocity = horizontalVel.x*cam.transform.right + horizontalVel.y*forwardDir + up*groundNormal;


		charCtrl.Move(velocity*Time.fixedDeltaTime); //Move the character

        if(isGrounded) velocity -= up*groundNormal; //Cancel out the gravity added when the caracter is grounded, since
                                                    // we aren't actually moving that direction
	}
		


    //**********************************************************
    // Handling Input from playerControllers
    //**********************************************************

	public void jump(bool isPressed) {
        if(isPressed && !isJumping && isGrounded) { //
            if(isGrounded) {
    			isJumping = true;
    			velocity += groundNormal*moveSettings.jumpVelocity;
            }
		}
        else if(isPressed) {
			isJumping = true;
		}
		else {
			isJumping = false;
		}
	}

    bool isRunning;
    public void run(bool isPressed) {
        isRunning = isPressed;
    }

	public void setMove(float x, float y) {
		moveAxis.x = x;
		moveAxis.y = y;
		if(moveAxis.sqrMagnitude > 1f) moveAxis.Normalize();
        anim.SetBool("isWalking", moveAxis.sqrMagnitude > 0.001f);
	}

    public void setCam(float x, float y) {
        camAxis.y = -y*camSettings.ySensitivity; //Positive rotations rotate the camera down;
        camAxis.x = x*camSettings.xSensitivity;
    }

	public void switchPlayers() {
        anim.ResetTrigger("isSwitching");
        anim.SetTrigger("isSwitching");
        bool p1OrNah = anim.GetBool("isP1Moving");
        anim.SetBool("isP1Moving", !p1OrNah);
	}
}


//***************************************
//Settings Structures
//***************************************

[System.Serializable]
public struct MoveSettings {
	public float maxWalkSpeed;
    public float maxRunSpeed;
	public float walkAcc; //Acceeration used when speeding up
	public float switchDirAcc; //Used when switching directions until goal velocity is met
	public float stopAcc; //Used when slowing down
    public float turningRadius; //Radius when input is exactly sideways

	public float jumpVelocity; //Initial impulse when jump button is pressed
	public float jumpGravity; //gravity while the jump button is held down and still moving up
    public float endJumpGravity; //gravity while the jump button is not held down and still moving up
	public float fallingGravity; //gravity while moving down
    public float airControl; //1 is exactly like ground movement
                                // 0 is no control
}

[System.Serializable]
public struct CameraSettings {
    public float xSensitivity; 
    public float ySensitivity;

    public float distance; //Maximum distance between camera and player
    public float stiffness; //How much the camera lags behind during motion. 1 is no lag, 0 is no
                            //camera movement
    public float maxAngle;
}

