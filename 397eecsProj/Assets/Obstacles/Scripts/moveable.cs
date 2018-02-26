using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveable : MonoBehaviour {

    public bool isMoveable;
    public Transform[] faceTs;
    public Transform defaultGrab;
    Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = gameObject.GetComponent<Rigidbody>();
	}

    private void FixedUpdate()
    {
        RaycastHit trash;
        if (!rb.SweepTest(Vector3.down, out trash, 0.08f)) {
            rb.AddForce(Vector3.down*100f, ForceMode.Acceleration);
        }
    }

}
