using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour {

    Vector3 initPos; // of block
    Quaternion initRot; // of block

	// Use this for initialization
	void Start () {
        initPos = transform.localPosition;
        initRot = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void reset() {
        transform.localPosition = initPos;
        transform.localRotation = initRot;
    }
}
