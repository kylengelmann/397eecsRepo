using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserTimer : MonoBehaviour {

    public float onTime = 1f;
    public float offTime = 3f;
	// Use this for initialization
	void Start () {
		StartCoroutine("switchOff");
	}

    IEnumerator switchOn() {
        foreach(Laser laser in gameObject.GetComponentsInChildren<Laser>()) {
            laser.isOn = true;
        }
        yield return new WaitForSeconds(onTime);
        StartCoroutine("switchOff");
    }

    IEnumerator switchOff() {
        foreach(Laser laser in gameObject.GetComponentsInChildren<Laser>()) {
            laser.isOn = false;
        }
        yield return new WaitForSeconds(offTime);
        StartCoroutine("switchOn");
    }

}
