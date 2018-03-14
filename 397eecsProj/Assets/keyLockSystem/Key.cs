using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
public class Key : MonoBehaviour {
    public enum KeyColor {
        gold
    }

    public KeyColor keyColor = KeyColor.gold;

    KeyColor lastColor;

    ParticleSystem pSystem;

	// Use this for initialization
	void Start () {
        pSystem = GetComponentInChildren<ParticleSystem>();
        lastColor = keyColor;
        changeColor();
	}

    void changeColor() {
        switch(keyColor) {
            case KeyColor.gold:
                Material mat = GetComponent<Renderer>().material;
                mat.color = keyColors.gold;
                if(pSystem != null) {
                    ParticleSystem.MainModule main = pSystem.main;
                    main.startColor = keyParticleColors.gold;
                }
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if(lastColor != keyColor) {
            changeColor();
        }
	}
}

public static class keyColors {
    public static Color gold = new Color(.765f, .675f, .18f);
}

public static class keyParticleColors {
    public static Color gold = new Color(.949f, .918f, .478f);
}