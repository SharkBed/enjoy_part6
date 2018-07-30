using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetWorkEDO : NetworkBehaviour
{
    private Vector3 velocity;

    private CharacterController characterController;    //未使用？
    public Transform player;
    public SeachPlayer e;
    private float applySpeed = 0.2f;
    public float playerSpeed = .0f;
    public int damageBoost = 0;
    public int sprintburst = 0;
    public PlayerFollowCamera refCamera;
    public Transform enemy;
    public Slider hpBar;
    public Slider stamina;
    public int hp;
    public int getStar = 0;
    private float angle;
    GateOpen g1, g2;
    private float speedScale = 1.0f;
    bool kusomuzu_flg = false;
    public GameObject hitEffect;
    public GameObject DustEffect;

    [SyncVar(hook = "OnScoreChanged")]
    public int score = 0;
    [SyncVar]
    public Color color = Color.black;
    [SyncVar]
    public string playerName = "";
    [SyncVar]
    public GameObject playerModelObject;
    [SyncVar(hook = "OnLifeChanged")]
    public int lifeCount;
    [SyncVar(hook = "OncharNumberChanged")]
    public int charctorNumbers = 0;

    protected Text _scoreText;

    protected bool _wasInit = false;

    public int waitTime = 0;
    public LEVEL _level;

    // Use this for initialization
    void Start()
    {
        characterController = GetComponent<CharacterController>();

        hp = 100;
        player = GetComponent<Transform>();
        hpBar = GameObject.Find("HitPoint").GetComponent<Slider>();
        stamina = GameObject.Find("Stamina").GetComponent<Slider>();
        enemy = GameObject.Find("Dragon").GetComponent<Transform>();
        e = GameObject.Find("Dragon").GetComponent<SeachPlayer>();
        g1 = GameObject.Find("GATE01").GetComponent<GateOpen>();
        g2 = GameObject.Find("GATE02").GetComponent<GateOpen>();

        _level = Prototype.NetworkLobby.LobbyMainMenu.ReturnLevel();
        if (_level == LEVEL.LV_SUPER) kusomuzu_flg = true;
    }

    void Awake()
    {
        NetworkGameManager.nEDO.Add(this);
    }

    // Update is called once per frame
    [ClientCallback]
    void FixedUpdate()
    {

        if (!hasAuthority)
            return;

        if (hp <= 0) {
            e.PlayerDead();
            if(angle <= 90.0f) {
                angle += 1.0f;
                player.transform.Rotate(new Vector3(0, 0, 1));
            }
            return;
        }

        speedScale = 1.0f;
        waitTime++;
        velocity = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) {
            DustEffect.SetActive(true);
            waitTime = 0;
            velocity.z += .2f;
        }
        if (Input.GetKey(KeyCode.A)) {
            DustEffect.SetActive(true);
            waitTime = 0;
            velocity.x -= .2f;
        }
        if (Input.GetKey(KeyCode.S)) {
            DustEffect.SetActive(true);
            waitTime = 0;
            velocity.z -= .2f;
        }
        if (Input.GetKey(KeyCode.D)) {
            DustEffect.SetActive(true);
            waitTime = 0;
            velocity.x += .2f;
        }

        if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A)&& !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
        {
            DustEffect.SetActive(false);
        }

        if (waitTime == 500) {

            e.Notification();
        }

        velocity = velocity.normalized;

        playerSpeed = .06f;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerSpeed = .1f;

        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (sprintburst <= 0)
            {
                sprintburst = 4300;
            }
        }

        if (sprintburst >= 4000)
        {
            speedScale *= 1.5f;
        }
        sprintburst--;
        if (damageBoost >= 0)
        {
            speedScale *= 1.5f;
            damageBoost--;
        }

        if (speedScale >= 1.5f)
        {

            speedScale = 1.5f;
        }
        DragonHit();

        velocity.x = velocity.x * playerSpeed * speedScale;
        velocity.y = velocity.y * playerSpeed * speedScale;
        velocity.z = velocity.z * playerSpeed * speedScale;

        hpBar.value = hp;
        if (sprintburst >= 0)
        {
            stamina.value = 4000 - sprintburst;
        }
        else
        {
            stamina.value = 4000;
        }

        if (kusomuzu_flg) {

            e.Notification();
            kusomuzu_flg = false;
        }
        
        if (_level == LEVEL.LV_NORMAL) {
            if (getStar >= 8) {
                e.Notification();
            }
        }
        if (_level == LEVEL.LV_HARD) {
            if (getStar >= 9) {
                e.Notification();
            }
        }
        if (_level == LEVEL.LV_SUPER) {
            if (getStar >= 10) {
                e.Notification();
            }
        }
        if (_level == LEVEL.LV_NIGHTMARE) {
            e.Notification();
        }

        // いずれかの方向に移動している場合
        if (velocity.magnitude > 0)
        {
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

        switch (_level) {
            case LEVEL.LV_EASY:
                if (getStar != 5) {
                    return;
                }
                break;
            case LEVEL.LV_NORMAL:
                if (getStar != 8) {
                    return;
                }
                break;
            case LEVEL.LV_HARD:
                if (getStar != 10) {
                    return;
                }
                break;
            case LEVEL.LV_SUPER:
                if (getStar != 12) {
                    return;
                }
                break;
            case LEVEL.LV_NIGHTMARE:
                if (getStar != 6) {
                    return;
                }
                break;
            case LEVEL.LV_NON:
                break;
        }

        g1.flg = true;
        g2.flg = true;
    }

    public void DragonHit()
    {

        Vector3 v3 = player.transform.position - enemy.position;
        float mag = v3.magnitude;
        if (mag < 1.8f)
        {
            if (damageBoost <= 0)
            {
                damageBoost = 300;
                hp -= 20;
                Instantiate(hitEffect, new Vector3(player.position.x, player.position.y + 1.0f, player.position.z), new Quaternion(0, 0, 0, 0));
            }
        }
    }

    public void StarIncrement()
    {
        getStar++;
    }

    public override void OnStartLocalPlayer()
    {
        //Camera.main.GetComponent<PlayerFollowCamera>().player = transform;
        //プレイヤーによってカメラを取得
        transform.Find("MainCamera").gameObject.SetActive(true);
    }

    public void Init()
    {
        if (_wasInit)
            return;

        GameObject scoreGO = new GameObject(playerName);
        scoreGO.transform.SetParent(NetworkGameManager.sInstance.uiScoreZone.transform, false);
        _scoreText = scoreGO.AddComponent<Text>();
        _scoreText.alignment = TextAnchor.MiddleCenter;
        _scoreText.font = NetworkGameManager.sInstance.uiScoreFont;
        //_scoreText.resizeTextForBestFit = true;
        _scoreText.fontSize = 30;
        _scoreText.color = Color.white;
        _wasInit = true;

        UpdateScoreLifeText();
    }

    void OnScoreChanged(int newValue)
    {
        score = newValue;
        UpdateScoreLifeText();
    }

    void OnLifeChanged(int newValue)
    {
        lifeCount = newValue;
        UpdateScoreLifeText();
    }

    void OncharNumberChanged(int newValue)
    {
        charctorNumbers = newValue;
        UpdateScoreLifeText();
    }

    void UpdateScoreLifeText()
    {
        if (_scoreText != null)
        {
            _scoreText.text = playerName;
        }
    }
    void OnDestroy()
    {
        NetworkGameManager.nEDO.Remove(this);
    }

}

