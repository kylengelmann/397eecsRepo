using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

    public GameObject collectible;
    public int collectibleIndex;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Character>()) {
            Global.gameManager.gotCake[collectibleIndex] = true;
            Debug.Log(Global.gameManager.gotCake[0]);
            Destroy(gameObject); 
        }
    }
}
