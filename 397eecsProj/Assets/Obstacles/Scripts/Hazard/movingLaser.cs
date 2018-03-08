using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingLaser : MonoBehaviour {

    public Transform start;
    public Transform end;
    public float speed;
    bool isMoving2Start;

    void Start()
    {
        //transform.position = start.position;
        isMoving2Start = false;
    }


    void FixedUpdate () {
        Vector3 goal = isMoving2Start ? start.position : end.position;
        transform.position = Vector3.MoveTowards(transform.position, goal, speed*Time.fixedDeltaTime);

        if(Vector3.SqrMagnitude(transform.position - goal) < 0.1f) {
            transform.position = start.position;
        }

	}
}
