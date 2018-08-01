using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Text_FadeIn : MonoBehaviour {
    public Image _this;
    float alpha = 0;
    int waitframe = 0;
	// Use this for initialization
	void Start ()
    {
        _this.color = new Color(1.0f, 1.0f, 1.0f, 0);
    }
	
	// Update is called once per frame
	void Update () {
        waitframe++;
        if(waitframe >= 60)
            alpha += 0.008f;
        _this.color = new Color(1.0f, 1.0f, 1.0f, alpha);

    }
}
