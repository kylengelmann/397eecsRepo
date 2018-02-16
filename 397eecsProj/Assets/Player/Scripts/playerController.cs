﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class playerController : MonoBehaviour {
// Component for player specific (rather than character specific) stuff
// Checks inputs, stores player specific actions, interacts with Character component

	public bool isPlayer1;
    public playerController otherPlayer;
    [HideInInspector] public bool invertY;
    [HideInInspector] public bool isMovingPlayer;
	// Holds button/axis names
	public struct Buttons {
		public string xAxis;
		public string yAxis;
		public string pause;
		public string switchControl;
        public string actionAxis03;
        public string actionAxis12;
	};
	[HideInInspector] public Buttons buttons;

	//The character component of the gameobject
	Character character;



	//Player specific actions
	public delegate void Action(bool isPressed);
	public Action action0;
    public Action action1;
    public Action action2;
    public Action action3;

	void Start () {

        string platform = "";
        string joyNum = "_Key";
        int numControllers = Input.GetJoystickNames().Length;
        if(numControllers > 0) {
            platform = "Mac";
            if(Application.platform == RuntimePlatform.WindowsEditor 
               || Application.platform == RuntimePlatform.WindowsPlayer) {
                platform = "Win";
            }
            if(numControllers > 1 && !isPlayer1) {
                joyNum = "_J2";
            }
            else {
                joyNum = "_J1";
            }
        }
        if(numControllers > 1) {
            buttons.xAxis = "LeftHorizontalJoystick" + joyNum;
            buttons.yAxis = "LeftVerticalJoystick" + joyNum;
            buttons.pause = "StartButton" + platform + joyNum;
            buttons.actionAxis03 = "AY" + platform + joyNum;
            buttons.actionAxis12 = "XB" + platform + joyNum;
            buttons.switchControl = "RightTrigger" + platform + joyNum;
        }
        else {
            if(isPlayer1) {
                buttons.xAxis = "LeftHorizontalJoystick" + joyNum;
                buttons.yAxis = "LeftVerticalJoystick" + joyNum;
                buttons.pause = "SelectButton" + platform + joyNum;
                buttons.actionAxis03 = "DPadVertical" + platform + joyNum;
                buttons.actionAxis12 = "DPadHorizontal" + platform + joyNum;
                buttons.switchControl = "LeftTrigger" + platform + joyNum;
            }
            else {
                buttons.xAxis = "RightHorizontalJoystick" + platform + joyNum;
                buttons.yAxis = "RightVerticalJoystick" + platform + joyNum;
                buttons.pause = "StartButton" + platform + joyNum;
                buttons.actionAxis03 = "AY" + platform + joyNum;
                buttons.actionAxis12 = "XB" + platform + joyNum;
                buttons.switchControl = "RightTrigger" + platform + joyNum;
            }
        }

		isMovingPlayer = isPlayer1; //Default to start with Player 1 in control

		character = gameObject.GetComponent<Character>();
		if(!isPlayer1) {
			action0 = character.jump;
		}
        else {
            action0 = character.run;
        }
	}
		

	void Update () {
		//Check input and such

        //Switch if an appropriate trigger is pressed
		if(!isMovingPlayer && Input.GetAxisRaw(buttons.switchControl) >= 0.5f) {
            switchPlayers();
		}
        if (action0 != null) {
            action0((Input.GetAxisRaw(buttons.actionAxis03) < -0.5f) && !isMovingPlayer);
		}
        if(action1 != null) {
            action1((Input.GetAxisRaw(buttons.actionAxis12) < -0.5f) && !isMovingPlayer);
        }
        if(action2 != null) {
            action2((Input.GetAxisRaw(buttons.actionAxis12) > 0.5f) && !isMovingPlayer);
        }
        if (action3 != null) {
            action3((Input.GetAxisRaw(buttons.actionAxis03) > 0.5f) && !isMovingPlayer);
        }
		if(Input.GetButtonDown(buttons.pause)) {
			Global.gameManager.togglePause();
		}

	    handleAxes(Input.GetAxisRaw(buttons.xAxis), (Input.GetAxisRaw(buttons.yAxis)));
    }



	public void handleAxes(float x, float y) {
        if(x*x + y*y < 0.04f) {
            x = y = 0f;
        }
		if(isMovingPlayer) {
			character.setMove(x, y);
		}
        else {
            if(invertY) y = -y;
            character.setCam(x, y);
        }
	}

	void switchPlayers() {
        if(character.switchPlayers()) { //TODO Disable camera and such, keep momentum in air
            otherPlayer.isMovingPlayer = isMovingPlayer;
            isMovingPlayer = !isMovingPlayer;
        }
	}
}
