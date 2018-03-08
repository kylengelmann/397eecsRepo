using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KataChild : MonoBehaviour {

    public GameObject kataChild;
    public Vector3 kataChildPos;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        
	}

    private void FixedUpdate()
    {
        if (!transform.parent)
        {
            return;
        }
        else if (transform.parent.GetComponent<Katamari>())
        {
            transform.localPosition = kataChildPos;
        }
    }
}
