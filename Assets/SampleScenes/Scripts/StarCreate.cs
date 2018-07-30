using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCreate : MonoBehaviour {
    public GameObject[] objs;

    public GameObject original;
    public LEVEL _level;
    public LastStarCreate lsc;

    // Use this for initialization
    void Start ()
    {
        _level = Prototype.NetworkLobby.LobbyMainMenu.ReturnLevel();
    }
	
	// Update is called once per frame
	void Update () {
        switch (_level) {
            case LEVEL.LV_EASY:
                LevelCon(25);
                break;
            case LEVEL.LV_NORMAL:
                LevelCon(40);
                break;
            case LEVEL.LV_HARD:
                LevelCon(50);
                break;
            case LEVEL.LV_SUPER:
                LevelCon(60);
                break;
            case LEVEL.LV_NIGHTMARE:
                LevelCon(30);
                break;
            case LEVEL.LV_NON:
                break;
        }
        
        Destroy(this);
    }

    public void LevelCon(int a)
    {
        for (int i = 0; i < a; i += 5) {
            Instantiate(original, objs[i + Random.Range(0, 5)].transform.position, new Quaternion(0, 0, 0, 0));
        }
    }
    public void LevelCon2()
    {
        for (int i = 0; i < 50; i += 5) {
            int rnd = Random.Range(0, 5);
            Instantiate(original, objs[i + rnd].transform.position, new Quaternion(0, 0, 0, 0));
            Instantiate(original, objs[i + (rnd + Random.Range(1, 5)) % 5].transform.position, new Quaternion(0, 0, 0, 0));
        }
    }


}
