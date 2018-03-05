using System.Collections;
using UnityEngine;

namespace Assets.Obstacles.Scripts
{
    public class FallingPlatform : MonoBehaviour
    {
        public float Speed;
        private Rigidbody _rb;

        // Use this for initialization
        void Start ()
        {
            _rb = GetComponent<Rigidbody>();
        }
	
        // Update is called once per frame
        void Update () {
		
        }

        void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.GetComponent<Character>())
            {
                _rb.useGravity = true;
                //StartCoroutine(Fall());
            }
        }

        private IEnumerator Fall()
        {
            
            yield return null;
        }
    }
}
