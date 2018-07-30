using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarCnt : MonoBehaviour {

    int star;
    Text t;
    NetWorkEDO ideo;

    public LEVEL _level;

    // Use this for initialization
    void Start () {
        t = GetComponent<Text>();
        ideo = GameObject.Find("PlayerLOD0 5(Clone)").GetComponent<NetWorkEDO>();
        _level = Prototype.NetworkLobby.LobbyMainMenu.ReturnLevel();
    }
	
	// Update is called once per frame
	void Update () {
        switch (_level) {

            case LEVEL.LV_EASY:
                star = 5 - ideo.getStar;
                break;
            case LEVEL.LV_NORMAL:
                star = 8 - ideo.getStar;
                break;
            case LEVEL.LV_HARD:
                star = 10 - ideo.getStar;
                break;
            case LEVEL.LV_SUPER:
                star = 12 - ideo.getStar;
                break;
            case LEVEL.LV_NIGHTMARE:
                star = 6 - ideo.getStar;
                break;
            case LEVEL.LV_NON:
                break;
        }
        t.text = star.ToString();
    }
}
