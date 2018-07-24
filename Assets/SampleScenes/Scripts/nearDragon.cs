using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class nearDragon : MonoBehaviour {
    public Image img;
    distance_Test dis;

    public Transform p;
    public Transform e;
    // Use this for initialization
    void Start () {
        p = GameObject.Find("PlayerLOD0 5(Clone)").GetComponent<Transform>();
        e = GameObject.Find("Dragon").GetComponent<Transform>();
        img = GetComponent<Image>();
        dis = GetComponent<distance_Test>();
    }
	
	// Update is called once per frame
	void Update () {

        Vector3 pong = p.position - e.position;
        float mag = pong.magnitude;
        if (mag <= 64.0f) {
            img.enabled = true;
        }
        else {
            img.enabled = false;
        }
    }
}
