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
        private Rigidbody _rb;
        public float Speed;
        public float PushStrength;
        public float DetectionRadius;

        // Use this for initialization
        void Start ()
        {
            _player = FindObjectOfType<Character>().gameObject;
            _rb = GetComponent<Rigidbody>();
        }
	
        // Update is called once per frame
        void Update ()
        {
            Collider[] detection = Physics.OverlapSphere(transform.position, DetectionRadius);

            if (detection.Contains(_player.GetComponent<Collider>()))
            {
                Vector3 groundedPlayer = new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z);
                Vector3 towardsPlayer = (groundedPlayer - transform.position).normalized * Speed;
                _rb.velocity = new Vector3(towardsPlayer.x, 0, towardsPlayer.z);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<Character>())
            {
                _player.GetComponent<Character>().velocity =
                    (_player.transform.position - transform.position).normalized * PushStrength;

                //StartCoroutine(Push(_player.transform.position));
            }
        }

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
