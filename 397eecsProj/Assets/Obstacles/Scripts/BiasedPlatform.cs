using UnityEngine;

namespace Assets.Obstacles.Scripts
{
    public class BiasedPlatform : MonoBehaviour
    {

        public bool PlayerOne;
        public bool PlayerTwo;
        public GameObject Player;

        // Use this for initialization
        void Start () {
            if (!PlayerOne && !PlayerTwo)
            {
                PlayerOne = true;
            }
        }
	
        // Update is called once per frame
        void Update () {
            if ((Player.GetComponent<Character>().MovingPlayer == 1 && PlayerOne) || (Player.GetComponent<Character>().MovingPlayer == 2 && PlayerTwo))
            {
                gameObject.GetComponent<BoxCollider>().enabled = true;
            }
            else
            {
                gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
