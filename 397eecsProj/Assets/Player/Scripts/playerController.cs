using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class playerController : MonoBehaviour {
// Component for player specific (rather than character specific) stuff
// Checks inputs, stores player specific actions, interacts with Character component

	public bool isPlayer1;
	[HideInInspector] public bool MovingPlayer; //Which player is controlling the player's movement? false for Player 1, true for Player 2
    private const bool Player1 = false;
    private const bool Player2 = true;

	// Holds button/axis names
	struct Buttons {
		public string xAxis;
		public string yAxis;
		public string pause;
		public string playerOneControl; //Left Trigger
	    public string playerTwoControl; //Right Trigger
	    public string pausePlayerOne;
	    public string pausePlayerTwo;
	    public string AButton;
	    public string BButton;
	    public string XButton;
	    public string YButton;
        //For Windows
	    public string DPadHorizontal;
        public string DPadVertical;
        //For Mac
	    public string DPadUp;
	    public string DPadDown;
        public string DPadLeft;
	    public string DPadRight;

        public string action0;
	};
	Buttons buttons;

	//The character component of the gameobject
	Character character;



	//Player specific actions
	public delegate void Action0(string buttonName);
	public Action0 action0;

	void Start () {
	/*  TODO
	 *  Check OS, make different inputs for p1 and p2 and for different controllers,
	 *  set strings accordingly, actually set inputs to right buttons/axes
	 */
		buttons.xAxis = "LeftHorizontalJoystick";
		buttons.yAxis = "LeftVerticalJoystick";
		buttons.pause = "pauseUnix";
	    if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
	    {
	        buttons.playerOneControl = "LeftTriggerMac";
	        buttons.playerTwoControl = "RightTriggerMac";
	        buttons.AButton = "AButtonMac";
	        buttons.BButton = "BButtonMac";
	        buttons.XButton = "XButtonMac";
	        buttons.YButton = "YButtonMac";
	        buttons.pausePlayerOne = "SelectButtonMac";
	        buttons.pausePlayerTwo = "StartButtonMac";
	    }
	    else
	    {
	        buttons.playerOneControl = "LeftTriggerWin";
	        buttons.playerTwoControl = "RightTriggerWin";
	        buttons.AButton = "AButtonWin";
	        buttons.BButton = "BButtonWin";
	        buttons.XButton = "XButtonWin";
	        buttons.YButton = "YButtonWin";
	        buttons.pausePlayerOne = "SelectButtonWin";
	        buttons.pausePlayerTwo = "StartButtonWin";
	    }
	    buttons.DPadHorizontal = "DPadHorizontal";
	    buttons.DPadVertical = "DPadVertical";
	    buttons.DPadUp = "DPadUp";
	    buttons.DPadDown = "DPadDown";
	    buttons.DPadLeft = "DPadLeft";
	    buttons.DPadRight = "DPadRight";
		buttons.action0 = "action0Unix";


		MovingPlayer = Player1; //Default to start with Player 1 in control

		character = gameObject.GetComponent<Character>();
		if(!isPlayer1) {
			action0 = character.jump;
		}
	}
		

	void Update () {
		//Check input and such

        //Switch if an appropriate trigger is pressed
		if(MovingPlayer == Player1 && Input.GetAxisRaw(buttons.playerTwoControl) == 1) {
            switchPlayers();
		}
	    else if (MovingPlayer == Player2 && Input.GetAxisRaw(buttons.playerOneControl) == 1)
	    {
	        switchPlayers();
	    }
        if (action0 != null) {
			action0(buttons.action0);
		}
		if(Input.GetButtonDown(buttons.pausePlayerOne) || Input.GetButtonDown(buttons.pausePlayerTwo)) {
			Global.gameManager.togglePause();
		}

	    handleAxes(Input.GetAxisRaw(buttons.xAxis), (-1 * Input.GetAxisRaw(buttons.yAxis)));
    }



	public void handleAxes(float x, float y) {
		if(isMovingPlayer) {
			character.setMove(x, y);
		}
        //else {
        //    character.setCam(x, y);
        //}
	}

	public void switchPlayers() {
	    if (MovingPlayer == Player1)
	    {
	        if (Application.platform == RuntimePlatform.OSXEditor ||
	            Application.platform == RuntimePlatform.OSXPlayer)
	        {
	            buttons.xAxis = "RightHorizontalJoystickMac";
	            buttons.yAxis = "RightVerticalJoystickMac";
	        }
	        else
	        {
	            buttons.xAxis = "RightHorizontalJoystickWin";
	            buttons.yAxis = "RightVerticalJoystickWin";
	        }

	        MovingPlayer = Player2;
	    }
	    else
	    {
	        buttons.xAxis = "LeftHorizontalJoystick";
	        buttons.yAxis = "LeftVerticalJoystick";

	        MovingPlayer = Player1;
	    }
	}

}
