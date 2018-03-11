using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class pauseMenu : MonoBehaviour {

    public bool isPlayer1;
    public Toggle invertY;
    playerController player;

    public static int mainMenuScene = 0;

    void Start() {
        foreach (playerController pc in FindObjectsOfType<playerController>()) {
            if(pc.isPlayer1 == isPlayer1) player = pc;
        }
        invertY.isOn = player.invertY;
    }

    //void OnEnable()
    //{
    //    invertY.isOn = player.invertY;
    //}

    bool toggle = false;

    void Update()
    {
        if(Input.GetButtonDown(player.buttons.pause)) {
            Global.gameManager.togglePause();
        }
        if(Input.GetAxisRaw(player.buttons.actionAxis03) < -0.5f) {
            if(!toggle){
                invertY.isOn = player.invertY = !player.invertY;
                toggle = true; 
            }
        }
        if(Input.GetAxisRaw(player.buttons.switchControl) > 0.5f) {
            SceneManager.LoadScene(mainMenuScene);
        }
        else {
            toggle = false;
        }
    }
}
