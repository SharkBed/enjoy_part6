using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class notification : MonoBehaviour {
    public Image img;
    
    public SeachPlayer e;
    // Use this for initialization
    void Start()
    {
        e = GameObject.Find("Dragon").GetComponent<SeachPlayer>();
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (e.notificationTime > 0) {
            img.enabled = true;
        }
        else {
            img.enabled = false;
        }
    }
}
