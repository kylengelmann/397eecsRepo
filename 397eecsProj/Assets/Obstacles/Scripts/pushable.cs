using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pushable : MonoBehaviour {

    BoxCollider box;
    RaycastHit hit;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Vector3 push(Vector3 velocity, float dt) {
        if(Physics.BoxCast(box.center, box.size/2f, velocity, out hit, transform.rotation, 
                           velocity.magnitude*dt, ~(1 << LayerMask.NameToLayer("Player")))) {
            if(Vector3.Dot(hit.normal, velocity) > 0f) {
                return Vector3.zero;
            }
        }
        transform.Translate(velocity*dt);
        return velocity;
    }
}
