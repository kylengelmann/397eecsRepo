using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserWallActivater : MonoBehaviour {

    public laserWall[] laserWalls;
    public PressurePlate pressurePlate;
    public bool onPressurePlateDown;
    bool activated;
	// Use this for initialization
	void Start () {
        if(pressurePlate) {
            pressurePlate.onChange = turnOnWalls;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Character>()){
            turnOnWalls(onPressurePlateDown);
        }
    }

    void turnOnWalls(bool activate){
        if(!activated && activate == onPressurePlateDown) {
            activated = true;
            foreach(laserWall lw in laserWalls) {
                lw.turnOn();
            }
        }
    }

    void reset() {
        activated = false;
    }
}
