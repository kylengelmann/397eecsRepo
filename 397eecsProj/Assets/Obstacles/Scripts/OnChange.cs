using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnChange : MonoBehaviour {

    public PressurePlate pp;
    public GameObject cube;

	// Use this for initialization
	void Start () {
        pp.onChange = onOff;
        cube.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onOff(bool onPress) {
        if (onPress) {
            cube.SetActive(onPress);
        }
        else {
            cube.SetActive(onPress);
        }
    }
}
