using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

[ExecuteInEditMode]
public class Lock : MonoBehaviour {

    public Key.KeyColor lockColor;
    Key.KeyColor lastColor;

    public delegate void OpenLock();
    public OpenLock onOpen;

	// Use this for initialization
	void Start () {
        lastColor = lockColor;
        changeColor();
	}

    void changeColor() {
        switch(lockColor) {
            case Key.KeyColor.gold:
                Material mat = GetComponent<Renderer>().material;
                mat.color = keyColors.gold;
                break;
        }
    }

    // Update is called once per frame
    void Update () {
        if(lastColor != lockColor) {
            changeColor();
        }

    }

	void OnCollisionEnter(Collision collision)
	{
        Key key;
        if(key = collision.gameObject.GetComponent<Key>()) {
            if(key.keyColor == lockColor && onOpen != null) {
                key.gameObject.SetActive(false);
                onOpen();
            }
        }
	}
}
