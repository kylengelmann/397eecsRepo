using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class playerController : MonoBehaviour {
// Component for player specific (rather than character specific) stuff
// Checks inputs, stores player specific actions, interacts with Character component

	public bool isPlayer1;
    public playerController otherPlayer;
    [HideInInspector] public bool invertY; // cam ctrl (pause)
    [HideInInspector] public bool isMovingPlayer;
	// Holds button/axis names
	public struct Buttons {
		public string xAxis;
		public string yAxis;
	    public string xAxisCam;
	    public string yAxisCam;
		public string pause;
		public string switchControl;
        public string actionAxis03; // for cross platform
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
        string controller = "";
        string[] joys = Input.GetJoystickNames();
        int numControllers = joys.Length;
        if(numControllers > 0) {
            if(joys[0].IndexOf("Joy-Con") >= 0) {
                controller = "_Joycon";
            }
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
            buttons.xAxis = "LeftHorizontalJoystick" + platform + joyNum + controller;
            buttons.yAxis = "LeftVerticalJoystick" + platform + joyNum + controller;
            buttons.xAxisCam = "RightHorizontalJoystick" + platform + joyNum + controller;
            buttons.yAxisCam = "RightVerticalJoystick" + platform + joyNum + controller;
            buttons.pause = "StartButton" + platform + joyNum + controller;
            buttons.actionAxis03 = "AY" + platform + joyNum + controller;
            buttons.actionAxis12 = "XB" + platform + joyNum + controller;
            buttons.switchControl = "RightTrigger" + platform + joyNum + controller;
        }
        else {
            if(isPlayer1) {
                buttons.xAxis = "LeftHorizontalJoystick" + platform + joyNum;
                buttons.yAxis = "LeftVerticalJoystick" + platform + joyNum;
                buttons.xAxisCam = buttons.xAxis;
                buttons.yAxisCam = buttons.yAxis;
                buttons.pause = "SelectButton" + platform + joyNum;
                buttons.actionAxis03 = "DPadVertical" + platform + joyNum;
                buttons.actionAxis12 = "DPadHorizontal" + platform + joyNum;
                buttons.switchControl = "LeftTrigger" + platform + joyNum;
            }
            else {
                buttons.xAxis = "RightHorizontalJoystick" + platform + joyNum;
                buttons.yAxis = "RightVerticalJoystick" + platform + joyNum;
                buttons.xAxisCam = buttons.xAxis;
                buttons.yAxisCam = buttons.yAxis;
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
		    action1 = character.breakObject;
		}
        else {
            action0 = character.run;
		    action1 = character.moveObject;
		}
	}
		

	void Update () {
		//Check input and such

        //Switch if an appropriate trigger is pressed
		if(Input.GetAxisRaw(buttons.switchControl) >= 0.5f) {
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

        if(isMovingPlayer) {
            handleMove(Input.GetAxisRaw(buttons.xAxis), (Input.GetAxisRaw(buttons.yAxis)));
        }
        else {
            handleCam(Input.GetAxisRaw(buttons.xAxis), (Input.GetAxisRaw(buttons.yAxis)));
        }

    }



	public void handleMove(float x, float y) {
        if(x*x + y*y < 0.04f) {
            x = y = 0f;
        }
		character.setMove(x, y);
	}

    public void handleCam(float x, float y) {
        if(x*x + y*y < 0.04f) {
            x = y = 0f;
        }
        if(invertY) y = -y;
        character.setCam(x, y);
    }

	void switchPlayers() {
        if(character.switchPlayers()) {
            otherPlayer.isMovingPlayer = isMovingPlayer;
            isMovingPlayer = !isMovingPlayer;
        }
	}
}
