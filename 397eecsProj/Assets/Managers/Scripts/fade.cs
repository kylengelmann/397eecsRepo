using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fade : MonoBehaviour {

    [HideInInspector]public Animator anim;

    void Start() {
        anim = gameObject.GetComponent<Animator>();
        anim.enabled = false;
    }

    public void beginFade(){
        anim.enabled = true;
    }

    public void reset() {
        anim.enabled = false;
        Global.gameManager.reset();
    }
}
