using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telepad : MonoBehaviour
{

    public Telepad PairedTelepad;
    public bool IsEntrance;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if (IsEntrance && collider.gameObject.GetComponent<Character>())
        {
            //TODO Add in a fade or some form of animation during teleportation

            Vector3 endPosition = new Vector3(PairedTelepad.gameObject.transform.position.x,
                                              PairedTelepad.gameObject.transform.position.y + 1.0f, 
                                              PairedTelepad.gameObject.transform.position.z);
            collider.gameObject.transform.position = endPosition;
        }
    }
}
