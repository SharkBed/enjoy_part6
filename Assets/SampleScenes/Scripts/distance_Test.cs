using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class distance_Test : MonoBehaviour {

    public float mag;
    public Transform p;
    public Transform e;
    public bool near;

    Image img;

    // Use this for initialization
    void Start ()
    {
        p = GameObject.Find("PlayerLOD0 5(Clone)").GetComponent<Transform>();
        e = GameObject.Find("Dragon").GetComponent<Transform>();
        img = GetComponent<Image>();
        img.color = Color.clear;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 pong = p.position - e.position;
        mag = pong.magnitude;

        if (mag <= 64.0f) {
            this.img.color = new Color(0.5f, 0f, 0f, (1.0f - (mag / 64.0f)) * 0.25f);
            near = true;
        }
        else {
            this.img.color = Color.Lerp(this.img.color, Color.clear, Time.deltaTime);
            near = false;
        }
    }

    public float GetMag(){
        return mag;
    }
}
