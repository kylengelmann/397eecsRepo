using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour {

    public ParticleSystem fireworks;
    [HideInInspector] public bool hasTriggered = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) 
    {
        if(!hasTriggered) {
            Character chara;
            if (chara = other.gameObject.GetComponent<Character>()) {
                chara.lastCkpt = this;
                fireworks.Play();
                hasTriggered = true;
            }
        }
    }
}
