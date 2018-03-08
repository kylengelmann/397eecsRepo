using System.Collections;
using UnityEngine;

namespace Assets.Obstacles.Scripts
{
    public class MovingPlatform : MonoBehaviour {

        public bool Return;
        public Vector3 PointOne;
        public Vector3 PointTwo;
        public float Speed;

        // Use this for initialization
        void Start () {
		
        }

        // Update is called once per frame
        void Update()
        {
            if (!Return)
            {
                StartCoroutine(ToPointTwo());
            }
            else
            {
                StartCoroutine(ToPointOne());
            }
        }

        private IEnumerator ToPointTwo()
        {
            float travelTime = 0.0f;
            while (!Return)
            {
                if (travelTime < 1.0f)
                {
                    travelTime += Time.deltaTime * Speed;
                    gameObject.transform.position = Vector3.Lerp(PointOne, PointTwo, travelTime);
                }
                else
                {
                    gameObject.transform.position = PointTwo;
                    Return = true;
                }
                yield return null;
            }
            Return = true;
        }

        private IEnumerator ToPointOne()
        {
            float travelTime = 0.0f;
            while (Return)
            {
                if (travelTime < 1.0f)
                {
                    travelTime += Time.deltaTime * Speed;
                    gameObject.transform.position = Vector3.Lerp(PointTwo, PointOne, travelTime);
                }
                else
                {
                    gameObject.transform.position = PointOne;
                    Return = false;
                }
                yield return null;
            }

            Return = false;
        }
    }
}
