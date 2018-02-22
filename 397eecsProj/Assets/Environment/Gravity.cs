using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {

    [HideInInspector] public Vector3 revGrav = Vector3.up;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        revGrav = findRevGrav(transform.position);
	}

    public static Vector3 findRevGrav(Vector3 pos)
    { // find reverse gravity
        Vector3 dir = Vector3.zero;
        if (Mathf.Abs(pos.x) >= Mathf.Abs(pos.y) && Mathf.Abs(pos.x) >= Mathf.Abs(pos.z))
        {
            if (pos.x >= 0f)
            {
                dir = Vector3.right; // +x
            }
            else
            {
                dir = Vector3.left; // -x
            }
        }
        else if (Mathf.Abs(pos.y) >= Mathf.Abs(pos.x) && Mathf.Abs(pos.y) >= Mathf.Abs(pos.z))
        {
            if (pos.y >= 0f)
            {
                dir = Vector3.up; // +y
            }
            else
            {
                dir = Vector3.down; // -y
            }
        }
        else
        {
            if (pos.z >= 0f)
            {
                dir = Vector3.forward; // +z
            }
            else
            {
                dir = Vector3.back; // -z
            }
        }

        return dir;
    }
}
