using System.Collections;
using UnityEngine;

namespace Assets.Obstacles.Scripts
{
    public class PlayerGate : MonoBehaviour {

        public bool IsRedGate;
        public bool IsBlueGate;
        public bool IsOpen;
        public GameObject Player;
        public Vector3 ClosedPosition;
        public Vector3 OpenPosition;
        public float Speed;

        // Use this for initialization
        void Start ()
        {

        }
	
        // Update is called once per frame
        void Update () {
            if (Player.GetComponent<Character>().MovingPlayer == 1 && IsRedGate && !IsOpen)
            {
                StartCoroutine(Open());
            }
            else if (Player.GetComponent<Character>().MovingPlayer == 2 && IsRedGate && IsOpen)
            {
                StartCoroutine(Close());
            }
            if (Player.GetComponent<Character>().MovingPlayer == 2 && IsBlueGate && !IsOpen)
            {
                StartCoroutine(Open());
            }
            else if (Player.GetComponent<Character>().MovingPlayer == 1 && IsBlueGate && IsOpen)
            {
                StartCoroutine(Close());
            }
        }

        private IEnumerator Open()
        {
            float openTime = 0.0f;
            while (!IsOpen)
            {
                if (openTime < 1.0f)
                {
                    openTime += Time.deltaTime * Speed;
                    gameObject.transform.position = new Vector3(
                        Mathf.Lerp(ClosedPosition.x, OpenPosition.x, openTime),
                        Mathf.Lerp(ClosedPosition.y, OpenPosition.y, openTime),
                        Mathf.Lerp(ClosedPosition.z, OpenPosition.z, openTime));
                }
                else
                {
                    gameObject.transform.position = OpenPosition;
                    IsOpen = true;
                }
                yield return null;
            }
            IsOpen = true;
        }

        private IEnumerator Close()
        {
            float closeTime = 0.0f;
            while (IsOpen)
            {
                if (closeTime < 1.0f)
                {
                    closeTime += Time.deltaTime * Speed;
                    gameObject.transform.position = new Vector3(
                        Mathf.Lerp(OpenPosition.x, ClosedPosition.x, closeTime),
                        Mathf.Lerp(OpenPosition.y, ClosedPosition.y, closeTime),
                        Mathf.Lerp(OpenPosition.z, ClosedPosition.z, closeTime));
                }
                else
                {
                    gameObject.transform.position = ClosedPosition;
                    IsOpen = false;
                }
                yield return null;
            }

            IsOpen = false;
        }
    }
}
