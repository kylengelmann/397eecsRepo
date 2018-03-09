using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserWall : MonoBehaviour {
    
    public enum MovingBehavior {
        noMove,
        oneWay,
        oneWayTurnOff,
        twoWay,
        oneWayReset
    };

    public MovingBehavior movingBehavior;
    public Transform start;
    public Transform end;
    public float speed;
    public bool isOn;
    Vector3 goal;

	// Use this for initialization
	void Start () {
        goal = end.position;
        //foreach(Laser laser in gameObject.GetComponentsInChildren<Laser>()) {
        //    laser.isOn = isOn;
        //}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if(isOn && movingBehavior != MovingBehavior.noMove) {
            transform.position = Vector3.MoveTowards(transform.position, goal, speed*Time.fixedDeltaTime);
            if(movingBehavior == MovingBehavior.oneWayTurnOff) {
                foreach(Laser laser in gameObject.GetComponentsInChildren<Laser>()) {
                    laser.isOn = false;
                }
                isOn = false;
            }
            else if(movingBehavior != MovingBehavior.oneWay && Vector3.SqrMagnitude(transform.position - goal) < 0.001f) {
                if(movingBehavior == MovingBehavior.oneWayReset) {
                    transform.position = start.position;
                }
                else if(Vector3.SqrMagnitude(goal - start.position) < 0.001f){
                    goal = end.position;
                }
                else {
                    goal = start.position;
                }
            }
        }
	}

    public void turnOn() {
        foreach(Laser laser in gameObject.GetComponentsInChildren<Laser>()) {
            laser.isOn = true;
        }

        isOn = true;
    }

    void reset()
    {
        foreach(Laser laser in gameObject.GetComponentsInChildren<Laser>()) {
            laser.isOn = false;
        }

        isOn = false;
    }
}
