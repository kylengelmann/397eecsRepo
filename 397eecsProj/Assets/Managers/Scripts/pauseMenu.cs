using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pauseMenu : MonoBehaviour {

    public bool isPlayer1;
    public Toggle invertY;
    playerController player;

    void Start() {
        foreach (playerController pc in Object.FindObjectsOfType<playerController>()) {
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
        else {
            toggle = false;
        }
    }

}
