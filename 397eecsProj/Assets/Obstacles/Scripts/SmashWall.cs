using System.Collections;
using UnityEngine;

namespace Assets.Obstacles.Scripts
{
    public class SmashWall : MonoBehaviour
    {
        public bool IsOut;
        private Vector3 _safePosition;
        public Vector3 OutPosition;
        public float Speed;

        // Use this for initialization
        void Start ()
        {
            _safePosition = gameObject.transform.position;
        }
	
        // Update is called once per frame
        void Update () {
            if (IsOut)
            {
                StartCoroutine(Safe());
            }
            else
            {
                StartCoroutine(Out());
            }
        }

        void OnCollisionEnter(Collision collision)
        {   
            if( IsOut&&collision.gameObject.GetComponent<Character>()) {
                if ( Vector3.Distance(OutPosition, transform.position) < 0.5f)
                {
                    collision.gameObject.GetComponent<Character>().enabled = false;
                    Global.gameManager.die();
                }
                else {
                    collision.gameObject.GetComponent<Character>().velocity = (OutPosition - _safePosition) * Speed*2f;
                }
            }
        }

        private IEnumerator Safe()
        {
            float time = 0.0f;
            while (IsOut)
            {
                if (time < 1.0f)
                {
                    time += Time.deltaTime * Speed;
                    gameObject.transform.position = new Vector3(
                        Mathf.Lerp(_safePosition.x, OutPosition.x, time),
                        Mathf.Lerp(_safePosition.y, OutPosition.y, time),
                        Mathf.Lerp(_safePosition.z, OutPosition.z, time));
                }
                else
                {
                    gameObject.transform.position = OutPosition;
                    IsOut = false;
                }
                yield return null;
            }
            IsOut = false;
        }

        private IEnumerator Out()
        {
            float time = 0.0f;
            while (!IsOut)
            {
                if (time < 1.0f)
                {
                    time += Time.deltaTime * Speed;
                    gameObject.transform.position = new Vector3(
                        Mathf.Lerp(OutPosition.x, _safePosition.x, time),
                        Mathf.Lerp(OutPosition.y, _safePosition.y, time),
                        Mathf.Lerp(OutPosition.z, _safePosition.z, time));
                }
                else
                {
                    gameObject.transform.position = _safePosition;
                    IsOut = true;
                }
                yield return null;
            }

            IsOut = true;
        }
    }
}
