using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

[ExecuteInEditMode]
public class Laser : MonoBehaviour {

    public bool isOn = true;
    public float length = 4f;
    float lastLength;
    public float radius = .2f;
    float lastRadius;
    public LineRenderer lineRenderer;
    CapsuleCollider cap;
    public Transform[] endCaps;


    void Start() {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 4;
        lineRenderer.useWorldSpace = false;
        cap = gameObject.GetComponent<CapsuleCollider>();
        cap.center = Vector3.zero;
        cap.direction = 0;
    }

    void Update()
    {
        lineRenderer.enabled = isOn;
        if(!Mathf.Approximately(length, lastLength)) {
            lineRenderer.SetPosition(0, Vector3.left*(length/2f));
            lineRenderer.SetPosition(1, Vector3.left*(length/2f - radius));
            lineRenderer.SetPosition(2, Vector3.right*(length/2f - radius));
            lineRenderer.SetPosition(3, Vector3.right*(length/2f));

            endCaps[0].localPosition = Vector3.left*((length + endCaps[0].localScale.x)/2f);
            endCaps[1].localPosition = Vector3.right*((length + endCaps[0].localScale.x)/2f);

            cap.height = length;
            lastLength = length;
        }
        if(!Mathf.Approximately(radius, lastRadius)) {
            lineRenderer.widthMultiplier = radius*2f;
            lineRenderer.SetPosition(1, Vector3.left*(length/2f - radius));
            lineRenderer.SetPosition(2, Vector3.right*(length/2f - radius));
            cap.radius = radius*.5f;
            lastRadius = radius;
        }
    }

    //void OnTriggerEnter(Collider other)
    //{   if(!isOn) return;
    //    Character c;
    //    if((c = other.gameObject.GetComponent<Character>()) != null) {
    //        c.enabled = false;
    //        Global.gameManager.die();
    //    }
    //}

    void OnTriggerStay(Collider other)
    {
        if(!isOn) return;
        Character c;
        if((c = other.gameObject.GetComponent<Character>()) != null) {
            c.enabled = false;
            Global.gameManager.die();
        }
    }

    public void warn(float seconds, float frequency) {
        StartCoroutine(__warn(seconds, frequency));
    }

    IEnumerator __warn(float seconds, float frequency) {
        float time = 0f;
        float timeBetween = 1f/frequency;
        while(time < seconds) {
            lineRenderer.enabled = !lineRenderer.enabled;
            yield return new WaitForSeconds(timeBetween);
            time += timeBetween;
        }
        isOn = true;
    }


}
