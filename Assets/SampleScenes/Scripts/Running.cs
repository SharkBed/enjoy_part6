using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Running : MonoBehaviour {

    public Image img;
    public NetWorkEDO tps;
    // Use this for initialization
    void Start () {
        img = GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
        if(tps == null) {
            tps = GameObject.Find("PlayerLOD0 5(Clone)").GetComponent<NetWorkEDO>();
        }
        if ((tps.sprintburst >= 4000) || (tps.damageBoost >= 0)) {
            img.enabled = true;
        }
        else {
            img.enabled = false;
        }
        
    }
}
