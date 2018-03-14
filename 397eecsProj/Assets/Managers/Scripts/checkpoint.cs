using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour {

    public ParticleSystem fireworks;
    public bool hasBeenTriggered;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) 
    {
        if(!hasBeenTriggered){
            Character chara;
            if (chara = other.gameObject.GetComponent<Character>()) {
                chara.lastCkpt = this;
                if(fireworks) {
                    fireworks.Play();
                }
                hasBeenTriggered = true;
            }
        }
    }
}
