using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public static Hud instance { get; private set; }

    public Text gameTime;
    public GameObject timeUp;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init()
    {
        instance = this;
        timeUp.SetActive(false);
    }

    public void SetTime(float remainingTime)
    {
        if (remainingTime <= 0f)
        {
            timeUp.SetActive(true);
            gameTime.text = "0";
        }
        else
        {
            gameTime.text = ((int)remainingTime).ToString();
        }
    }
}
