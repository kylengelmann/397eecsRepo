using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Lock))]
public class lockedGate : MonoBehaviour {
    [HideInInspector] public Lock gateLock;
    public float timeToOpen;
    Vector3 startPos;
    Vector3 startScale;
	// Use this for initialization
	void Start () {
        gateLock = GetComponent<Lock>();
        gateLock.onOpen = onOpen;

        startScale = transform.localScale;
        startPos = transform.localPosition;
	}

    public void onOpen() {
        StartCoroutine(open());
    }
	
    IEnumerator open() {
        float time = 0f;
        Vector3 goalScale = startScale;
        goalScale.y = 0f;
        Vector3 goalPos = startPos;
        goalPos += Vector3.down*startScale.y/2;

        while(time < timeToOpen) {
            transform.localPosition = Vector3.Lerp(startPos, goalPos, time/timeToOpen);
            transform.localScale = Vector3.Lerp(startScale, goalScale, time/timeToOpen);
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        gameObject.SetActive(false);
    }

	void reset()
	{
        transform.localPosition = startPos;
        transform.localScale = startScale;
	}


}
