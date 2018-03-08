using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faceTraverser : MonoBehaviour {
    public Vector3 worldRotation;
    public Transform camPos;
    public Transform cam;
    public faceTraverser linked;
    public GameObject Face;
    public GameObject world;
    public bool canEnter;

    void OnTriggerEnter(Collider other)
    {
        if(canEnter) {
            if(other.gameObject.GetComponent<Character>()) {
                cam.position = camPos.position;
                cam.rotation = camPos.rotation;
                Quaternion nextRot = Quaternion.Euler(linked.worldRotation.x, linked.worldRotation.y, linked.worldRotation.z);
                //other.gameObject.transform.position = linked.transform.position + linked.transform.up;
                Face.SetActive(false);
                Global.gameManager.switchFace(world.transform.rotation, nextRot, 
                                              linked.Face, linked.transform);
            }
        }
    }
}
