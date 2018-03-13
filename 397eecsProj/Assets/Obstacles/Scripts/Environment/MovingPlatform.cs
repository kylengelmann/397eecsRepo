using System.Collections;
using UnityEngine;

namespace Assets.Obstacles.Scripts
{
    public class MovingPlatform : MonoBehaviour {

        public bool Return;
        public Vector3 PointOne;
        public Vector3 PointTwo;
        public float Speed;

        public float maxMoveHeight;

        BoxCollider box;

        Collider[] onMe;

        bool toPoint1 = true;

        // Use this for initialization
        void Start () {
            box = GetComponent<BoxCollider>();
            onMe = new Collider[8];
            StartCoroutine(ToPointTwo());
        }

        // //Update is called once per frame
        //void Update()
        //{
        //    if (!Return)
        //    {
        //        toPoint1 = true;
        //        StartCoroutine(ToPointTwo());
        //    }
        //    else
        //    {
        //        toPoint1 = false;
        //        StartCoroutine(ToPointOne());
        //    }
        //}

        void FixedUpdate()
        {
            //if (!Return)
            //{
            //    StartCoroutine(ToPointTwo());
            //}
            //else
            //{
            //    StartCoroutine(ToPointOne());
            //}

            Vector3 center = box.center + Vector3.up*(box.size.y/2f + maxMoveHeight/2f);

            // For some reason you can't just multiply Vectors?
            center.x *= transform.lossyScale.x;
            center.y *= transform.lossyScale.y;
            center.z *= transform.lossyScale.z;

            center += transform.position;

            // Set the overlap box's half extents
            Vector3 halfExtents = box.size/2f;
            halfExtents.y = maxMoveHeight/2f;

            halfExtents.x *= transform.lossyScale.x;
            halfExtents.z *= transform.lossyScale.z;


            Debug.Log(halfExtents);



            // Overlap box to get objects on the belt
            int numOnMe = Physics.OverlapBoxNonAlloc(center, halfExtents, onMe, transform.rotation);

            // Find the ones that can be moved by the belt and move them
            for(int i = 0; i < numOnMe; i++) {
                velocityTaker velTake;
                if((velTake = onMe[i].gameObject.GetComponent<velocityTaker>()) != null) {
                    if(Return) {
                        velTake.velocity = (PointOne - PointTwo)*Speed;
                    }
                    else {
                        velTake.velocity = (PointTwo - PointOne)*Speed;
                    }
                    velTake.isOnGround = true;
                }
            }
        }

        private IEnumerator ToPointTwo()
        {
            float travelTime = 0.0f;
            while (!Return)
            {
                if (travelTime < 1.0f)
                {
                    travelTime += Time.fixedDeltaTime * Speed;
                    gameObject.transform.localPosition = Vector3.Lerp(PointOne, PointTwo, travelTime);
                }
                else
                {
                    gameObject.transform.localPosition = PointTwo;
                    Return = true;
                }
                yield return new WaitForFixedUpdate();
            }
            StartCoroutine(ToPointOne());
        }

        private IEnumerator ToPointOne()
        {
            float travelTime = 0.0f;
            while (Return)
            {
                if (travelTime < 1.0f)
                {
                    travelTime += Time.fixedDeltaTime * Speed;
                    gameObject.transform.localPosition = Vector3.Lerp(PointTwo, PointOne, travelTime);
                }
                else
                {
                    gameObject.transform.localPosition = PointOne;
                    Return = false;
                }
                yield return new WaitForFixedUpdate();
            }
            StartCoroutine(ToPointTwo());
        }
    }
}
