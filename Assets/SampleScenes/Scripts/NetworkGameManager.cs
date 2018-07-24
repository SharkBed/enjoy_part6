using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;
using System.Collections;
using System.Collections.Generic;

//ゲーム時間の同期処理やUI表示

public class NetworkGameManager : NetworkBehaviour
{
    static public List<NetworkSpaceship> sShips = new List<NetworkSpaceship>();
    static public List<NetWorkEDO> nEDO = new List<NetWorkEDO>();

    static public NetworkGameManager sInstance = null;

    public Hud hud;

    public const int timeLimit = 60 * 15;//5分

    public GameObject uiScoreZone;
    public Font uiScoreFont;

    public GameObject ClearObject;

    public static bool isClockAvailable
    {
        get
        {
            return sInstance != null && (sInstance.isServer || sInstance.latency > 0f);
        }
    }
    public static bool isInPlay
    {
        get
        {
            return isClockAvailable && 0 < sInstance.remainingTime && sInstance.remainingTime < timeLimit;
        }
    }

    private float remainingTime
    {
        get
        {
            return (timeLimit ) - (worldClock - serverStartTime);
        }
    }
    public float worldClock
    {
        get
        {
            return localClock + clockCorrectDelta;
        }
    }

    private const int MsgClientGameTimeMessageId = 1100;
    private const int MsgServerGameTimeMessageId = 1101;

    private float clientSendIntervalTime = 0f;
    private float latency;
    private float clientDeltaTime;
    private float clockCorrectDelta = 0f;
    private bool timeUpStarted = false;
    private float lastSendTime;

    [Space]

    protected bool _running = true;

    [SyncVar]
    public float serverStartTime;
    private bool transformNetworkGui = true;

    private static float localClock
    {
        get
        {
            //return Time.realtimeSinceStartup;
            var dt = System.DateTime.UtcNow;
            return dt.Hour * 3600 + dt.Minute * 60 + dt.Second + dt.Millisecond / 1000f;
        }
    }

    void Awake()
    {
        sInstance = this;
    }

    public override void OnStartServer()
    {
        serverStartTime = localClock;
        NetworkServer.RegisterHandler(MsgClientGameTimeMessageId, OnServerReceiveGameTime);
    }

    public override void OnStartClient()
    {
        for (int i = 0; i < nEDO.Count; ++i)
        {
            nEDO[i].Init();
        }

        hud.Init();

        var client = NetworkManager.singleton.client;
        if (client != null)
        {
            client.RegisterHandler(MsgServerGameTimeMessageId, OnClientReceiveGameTime);
        }
    }

    void Start()
    {
        if (NetworkGameManager.sInstance != null)
        {
            for (int i = 0; i < nEDO.Count; ++i)
            {
                nEDO[i].Init();
            }

            hud.Init();
        }
    }

    void Update()
    {
        if (!_running || nEDO.Count == 0)
            return;

        bool allDestroyed = true;

        //サーバーだけの処理
        if (isServer)
        {
           if(remainingTime <= 0f && !timeUpStarted || ClearChackScript.s_instance.IsClear())
            {
                timeUpStarted = true;
                _running = false;
                StartCoroutine(ReturnToLoby());
            }

           for(int i = 0; i < nEDO.Count; ++i) 
           {
                if(nEDO[i].hp ==0) 
                {
                    _running = false;
                    StartCoroutine(ReturnToLoby());
                }
           }   
            
        }
        //クライアントだけの処理
        if(isClient)
        {
            clientSendIntervalTime -= Time.unscaledDeltaTime;
            if (clientSendIntervalTime <= 0f)
            { // １秒おきに送信
                clientSendIntervalTime = 1f;
                //  msg = 送信データ。 lastSendTime = localClock = ローカル時刻
                lastSendTime = localClock; // 最後に送信したクライアント時刻を覚えておく
                var msg = new GameTimeClientMessage { sendTime = lastSendTime };
                NetworkManager.singleton.client.SendUnreliable(MsgClientGameTimeMessageId, msg);
                var me = NetworkClient.allClients;
            }
        }

        for (int i = 0; i < nEDO.Count; ++i)
        {
            allDestroyed &= (nEDO[i].lifeCount == 0);
        }

        hud.SetTime(Mathf.Clamp(remainingTime, 0f, timeLimit));
        hud.SetLevel();
    }

    //テスト用
    void OnGUI()
    {

        //var reset = false;        


        //GUILayout.Label("**** " + localClock);

        //if (isServer)
        //{
        //    GUILayout.Label("server: " + localClock);
        //}
        //else
        //{
        //    GUILayout.Label("client: " + worldClock);
        //    GUILayout.Label("latency: " + latency);
        //}
       
        //if (connectionToServer != null)
        //{
        //    int toServerMsgNum;
        //    int toServerNumBytes;
        //    connectionToServer.GetStatsIn(out toServerMsgNum, out toServerNumBytes);
        //    GUILayout.Label("SRV  msg/bytes: " + toServerMsgNum + "/" + toServerNumBytes);
        //    if (reset)
        //    {
        //        connectionToServer.ResetStats();
        //    }
        //}
        //if (connectionToClient != null)
        //{
        //    int toClientMsgNum;
        //    int toClientNumBytes;
        //    connectionToClient.GetStatsIn(out toClientMsgNum, out toClientNumBytes);
        //    GUILayout.Label("CLI msg/bytes: " + toClientMsgNum + "/" + toClientNumBytes);
        //    if (reset)
        //    {
        //        connectionToClient.ResetStats();
        //    }
        //}
    }

    //ロビーに帰ります
    //タイムアップか死ぬかゴールするか
    IEnumerator ReturnToLoby()
    {
        if (isServer)
        {
            ClearChackScript.s_instance.ClearFlag = false;
            yield return new WaitForSeconds(3.0f);
            LobbyManager.s_Singleton.ServerReturnToLobby();
        }
    }

    private void OnServerReceiveGameTime(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<GameTimeClientMessage>();
        var rtMsg = new GameTimeServerMessage { sendTime = msg.sendTime, serverTime = localClock };
        NetworkServer.SendToClient(netMsg.conn.connectionId, MsgServerGameTimeMessageId, rtMsg);
    }

    private void OnClientReceiveGameTime(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<GameTimeServerMessage>();
        if (lastSendTime != msg.sendTime)
        {
            var now = localClock;
            latency = (now - msg.sendTime) * 0.5f;
            clientDeltaTime = msg.serverTime - now;
            clockCorrectDelta = clientDeltaTime + latency;
        }
        // now = localClock がどのクライアント(サーバー)でも同じ値のとき（全クライアント同一PC上＋localClockの get{} 実装修正）だけ有効な誤差確認。
        // var err = currentClock - now;
        // Debug.Log("同期誤差=" + err + " 現在時刻=" + now + " 補正時刻=" + currentClock + " レイテンシ=" + latency);
    }

    public class GameTimeClientMessage : MessageBase
    {
        public float sendTime;
    }

    public class GameTimeServerMessage : MessageBase
    {
        public float sendTime;
        public float serverTime;
    }
}
