using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public bool isPaused = false;

	public void togglePause() {
	/*  TODO
	 * 	Create pause menu and implement pausing
	 */ 
		isPaused = !isPaused;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
