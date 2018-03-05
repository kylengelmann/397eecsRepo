using UnityEngine;

namespace Assets.Obstacles.Scripts
{
    public class Goal : MonoBehaviour {

        public checkpoint initCheck;
        // Use this for initialization
        void Start () {
		
        }
	
        // Update is called once per frame
        void Update () {
		
        }

        void OnTriggerEnter(Collider collision)
        {
            //Debug.Log("I am triggered");
            if (collision.gameObject.GetComponent<Character>())
            {
                collision.gameObject.GetComponent<Character>().lastCkpt = initCheck;
                FindObjectOfType<GameManager>().die();
            }
        }
    }
}
