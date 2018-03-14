using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Assets.Obstacles.Scripts
{
    public class Enemy : MonoBehaviour
    {
        private GameObject _player;
        //private Rigidbody _rb;
        private CharacterController charCtrl;
        public float Speed;
        public float PushStrength;
        public float DetectionRadius;

        public Vector3 velocity;

        // Use this for initialization
        void Start ()
        {
            _player = FindObjectOfType<Character>().gameObject;
            //_rb = GetComponent<Rigidbody>();
            charCtrl = GetComponent<CharacterController>();
        }
	
        // Update is called once per frame
        void FixedUpdate ()
        {
            Collider[] detection = Physics.OverlapSphere(transform.position, DetectionRadius);

            if (detection.Contains(_player.GetComponent<Collider>()))
            {
                Vector3 groundedPlayer = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
                Vector3 towardsPlayer = (groundedPlayer - transform.position).normalized * Speed;
                velocity = new Vector3(towardsPlayer.x, 0, towardsPlayer.z);
                charCtrl.Move(velocity*Time.fixedDeltaTime);
                transform.rotation = Quaternion.LookRotation(velocity, Vector3.up);

                Vector3 boxPos = transform.position + transform.right*0f + transform.up*0.25f + transform.forward*0.5f;
                Collider[] touched = Physics.OverlapBox(boxPos, new Vector3(0.375f, 0.25f, 0.1f), transform.rotation);
                foreach (Collider collider in touched) //Checks everything it collided with to see if any objects it detected are breakable
                {
                    if (collider.gameObject.GetComponent<Character>()) 
                    {
                        _player.GetComponent<Character>().velocity =
                                    (_player.transform.position - transform.position).normalized * PushStrength;
                    }
                }
            }


        }

        //void OnCollisionEnter(Collision collision)
        //{
        //    if (collision.gameObject.GetComponent<Character>())
        //    {
        //        _player.GetComponent<Character>().velocity =
        //            (_player.transform.position - transform.position).normalized * PushStrength;

        //        //StartCoroutine(Push(_player.transform.position));
        //    }
        //}

        private IEnumerator Push(Vector3 playerPosition)
        {
            float originalSpeed = Speed;
            Speed = 0;
            Vector3 start = playerPosition;
            Vector3 end = playerPosition + ((playerPosition - transform.position).normalized * PushStrength);
            float pushTime = 0.0f;


            while (pushTime < 1.0f)
            {
                pushTime += Time.deltaTime;

                _player.transform.position = (new Vector3(Mathf.Lerp(start.x, end.x, pushTime),
                                                                            Mathf.Lerp(start.y, end.y, pushTime),
                                                                            Mathf.Lerp(start.z, end.z, pushTime)));

                yield return null;
            }

            Speed = originalSpeed;
        }
    }
}
