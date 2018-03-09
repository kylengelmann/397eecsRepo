using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laserBlinker : MonoBehaviour {

    public float onTime = 1f;
    public float offTime = 4f;
    public float warnTime = 1f;
    public float warnFrequency = 10f;
	// Use this for initialization
	void Start () {
		StartCoroutine(switchOff());
	}

    IEnumerator switchOn() {
        foreach(Laser laser in gameObject.GetComponentsInChildren<Laser>()) {
            laser.isOn = true;
        }
        yield return new WaitForSeconds(onTime);
        StartCoroutine(switchOff());
    }

    IEnumerator switchOff() {
        foreach(Laser laser in gameObject.GetComponentsInChildren<Laser>()) {
            laser.isOn = false;
        }
        yield return new WaitForSeconds(offTime);

        StartCoroutine(warn());
    }

    IEnumerator warn() {
        float time = 0f;
        float timeBetween = 1f/warnFrequency;
        while(time < warnTime) {
            foreach(Laser laser in gameObject.GetComponentsInChildren<Laser>()) {
                laser.lineRenderer.enabled = !laser.lineRenderer.enabled;
            }
            yield return new WaitForSeconds(timeBetween);
            time += timeBetween;
        }
        StartCoroutine(switchOn());
    }

}
