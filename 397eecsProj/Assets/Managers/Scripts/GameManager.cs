using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject pauseMenu;
	[HideInInspector]public bool isPaused = false;

	public void togglePause() {
		isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
	}

	// Use this for initialization
	void Start () {
        Global.gameManager = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
