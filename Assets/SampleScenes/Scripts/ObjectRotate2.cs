using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotate2 : MonoBehaviour
{
    Transform p;
    bool isGet = false;
    int timer = 0;
    NetWorkEDO ideo;
    SeachPlayer dr;

    public AudioSource se;
    LEVEL _level;
    // Use this for initialization
    void Start()
    {
        p = GameObject.Find("PlayerLOD0 5(Clone)").GetComponent<Transform>();
        ideo = GameObject.Find("PlayerLOD0 5(Clone)").GetComponent<NetWorkEDO>();
        dr = GameObject.Find("Dragon").GetComponent<SeachPlayer>();

        _level = Prototype.NetworkLobby.LobbyMainMenu.ReturnLevel();

        se = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 2, 0));

        Vector3 v = transform.position - p.position;
        float mag = v.magnitude;

        if ((!isGet) && (mag <= 2.5f)) {
            se.mute = false;
            se.Play();

            //回復エフェクト
            ideo.RecoveryHit();

            if (_level == LEVEL.LV_HARD || _level == LEVEL.LV_SUPER || _level == LEVEL.LV_NIGHTMARE) {
                ideo.hp += 20;
                if(ideo.hp > 100) {
                    ideo.hp = 100;
                }
            }
            else {
                ideo.hp = 100;
            }
            ideo.sprintburst = 0;
            dr.Notification();
            isGet = true;
        }

        if (isGet) {
            transform.Translate(new Vector3(0, .1f, 0));
            transform.Rotate(new Vector3(0, 3, 0));
            ++timer;
            if (timer >= 100) {
                transform.Translate(new Vector3(0, -100f, 0));
                Destroy(this);
                return;
            }
        }
    }
}
