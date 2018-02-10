using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class playerController : MonoBehaviour {
// Component for player specific (rather than character specific) stuff
// Checks inputs, stores player specific actions, interacts with Character component

	public bool isPlayer1;
	[HideInInspector] public bool isMovingPlayer; //Is this player moving the character?

	// Holds button/axis names
	struct Buttons {
		public string xAxis;
		public string yAxis;
		public string pause;
		public string switchPlayers;
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
		buttons.xAxis = "horizontalUnix";
		buttons.yAxis = "verticalUnix";
		buttons.pause = "pauseUnix";
		buttons.switchPlayers = "switchUnix";
		buttons.action0 = "action0Unix";


		isMovingPlayer = isPlayer1;

		character = gameObject.GetComponent<Character>();
		if(!isPlayer1) {
			action0 = character.jump;
		}
	}
		

	void Update () {
		//Check input and such
		handleAxes(Input.GetAxisRaw(buttons.xAxis), Input.GetAxisRaw(buttons.yAxis));
		if(Input.GetButtonDown(buttons.switchPlayers)) { //I realize this is not quite how this is gonna work
			switchPlayers();
		}
		if(action0 != null) {
			action0(buttons.action0);
		}
		if(Input.GetButtonDown(buttons.pause)) {
			Global.gameManager.togglePause();
		}
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
		character.switchPlayers();
	}

}
