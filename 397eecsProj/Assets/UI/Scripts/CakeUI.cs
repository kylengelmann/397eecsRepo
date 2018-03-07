using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CakeUI : MonoBehaviour {

    public GameObject cakes;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < 6; i++) {
            Transform cakeT = cakes.transform.GetChild(i);
            cakeT.gameObject.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < 6; i++)
        {
            if (Global.gameManager.gotCake[i])
            {
                Transform gotCakeT = cakes.transform.GetChild(i);
                gotCakeT.gameObject.SetActive(true);
            }
        }
	}

}
