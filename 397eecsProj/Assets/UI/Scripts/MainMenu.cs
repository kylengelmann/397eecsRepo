using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    string p1a;
    string p2a;
    public string sceneToLoad;
    public int sceneBuildIndex;

	// Use this for initialization
    void Start()
    {

        string platform = "";
        string joyNum = "_Key";
        string controller = "";
        string[] joys = Input.GetJoystickNames();
        int numControllers = joys.Length;
        if (numControllers > 0)
        {
            if (joys[0].IndexOf("Joy-Con") >= 0)
            {
                controller = "_Joycon";
            }
            platform = "Mac";
            if (Application.platform == RuntimePlatform.WindowsEditor
               || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                platform = "Win";
            }
            joyNum = "_J1";
        }


        if (numControllers > 1)
        {
            p1a = "AY" + platform + "_J1" + controller;
            p2a = "AY" + platform + "_J2" + controller;
        }
        else
        {
            p1a = "DPadVertical" + platform + joyNum;
            p2a = "AY" + platform + joyNum;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxisRaw(p1a) < -0.25f || Input.GetAxisRaw(p2a) < -0.25f)
        {
            SceneManager.LoadScene(sceneBuildIndex);
        }
	}
        
}
