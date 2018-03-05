using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katamari : MonoBehaviour {

    public Rigidbody kataSphere;
    public float kataSphereRad;
    public float velTransfer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        Collider[] ktColl = Physics.OverlapSphere(kataSphere.transform.position, kataSphereRad+0.2f);
        foreach (Collider c in ktColl) {
            if (c.GetComponent<Character>())
            {
                Vector3 sphNorm = (kataSphere.transform.position - c.transform.position);
                sphNorm.y = 0f;
                sphNorm = sphNorm.normalized;
                float sphSpeed = Mathf.Max(Vector3.Dot(c.GetComponent<Character>().velocity, sphNorm), 0f);
                kataSphere.velocity += sphNorm * sphSpeed * velTransfer;
            }
            else if (c.GetComponent<KataChild>()) {
                if (c.transform.parent != transform) {
                    c.transform.SetParent(kataSphere.transform);
                    c.GetComponent<KataChild>().kataChildPos = c.transform.localPosition;
                }
            }
        }
    }


}
