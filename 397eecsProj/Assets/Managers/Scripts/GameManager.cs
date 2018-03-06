using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject pauseMenu;
	[HideInInspector]public bool isPaused = false;
    public Character character;
    public fade fadePanel;
    public GameObject world;
    public bool[] gotCake;

	public void togglePause() {
		isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        if(Time.timeScale > 0.1f) Time.timeScale = 0f;
        else Time.timeScale = 1f;
        foreach (playerController pc in Object.FindObjectsOfType<playerController>()) {
            pc.enabled = !pc.enabled;
        }
	}

    public void die() {
        fadePanel.gameObject.SetActive(true);
        fadePanel.beginFade();
    }

    public void reset() {
        fadePanel.gameObject.SetActive(false);
        character.enabled = true;
        character.reset();  
        world.BroadcastMessage("reset", SendMessageOptions.DontRequireReceiver); // calls all the resets
    }

    void Awake()
    {
        pauseMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    void Start () {
        Global.gameManager = this;
        gotCake = new bool[6];
        for(int i = 0; i < 6; i++){
            gotCake[i] = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(character.gameObject.transform.position.y < -5f) {
            //character.transform.position = new Vector3(0f, 1.08f, 0f);
            //character.velocity = Vector3.zero;
            //character.reset();
            die();
        }
	}
}
