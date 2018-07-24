using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerFollowCamera : MonoBehaviour
{
    private float turnSpeed = 4.0f;
    public Transform player;

    private float distance = 2.5f;
    private Quaternion vRotation;
    public Quaternion hRotation;

    public float angleLimit;

    // Use this for initialization
    void Start () {
        // 回転の初期化
        vRotation = Quaternion.identity;                // 垂直回転(X軸を軸とする回転)は、30度見下ろす回転
        hRotation = Quaternion.identity;                // 水平回転(Y軸を軸とする回転)は、無回転
        transform.rotation = hRotation * vRotation;     // 最終的なカメラの回転は、垂直回転してから水平回転する合成回転

        // 位置の初期化
        // player位置から距離distanceだけ手前に引いた位置を設定します
        transform.position = player.position - transform.rotation * Vector3.forward * distance;
    }

    // Update is called once per frame
    void LateUpdate ()
    {
        // 水平回転の更新
        angleLimit += -Input.GetAxis("Mouse Y");
        angleLimit = angleLimit <= 9.0f ? angleLimit : 9.0f;
        angleLimit = angleLimit >= -3.0f ? angleLimit : -3.0f;

        hRotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * turnSpeed, 0);
        vRotation = Quaternion.Euler(angleLimit * turnSpeed, 0, 0);

        transform.rotation = hRotation * vRotation;

        transform.position = player.position + new Vector3(0, 2, 0) - transform.rotation * Vector3.forward * distance;
    }
}
