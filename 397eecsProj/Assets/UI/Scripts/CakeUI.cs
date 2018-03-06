using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CakeUI : MonoBehaviour {

    public Sprite[] cakeCollection;

	// Use this for initialization
	void Start () {
        cakeCollection = new Sprite[6];
        //for (int i = 0; i < 6; i++) {
        //    cakeCollection[i].SpriteRenderer.enabled = false;
        //    return;
        //}
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < 6; i++) {
            if (Global.gameManager.gotCake[i]) {
                // get corresponding icon and display it
            }
        }
	}
}
