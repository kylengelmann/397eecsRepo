using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movedByGround : MonoBehaviour {

    [HideInInspector]public Vector3 velocity;
    Rigidbody rig;
    CharacterController charCtrl;
    bool useCharCtrl;

	// Use this for initialization
	void Start () {
        if((charCtrl = gameObject.GetComponent<CharacterController>()) != null) {
            useCharCtrl = true;
        }
        else {
            rig = gameObject.GetComponent<Rigidbody>();
            useCharCtrl = false;
        }
	}
	
	void FixedUpdate () {
        if(useCharCtrl) {
            charCtrl.Move(velocity*Time.fixedDeltaTime);
        }
        else {
            rig.MovePosition(rig.position + velocity*Time.fixedDeltaTime);
        }
        velocity = Vector3.zero;
	}
}
