using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sideLasers : MonoBehaviour {
    public Transform endPos;
    public float speed;
    bool isOn = false;
    Vector3 startPos;
    // Use this for initialization
    void Start () {
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate () {
        if(isOn){
            transform.position = Vector3.MoveTowards(transform.position, endPos.position, speed*Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Character>()){
            if(!isOn) {
                isOn = true;
                foreach(Laser laser in gameObject.GetComponentsInChildren<Laser>()) {
                    laser.isOn = true;
                }
            }
        }
    }

    void reset()
    {
        foreach(Laser laser in gameObject.GetComponentsInChildren<Laser>()) {
            laser.isOn = false;
        }

        isOn = false;

        transform.localPosition = startPos;
    }
}
