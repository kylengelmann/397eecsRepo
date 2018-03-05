using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KataOnChange : MonoBehaviour
{

    public KataButton kb;
    public GameObject cube;

    // Use this for initialization
    void Start()
    {
        kb.onChange = kataOnOff;
        cube.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void kataOnOff(bool onPress)
    {
        if (onPress)
        {
            cube.SetActive(onPress);
        }
        else
        {
            cube.SetActive(onPress);
        }
    }
}
