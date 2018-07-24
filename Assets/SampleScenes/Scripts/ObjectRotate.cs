using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotate : MonoBehaviour {
    Transform p;
    bool isGet = false;
    int timer = 0;
   NetWorkEDO ideo;
    SeachPlayer dr;
    // Use this for initialization
    void Start () {
        p = GameObject.Find("PlayerLOD0 5(Clone)").GetComponent<Transform>();
        ideo = GameObject.Find("PlayerLOD0 5(Clone)").GetComponent<NetWorkEDO>();
        dr = GameObject.Find("Dragon").GetComponent<SeachPlayer>();

    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(0, 2, 0));

        Vector3 v = transform.position - p.position;
        float mag = v.magnitude;

        if ((!isGet) && (mag <= 2.5f)) {
            ideo.StarIncrement();
            dr.Notification();
            isGet = true;
        }

        if (isGet) {
            transform.Translate(new Vector3(0, .1f, 0));
            transform.Rotate(new Vector3(0, 3, 0));
            ++timer;
            if(timer >= 100) {
                transform.Translate(new Vector3(0, -100f, 0));
                Destroy(this);
                return;
            }
        }
    }
}
