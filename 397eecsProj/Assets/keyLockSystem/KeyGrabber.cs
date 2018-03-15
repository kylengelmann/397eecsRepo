using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyGrabber : MonoBehaviour {

    [HideInInspector]public bool isGrabbing;
    Rigidbody grabbedKey;
    public Transform goalKeyPos;
    public float grabRadius;
    public float grabForceMultiplier;
    public float damping;
    public Vector2 droppedVel;

    Collider[] inRadius;

    int layerMask;

	void Start()
	{
        inRadius = new Collider[8];
        layerMask = 1<<LayerMask.NameToLayer("Key");
	}

	void Update()
	{
        if(isGrabbing && grabbedKey == null) {
            int numInRadius = Physics.OverlapSphereNonAlloc(transform.position, 
                                                            grabRadius, inRadius, layerMask);

            for(int i = 0; i < numInRadius; i++) {
                if(inRadius[i].GetComponent<Key>()) {
                    grabbedKey = inRadius[i].GetComponent<Rigidbody>();
                    break;
                }
            }
        }
        else if (!isGrabbing && grabbedKey != null) {
            grabbedKey.velocity = transform.forward*(-droppedVel.x) + transform.up*(droppedVel.y);
            grabbedKey = null;
        }
	}

	void FixedUpdate()
	{
        if(grabbedKey)
        {
            grabbedKey.constraints = RigidbodyConstraints.None;
            Vector3 grabForce = (goalKeyPos.position - grabbedKey.position)*grabForceMultiplier;
            Vector3 dragForce = grabbedKey.velocity*(-damping);
            grabbedKey.AddForce(grabForce + dragForce);
        }
	}


}
