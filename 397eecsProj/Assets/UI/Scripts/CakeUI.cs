using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CakeUI : MonoBehaviour {

    public GameObject cakes;
    public Sprite grayCake;
    public Sprite gotCake;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < 6; i++) {
            Transform cakeT = cakes.transform.GetChild(i);
            //cakeT.gameObject.SetActive(false);
            Image cakeImg = cakeT.gameObject.GetComponent<Image>();
            cakeImg.sprite = grayCake;
        }
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < 6; i++)
        {
            if (Global.gameManager.gotCake[i])
            {
                Transform gotCakeT = cakes.transform.GetChild(i);
                //gotCakeT.gameObject.SetActive(true);
                Image cakeImg = gotCakeT.gameObject.GetComponent<Image>();
                cakeImg.sprite = gotCake;
            }
        }
	}

}
