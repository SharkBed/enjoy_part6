using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SeachPlayer : MonoBehaviour {

    public Transform[] points;
    public int destPoint;
    private NavMeshAgent agent;
    public Transform player;

    public LEVEL _level;

    public int notificationTime = 0;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        GotoNextPoint();

        player = GameObject.Find("PlayerLOD0 5(Clone)").GetComponent<Transform>();

        _level = Prototype.NetworkLobby.LobbyMainMenu.ReturnLevel();
        if (_level == LEVEL.LV_EASY) agent.speed = 9;
        if (_level == LEVEL.LV_NORMAL) agent.speed = 10;
        if (_level == LEVEL.LV_HARD) agent.speed = 11;
        if (_level == LEVEL.LV_SUPER) {
            agent.speed = 12;
            agent.acceleration += 2;
        }

    }

    void GotoNextPoint()
    {
        if (points.Length == 0) //ポイントの数が0だった場合動かない
            return;

        //次の巡回先をエージェントに指定する
        agent.SetDestination(points[destPoint].position);

        //次の巡回先を設定
        destPoint = (destPoint + Random.Range(1, (points.Length - 1))) % points.Length;
    }

    void OnTriggerStay(Collider col){
        if(col.tag == "Player") {  //見つけたお！

            if (!Physics.Linecast(transform.position + Vector3.up, col.gameObject.transform.position + Vector3.up, LayerMask.GetMask("Field"))){
                agent.SetDestination(player.position);

                notificationTime = 2;
            }
        }
    }

    private void Update() {
        if (Input.GetKey("space")) {
            
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f) { //目的地に近づいた場合次の巡回先に切り替え
            GotoNextPoint();
        }

        if(notificationTime > 0) {
            notificationTime--;
        }
    }

    public void Notification()
    {
        notificationTime = 100;
        agent.SetDestination(player.position);
        destPoint = (destPoint + Random.Range(1, (points.Length - 1))) % points.Length;
    }
}
