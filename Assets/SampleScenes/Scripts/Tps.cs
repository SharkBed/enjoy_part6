using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tps : MonoBehaviour
{
    public Transform player;
    public Vector3 velocity;
    private float applySpeed = 0.2f;
    public float playerSpeed = .0f;
    public int damageBoost = 0;
    public int sprintburst = 0;
    public PlayerFollowCamera refCamera;
    public Transform enemy;
    public Slider hpBar;
    public Slider stamina;
    public int hp;

    // Use this for initialization
    void Start()
    {
        hp = 100;
        player = GetComponent<Transform>();
        hpBar = GameObject.Find("HitPoint").GetComponent<Slider>();
        stamina = GameObject.Find("Stamina").GetComponent<Slider>();
        enemy = GameObject.Find("Dragon").GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
            velocity.z += .2f;
        if (Input.GetKey(KeyCode.A))
            velocity.x -= .2f;
        if (Input.GetKey(KeyCode.S))
            velocity.z -= .2f;
        if (Input.GetKey(KeyCode.D))
            velocity.x += .2f;

        velocity = velocity.normalized;

        playerSpeed = .06f;

        if (Input.GetKey(KeyCode.LeftShift)) {
            playerSpeed = .1f;
            if(sprintburst <= 0) {
                sprintburst = 4300;
            }
        }

        if(sprintburst >= 4000) { 
            playerSpeed *= 1.5f;
        }
        sprintburst--;
        if (damageBoost>=0) {
            playerSpeed *= 1.5f;
            damageBoost--;
        }
        DragonHit();
        velocity.x = velocity.x * playerSpeed;
        velocity.y = velocity.y * playerSpeed;
        velocity.z = velocity.z * playerSpeed;

        hpBar.value = hp;
        if(sprintburst >= 0) {
            stamina.value = 4000 - sprintburst;
        }
        else {
            stamina.value = 4000;
        }

        // いずれかの方向に移動している場合
        if (velocity.magnitude > 0) {
            // プレイヤーの回転(transform.rotation)の更新
            // 無回転状態のプレイヤーのZ+方向(後頭部)を、
            // カメラの水平回転(refCamera.hRotation)で回した移動の反対方向(-velocity)に回す回転に段々近づけます
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(refCamera.hRotation * velocity),
                                                  applySpeed);

            // プレイヤーの位置(transform.position)の更新
            // カメラの水平回転(refCamera.hRotation)で回した移動方向(velocity)を足し込みます
            transform.position += refCamera.hRotation * velocity;
        }
    }

    public void DragonHit()
    {

        Vector3 v3 = player.transform.position - enemy.position;
        float mag = v3.magnitude;
        if (mag < 1.8f) {
            if (damageBoost <= 0) {
                damageBoost = 300;
                hp -= 25;
            }
        }
    }
}
