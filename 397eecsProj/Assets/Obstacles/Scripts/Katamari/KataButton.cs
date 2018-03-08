using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KataButton : MonoBehaviour
{

    public float pressDist;
    private bool wasPressed;
    public int kataCount;

    public delegate void Response(bool onPlate);
    public Response onChange;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        bool nowPressed = false;
        Collider[] onPp = Physics.OverlapBox(transform.position, Vector3.one * 0.5f, transform.rotation);

        foreach (Collider c in onPp)
        {
            if (c.GetComponent<Katamari>() && c.transform.childCount >= kataCount)
            {
                nowPressed = true;
                break;
            }
        }

        if (nowPressed && !wasPressed)
        {
            Vector3 pressedPos = new Vector3(transform.position.x, transform.position.y - pressDist, transform.position.z);
            transform.position = pressedPos;
            onChange(true);
        }

        if (!nowPressed && wasPressed)
        {
            Vector3 neutralPos = new Vector3(transform.position.x, transform.position.y + pressDist, transform.position.z);
            transform.position = neutralPos;
            onChange(false);
        }

        wasPressed = nowPressed;
    }
}
