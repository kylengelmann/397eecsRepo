using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour {
// Component for character specific (rather than player specific) stuff
// Handles character physics and animations, as well as camera motion

	public Camera cam;
    public LayerMask camMask;
    Animator anim;
    public checkpoint lastCkpt;

    public ParticleSystem jetPackParticles;
    public ParticleSystem runThrusterParticles;

    public AudioClip smashSfx;
    public AudioClip cakeSfx;
    public AudioClip levelSfx; 

    private AudioSource audioSource;


    //Settings
    public MoveSettings moveSettings; //See bottom of script
    public CameraSettings camSettings; //See bottom of script

    //For camera calculations
    Vector3 goalCamPos;
    Quaternion goalCamRot = Quaternion.identity;
    Quaternion goalCamRotNoY = Quaternion.identity;
    Vector2 camAxis;
    float camRotY = 30f;

    //For physics calculations
	[HideInInspector] public Vector3 velocity;
	CharacterController charCtrl; //Object that checks for collisions and moves character
    Vector2 moveAxis; // The values of the input axes e.g moveAxis.x = Input.GetAxis("Horizontal")
    Vector3 groundNormal; 

    // Animation bools
    bool isJumping = false; // Is the jumping button being held down?
    bool isGrounded; // Is the player on the ground?
    bool isMovingObj = false; // is the player moving an object

    [HideInInspector] public enum characterState { // Various states the character can be in
        free, // Default state for moving, idle, and jumping
        switching, // State while switching
        moving,
        smashing
    }

    public characterState currentState = characterState.free; // What the character is currently doing

    [HideInInspector] public int MovingPlayer;
    public Vector3 RespawnPoint;

    velocityTaker velTake;

    void transferVelFromVelTaker(Vector3 vel) {
        velocity += vel;
    }

	void Awake()
	{
        audioSource = GetComponent<AudioSource>();
	}

	void Start () {
		charCtrl = gameObject.GetComponent<CharacterController>();
		groundNormal = Vector3.up;
        anim = gameObject.GetComponent<Animator>();
        anim.SetBool("isP1Moving", true);


        velTake = gameObject.GetComponent<velocityTaker>();
        if(velTake != null) {
            velTake.transferVelocity = transferVelFromVelTaker;
        }

	    MovingPlayer = 1;
	    RespawnPoint = gameObject.transform.position;

        reset();
	}

    public void reset() {
        //transform.position = RespawnPoint;
        velocity = Vector3.zero;
        transform.position = lastCkpt.transform.position;
        transform.rotation = lastCkpt.transform.rotation;
        //transform.position = Vector3.up; //Kyle's original
        cam.transform.rotation = lastCkpt.transform.rotation*Quaternion.AngleAxis(30f, Vector3.right);
        //cam.transform.rotation = Quaternion.AngleAxis(30f, Vector3.right);
        cam.transform.position = transform.position - cam.transform.forward*camSettings.distance;
        goalCamRotNoY = lastCkpt.transform.rotation;
        camRotY = 30f;

        currentState = characterState.free;


    }

	//void Update () {
 //       // Checks the velocity parallel to the ground plane and rotates the character
 //       // to face that direction
	//	//Vector3 horizontalVel = velocity - Vector3.Dot(groundNormal, velocity)*groundNormal;
		
	//}


    //*************************************************
    //Physics and Camera
    //*************************************************

	
    RaycastHit groundHit; // Stores information about the ground
    // Checks if the player is on the ground
	void checkGrounded() {
		if(Vector3.Dot(groundNormal, velocity) <= 0.1f) {
            int lm = gameObject.layer; //LayerMask
            lm = ~(1<<(lm));
            lm &= ~(1<<(LayerMask.NameToLayer("Ignore Raycast")));
            lm &= ~(1<<(LayerMask.NameToLayer("Key")));
			if(Physics.SphereCast(transform.position, charCtrl.radius - 0.05f, -groundNormal, 
                                  out groundHit, charCtrl.height/2f - charCtrl.radius + charCtrl.skinWidth + 0.08f, lm)) {


                isGrounded = !groundHit.collider.isTrigger;
            }
            else isGrounded = false;
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


        ////////////////////////
        //Camera Calculations

        //Angle (in degrees) The player has moved in a frame
        float angle = right*Time.fixedDeltaTime / moveSettings.turningRadius * Mathf.Rad2Deg;
        goalCamRotNoY = goalCamRotNoY*Quaternion.AngleAxis(angle, groundNormal); //Apply that angle to the goal rotation

        float xAngle = camAxis.x*Time.fixedDeltaTime; //Amount to rotate from player input
        goalCamRotNoY = goalCamRotNoY*Quaternion.AngleAxis(xAngle, groundNormal); //Apply that angle to the goal rotation

        //Reapply vertical every frame to keep camRotY accurate
        camRotY += camAxis.y*Time.fixedDeltaTime; //Amount to rotate from player input
        camRotY = Mathf.Clamp(camRotY, camSettings.minAngle, camSettings.maxAngle); //Clamp to settings
        goalCamRot = goalCamRotNoY*Quaternion.AngleAxis(camRotY, Vector3.right); //Apply rotation


        //To prevent camera from clipping though the floor
        RaycastHit camHit;
        Vector3 camDir = goalCamRot*(-Vector3.forward); // Direction to perform sphere cast
        float camDist = camSettings.distance; //Will store distance camera is from player
        int lm = camMask.value;
        float camRadius = .5f; // Radius of sphereCast
        if(Physics.SphereCast(transform.position, camRadius, camDir, out camHit, camSettings.distance - camRadius, lm)) {
            if(!camHit.collider.isTrigger) camDist  = camHit.distance; //If object is inbetween camera and player, adjust distance to prevent clipping
        }
        goalCamPos = transform.position + camDir * camDist;

        //Lerp/Slerp between previous and goal to get new. Stiffness determines amount of lag
        cam.transform.position = Vector3.Lerp(cam.transform.position, goalCamPos, camSettings.stiffness);
        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, goalCamRot, camSettings.stiffness);


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
                jetPackParticles.Stop();
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

        // direction facing
        Vector3 worldHorizontalVel = velocity - Vector3.Dot(groundNormal, velocity) * groundNormal;

        ////////////////////////////////////
        // move object
        // TODO
        if (currentState == characterState.moving) {
            //movingCube.isKinematic = false;
            movingCube.MovePosition(movingCube.position + groundNormal*0.1f);
            //movingCube.constraints = RigidbodyConstraints.FreezeRotationX | 
            //                         RigidbodyConstraints.FreezeRotationZ | 
            //                         RigidbodyConstraints.FreezePositionY;
            //movingCube.drag = 10f;
            //movingCube.angularDrag = 10f;

            Vector3 goalPos = grabPoint.position + grabPoint.forward * 0.75f;
            Quaternion goalRot = moved.transform.rotation;
            Vector3 goalDir = transform.position - goalPos; // dir from goal to player
            float goalDist = goalDir.magnitude;
            goalDir.Normalize();
            float vDotGoal = Vector3.Dot(velocity, goalDir);

            //velocity /= 1.3f;


            if (vDotGoal > 0) 
            {
                Vector3 goalVelo = vDotGoal * goalDir; // vel towards goal
                velocity -= goalVelo;
                goalVelo /= Mathf.Exp(0.8f*goalDist);
                velocity += goalVelo;
            }

            Vector3 boxGoalPos = transform.position + transform.forward*(0.75f+moved.transform.localScale.z);
            Quaternion boxGoalRot = transform.rotation;
            //float boxGoalAngle = 1f - Quaternion.Dot(movingCube.rotation, boxGoalRot); //Quaternion.Angle(movingCube.rotation, boxGoalRot);

            float boxGoalAngle = Mathf.Asin(Vector3.Dot(Vector3.Cross(movingCube.transform.forward, transform.forward), groundNormal));
            //Debug.Log(Mathf.Rad2Deg*boxGoalAngle);

            Vector3 boxGoalDir = boxGoalPos - movingCube.position;
            movingCube.maxAngularVelocity = Mathf.Infinity;
            movingCube.AddForce(boxGoalDir*50f);
            movingCube.AddTorque(boxGoalAngle*groundNormal*250f);
            //movingCube.MoveRotation(transform.rotation);

            if (worldHorizontalVel.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(worldHorizontalVel, groundNormal), 0.1f); // dir facing   
            }
        }
        else{
            if (worldHorizontalVel.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(worldHorizontalVel, groundNormal), 0.33f); // dir facing   
            }
        }



		CollisionFlags cFlags = charCtrl.Move(velocity*Time.fixedDeltaTime); //Move the character

        //Cancel out velocity caracter hits the floor or ceiling
        float vDotG = Vector3.Dot(velocity, groundNormal);
        if((cFlags&CollisionFlags.Below) != 0 && vDotG < 0f) {
            velocity -= vDotG*groundNormal;
        }
        if((cFlags&CollisionFlags.Above) != 0 && vDotG > 0f) {
            velocity -= vDotG*groundNormal;
        }
        //if(((cFlags&CollisionFlags.Below) != 0) || ((cFlags&CollisionFlags.Above) != 0)) {
        //    velocity -= Vector3.Dot(velocity, groundNormal)*groundNormal;
        //}

	}
		


    //**********************************************************
    // Handling Input from playerControllers
    //**********************************************************

	public void jump(bool isPressed) {
        if(currentState == characterState.switching) { 
            isPressed = false;
        }
        if(isPressed && !isJumping && isGrounded) { //
            if(isGrounded) {
    			isJumping = true;
    			velocity += groundNormal*moveSettings.jumpVelocity;
            }
            jetPackParticles.Play();
		}
		else {
            if(!isPressed && isJumping) {
                jetPackParticles.Stop();
            }
            isJumping = isPressed;
        }
	}

    bool isRunning;
    public void run(bool isPressed) {
        if(currentState == characterState.switching) {
            isPressed = false;
        }
        if(!isRunning && isPressed) {
            runThrusterParticles.Play();
        }
        else if(isRunning && !isPressed) {
            runThrusterParticles.Stop();
        }
        isRunning = isPressed;
    }

    bool smash = false;
    public void breakObject(bool isPressed)
    {
        if(currentState == characterState.switching) {
            isPressed = false;
        }
        //Make it such that pressing the button makes the interactions capsule coll. enabled for a bit
        //but then is disabled regardless if the button is held down

        if (isPressed && !smash)
        {
            //Vector3 currPosition = gameObject.transform.position;
            //Collider[] touched = Physics.OverlapCapsule(currPosition, new Vector3(currPosition.x, currPosition.y, currPosition.z + 1.0f), 0.5f);
            anim.ResetTrigger("isBreakingObj");
            anim.SetTrigger("isBreakingObj");
            currentState = characterState.smashing;
            // add a Time.deltaTime 
        //    foreach (Collider collider in touched) //Checks everything it collided with to see if any objects it detected are breakable
        //    {
        //        if (collider.gameObject.GetComponent<InteractableObject>()) 
        //        {
        //            if (collider.gameObject.GetComponent<InteractableObject>().isBreakable)
        //            {
        //                //TODO Play character and object animations for breaking
        //                Destroy(collider.gameObject);
        //            }
        //        }
        //    }
        //}
        }
        smash = isPressed;
    }

    public void doBreak() {
        Vector3 currPosition = gameObject.transform.position;
        //Collider[] touched = Physics.OverlapCapsule(currPosition, new Vector3(currPosition.x, currPosition.y, currPosition.z + 1.0f), 0.5f);
        Vector3 boxPos = transform.position + transform.right*0f + transform.up*0.25f + transform.forward*0.5f;
        Collider[] touched = Physics.OverlapBox(boxPos, new Vector3(0.375f, 0.25f, 1.0f), transform.rotation); //Z was originally 0.25
        foreach (Collider collider in touched) //Checks everything it collided with to see if any objects it detected are breakable
        {
            if (collider.gameObject.GetComponent<InteractableObject>()) 
            {
                if (collider.gameObject.GetComponent<InteractableObject>().isBreakable)
                {
                    //TODO Play character and object animations for breaking
                    //Destroy(collider.gameObject);
                    collider.gameObject.SetActive(false);
                    audioSource.clip = smashSfx;
                    audioSource.Play();
                    break;
                }
            }
        }
    }

    public void endBreak() {
        currentState = characterState.free;
    }


    bool isMoving;
    moveable moved; // obj ur trying to move
    Rigidbody movingCube; // rb of the cube player is moving 
    Transform grabPoint; // transform corr. w/ face player grabs

    public void moveObject(bool isPressed) //////////////////////// MOVING OBJECTS
    // TODO
    {
        //While the button is pressed, if there is an interactable object that can be moved
        //Refer to the breakObject function to see how capsule overlap is being used and how to find specific objects

        //anim.SetBool("isMovingObj", true);
        if(currentState == characterState.switching) {
            isPressed = false;
        }
        anim.SetBool("isMovingObj", isPressed);

        if (isPressed && !isMoving) 
        {

            int lm = 1 << LayerMask.NameToLayer("Moveable"); // layer mask
            Collider[] touched = new Collider[1]; // get first object you touch after press
            Vector3 boxPos = transform.position + transform.right*0f + transform.up*0.25f + transform.forward*0.5f;
            int objTouched = Physics.OverlapBoxNonAlloc(boxPos, new Vector3(0.375f, 0.25f, 0.25f), 
                                                        touched, transform.rotation, lm);
            if (objTouched == 0) {
                return;
            }

            currentState = characterState.moving;
            moved = touched[0].gameObject.GetComponent<moveable>(); // the moved obj
            movingCube = moved.gameObject.GetComponent<Rigidbody>(); // its rb

            float minDist = Mathf.Infinity;
            foreach(Transform t in moved.faceTs) { // get the closest face and set grabPoint
                float currDist = Vector3.Distance(transform.position, t.position);
                if(currDist < minDist) {
                    minDist = currDist;
                    grabPoint = t;
                }
            }

            transform.position = grabPoint.position + grabPoint.forward * 0.75f;// align player to center of face
            transform.LookAt(grabPoint); // player face grabPoint
            moved.transform.rotation = transform.rotation; // make box's rot the player's rot
            grabPoint = moved.defaultGrab; // reassign grab point
            //moved.transform.parent = transform; // parent that ish
            //movingCube.isKinematic = false;

            movingCube.constraints = RigidbodyConstraints.FreezeRotationX | 
                RigidbodyConstraints.FreezeRotationZ | 
                RigidbodyConstraints.FreezePositionY;
            movingCube.drag = 8f;
            movingCube.angularDrag = 10f;
        }



        isMoving = isPressed;
        if (!isMoving && currentState == characterState.moving) {
            //movingCube.isKinematic = true;
            //moved.transform.parent = null; // unparent that ish
            movingCube.drag = 1f;
            movingCube.angularDrag = 0f;
            movingCube.constraints = RigidbodyConstraints.None;
            currentState = characterState.free;
        }
    }


	public void setMove(float x, float y) {
        if(currentState == characterState.switching || currentState == characterState.smashing) {
            moveAxis = Vector2.zero;
            return;
        }
		moveAxis.x = x;
		moveAxis.y = y;
        if(moveAxis.sqrMagnitude > 1f) moveAxis.Normalize();
        anim.SetBool("isWalking", (moveAxis.sqrMagnitude > 0.001f));
	}

    public void setCam(float x, float y) {
        if(currentState == characterState.switching) {
            camAxis = Vector2.zero;
            return;
        }
        camAxis.y = -y*camSettings.ySensitivity; //Positive rotations rotate the camera down;
        camAxis.x = x*camSettings.xSensitivity;
    }

	public bool switchPlayers() {
        if(currentState != characterState.free) { // If the character isn't in the default state,
                                                    // it probably won't be allowed to switch
            return false;
        }
        currentState = characterState.switching; // Record that the character is switching
        anim.ResetTrigger("isSwitching");
        anim.SetTrigger("isSwitching");
        bool p1OrNah = anim.GetBool("isP1Moving");
        anim.SetBool("isP1Moving", !p1OrNah);

	    MovingPlayer = MovingPlayer == 1 ? 2 : 1;

        return true;
	}

    public void endSwitch() {
        //transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -transform.localScale.z);
        transform.LookAt(transform.position - transform.forward);
        currentState = characterState.free; // Record thate the character is no longer switching
    }

	private void OnTriggerEnter(Collider other)
	{
        if (other.GetComponent<Collectible>()) {
            audioSource.clip = cakeSfx;
            audioSource.Play();
        }
        else if (other.GetComponent<Telepad>()) {
            audioSource.clip = levelSfx;
            audioSource.Play();
        }
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
    public float minAngle;
}

