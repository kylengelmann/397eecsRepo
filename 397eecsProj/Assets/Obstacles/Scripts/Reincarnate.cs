using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reincarnate : MonoBehaviour {

    public void reset() {
        int numChilds = transform.childCount;
        for(int i = 0; i < numChilds; i++) {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}
