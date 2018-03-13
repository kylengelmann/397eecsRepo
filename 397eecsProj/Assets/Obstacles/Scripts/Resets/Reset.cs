using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour {

    Vector3 initPos; // of block
    Quaternion initRot; // of block
    Transform initParent;
    Vector3 initVel;
    Rigidbody rb;

	// Use this for initialization
	void Start () {
        initPos = transform.localPosition;
        initRot = transform.localRotation;
        initParent = transform.parent;
        if (rb = GetComponent<Rigidbody>()) {
            initVel = rb.velocity;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void reset() {
        transform.parent = initParent;
        transform.localPosition = initPos;
        transform.localRotation = initRot;
        if (rb) {
            rb.velocity = initVel;
        }
    }
}
