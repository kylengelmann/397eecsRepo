using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject pauseMenu;
	[HideInInspector]public bool isPaused = false;
    public Character character;

	public void togglePause() {
		isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        if(Time.timeScale > 0.1f) Time.timeScale = 0f;
        else Time.timeScale = 1f;
        foreach (playerController pc in Object.FindObjectsOfType<playerController>()) {
            pc.enabled = !pc.enabled;
        }
	}

	// Use this for initialization
	void Start () {
        Global.gameManager = this;
	}
	
	// Update is called once per frame
	void Update () {
        if(character.gameObject.transform.position.y < -5f) {
            character.transform.position = new Vector3(0f, 1.08f, 0f);
            character.velocity = Vector3.zero;
        }
	}
}
