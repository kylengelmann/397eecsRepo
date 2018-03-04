using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class velocityTaker : MonoBehaviour {

    [HideInInspector]public Vector3 velocity;
    Rigidbody rig;
    CharacterController charCtrl;
    bool useCharCtrl;
    [HideInInspector] public bool isOnGround;
    bool wasOnGround;

    public delegate void TransferVelocity(Vector3 vel);
    public TransferVelocity transferVelocity;

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
        if(isOnGround) {
            if(useCharCtrl) {
                charCtrl.Move(velocity*Time.fixedDeltaTime);
            }
            else {
                rig.MovePosition(rig.position + velocity*Time.fixedDeltaTime);
            }
        }
        else if(wasOnGround) {
            if(useCharCtrl) {
                if(transferVelocity != null) {
                    transferVelocity(velocity);
                }
            }
            else {
                rig.velocity += velocity;
            }
        }
        wasOnGround = isOnGround;
        isOnGround = false;
        velocity = Vector3.zero;
    }
}
