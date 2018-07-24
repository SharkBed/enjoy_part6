using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Prototype.NetworkLobby
{
    //Player entry in the lobby. Handle selecting color/setting name & getting ready for the game
    //Any LobbyHook can then grab it and pass those value to the game player prefab (see the Pong Example in the Samples Scenes)
    public class LobbyPlayer : NetworkLobbyPlayer
    {
        //static Color[] Colors = new Color[] { Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow };

        //キャラクターモデル達
        public  GameObject[] characterTypeModels;

        //used on server to avoid assigning the same color to two player
        //static List<int> _colorInUse = new List<int>();

        //同じキャラクターが使用されないように
        static List<int> charInUse = new List<int>();

        public Button colorButton;
        //キャラクター変更用ボタン
        [SerializeField]
        public Button charctorButton;

        //黒ひげ
        public Sprite face1;
        //デフォルトの人間
        public Sprite face2;
        //赤神      
        public Sprite face3;
        //青髪
        public Sprite face4;

        public InputField nameInput;
        public Button readyButton;
        public Button waitingPlayerButton;
        public Button removePlayerButton;

        public GameObject localIcone;
        public GameObject remoteIcone;

        //OnMyName function will be invoked on clients when server change the value of playerName
        [SyncVar(hook = "OnMyName")]
        public string playerName = "";
        //[SyncVar(hook = "OnMyColor")]
        //public Color playerColor = Color.white;
        //[SyncVar(hook = "OnMyCharctor")]
        //public GameObject playerCharType = null;
        //キャラクター生成用に借り作成（ホントはimage）
        [SyncVar(hook = "OnMyCharNumber")]
        public int CharIdx = 0;

        //ボタン表示用のキャラクター画像
        [SyncVar(hook = "OnChangeButtonImage")]
        public string buttonImageString = "face1";

        public Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        public Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);

        static Color JoinColor = new Color(255.0f/255.0f, 0.0f, 101.0f/255.0f,1.0f);
        static Color NotReadyColor = new Color(34.0f / 255.0f, 44 / 255.0f, 55.0f / 255.0f, 1.0f);
        static Color ReadyColor = new Color(0.0f, 204.0f / 255.0f, 204.0f / 255.0f, 1.0f);
        static Color TransparentColor = new Color(0, 0, 0, 0);

        //static Color OddRowColor = new Color(250.0f / 255.0f, 250.0f / 255.0f, 250.0f / 255.0f, 1.0f);
        //static Color EvenRowColor = new Color(180.0f / 255.0f, 180.0f / 255.0f, 180.0f / 255.0f, 1.0f);


        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();

            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(1);

            LobbyPlayerList._instance.AddPlayer(this);
            LobbyPlayerList._instance.DisplayDirectServerWarning(isServer && LobbyManager.s_Singleton.matchMaker == null);

            if (isLocalPlayer)
            {
                SetupLocalPlayer();
            }
            else
            {
                SetupOtherPlayer();
            }

            //setup the player data on UI. The value are SyncVar so the player
            //will be created with the right value currently on server
            OnMyName(playerName);
            //OnMyColor(playerColor);
            OnChangeButtonImage(buttonImageString);

            //OnMyCharctor(playerCharType);

            //OnMyCharNumber(CharIdx);
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            //if we return from a game, color of text can still be the one for "Ready"
            readyButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;

           SetupLocalPlayer();
        }

        void ChangeReadyButtonColor(Color c)
        {
            ColorBlock b = readyButton.colors;
            b.normalColor = c;
            b.pressedColor = c;
            b.highlightedColor = c;
            b.disabledColor = c;
            readyButton.colors = b;
        }

        void SetupOtherPlayer()
        {
            nameInput.interactable = false;
            removePlayerButton.interactable = NetworkServer.active;

            ChangeReadyButtonColor(NotReadyColor);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "...";
            readyButton.interactable = false;

            //charctorButton.interactable = false;

            OnClientReady(false);
        }

        void SetupLocalPlayer()
        {
            nameInput.interactable = true;
            remoteIcone.gameObject.SetActive(false);
            localIcone.gameObject.SetActive(true);

            CheckRemoveButton();

            //if (playerColor == Color.white)
            //{
            //    CmdColorChange();
            //}

            ChangeReadyButtonColor(JoinColor);

            readyButton.transform.GetChild(0).GetComponent<Text>().text = "JOIN";
            readyButton.interactable = true;

            //charctorButton.transform.GetChild(0).GetComponent<Text>().text = CharIdx.ToString();

            //have to use child count of player prefab already setup as "this.slot" is not set yet
            if (playerName == "")
                CmdNameChanged("Player" + (LobbyPlayerList._instance.playerListContentTransform.childCount-1));

            //we switch from simple name display to name input
            colorButton.interactable = true;
            nameInput.interactable = true;

            nameInput.onEndEdit.RemoveAllListeners();
            nameInput.onEndEdit.AddListener(OnNameChanged);

            //色は変えなくていいだろ
            colorButton.onClick.RemoveAllListeners();
            colorButton.onClick.AddListener(OnColorClicked);

            readyButton.onClick.RemoveAllListeners();
            readyButton.onClick.AddListener(OnReadyClicked);


            charctorButton.interactable = true;
            //キャラをかえようぜ
            CmdOnChangeButtonImage(buttonImageString);
            charctorButton.onClick.RemoveAllListeners();
            charctorButton.onClick.AddListener(OnChangeCharButton);

            //when OnClientEnterLobby is called, the loval PlayerController is not yet created, so we need to redo that here to disable
            //the add button if we reach maxLocalPlayer. We pass 0, as it was already counted on OnClientEnterLobby
            if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(0);
        }

        //This enable/disable the remove button depending on if that is the only local player or not
        public void CheckRemoveButton()
        {
            if (!isLocalPlayer)
                return;

            int localPlayerCount = 0;
            foreach (PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            removePlayerButton.interactable = localPlayerCount > 1;
        }

        public override void OnClientReady(bool readyState)
        {
            if (readyState)
            {
                ChangeReadyButtonColor(TransparentColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = "READY";
                textComponent.color = ReadyColor;
                readyButton.interactable = false;
                colorButton.interactable = false;
                nameInput.interactable = false;
            }
            else
            {
                ChangeReadyButtonColor(isLocalPlayer ? JoinColor : NotReadyColor);

                Text textComponent = readyButton.transform.GetChild(0).GetComponent<Text>();
                textComponent.text = isLocalPlayer ? "JOIN" : "...";
                textComponent.color = Color.white;
                readyButton.interactable = isLocalPlayer;
                colorButton.interactable = isLocalPlayer;
                nameInput.interactable = isLocalPlayer;
            }
        }

        public void OnPlayerListChanged(int idx)
        { 
            GetComponent<Image>().color = (idx % 2 == 0) ? EvenRowColor : OddRowColor;
        }

        ///===== callback from sync var

        public void OnMyName(string newName)
        {
            playerName = newName;
            nameInput.text = playerName;
        }

        //public void OnMyColor(Color newColor)
        //{
        //    playerColor = newColor;
        //    colorButton.GetComponent<Image>().color = newColor;
        //}

        //public void OnMyCharctor(GameObject newChar)
        //{
        //    playerCharType = newChar;
        //}

        public void OnMyCharNumber(int num)
        {
            charctorButton.transform.GetChild(0).GetComponent<Text>().text = num.ToString();
        }

        //===== UI Handler

        //Note that those handler use Command function, as we need to change the value on the server not locally
        //so that all client get the new value throught syncvar
        public void OnColorClicked()
        {
            //CmdColorChange();
        }

        public void OnReadyClicked()
        {
            SendReadyToBeginMessage();
        }

        public void OnNameChanged(string str)
        {
            CmdNameChanged(str);
        }

        //public void OnCharctorClicked()
        //{
        //    CmdCharChanged(1);
        //}

        public void OnChangeCharButton()
        {
            var image = charctorButton.GetComponent<Image>();

            int index = 0;

            //フェイス１のとき
            if(image.sprite == face1)
            {
                //image.sprite = face2;
                //index = 1;
                //buttonImageString = "face2";

                //これに
                image.sprite = face1;
                index = 0;
                buttonImageString = "face1";
            }
            //フェイス２のとき
            else if (image.sprite == face2)
            {
                //image.sprite = face3;
                //index = 2;
                //buttonImageString = "face3";

                //これに
                image.sprite = face1;
                index = 0;
                buttonImageString = "face1";
            }
            //フェイス３のとき
            else if (image.sprite == face3)
            {
                //image.sprite = face4;
                //index = 3;
                //buttonImageString = "face4";

                //これに
                image.sprite = face1;
                index = 0;
                buttonImageString = "face1";
            }
            //フェイス４のとき
            else if (image.sprite == face4)
            {
                //image.sprite = face1;
                //index = 0;
                //buttonImageString = "face1";

                //これに
                image.sprite = face1;
                index = 0;
                buttonImageString = "face1";
            }
            else
            {
                //ここには入りません
            }

            //サーバーでキャラの切り替え
            CmdCharChanged(index);
            //ボタンイメージの同期
            CmdOnChangeButtonImage(buttonImageString);
        }

        public void OnChangeButtonImage(string imageString)
        {
            if (imageString == "face1")
            {
                charctorButton.GetComponent<Image>().sprite = face1;
            }
            else if (imageString == "face2")
            {
                charctorButton.GetComponent<Image>().sprite = face2;
            }
            else if (imageString == "face3")
            {
                charctorButton.GetComponent<Image>().sprite = face3;
            }
            else if (imageString == "face4")
            {
                charctorButton.GetComponent<Image>().sprite = face4;
            }
            else
            {

            }
        }


        public void OnRemovePlayerClick()
        {
            if (isLocalPlayer)
            {
                RemovePlayer();
            }
            else if (isServer)
            {
                LobbyManager.s_Singleton.KickPlayer(connectionToClient);
            }
                
        }

        public void ToggleJoinButton(bool enabled)
        {
            readyButton.gameObject.SetActive(enabled);
            waitingPlayerButton.gameObject.SetActive(!enabled);
        }

        [ClientRpc]
        public void RpcUpdateCountdown(int countdown)
        {
            LobbyManager.s_Singleton.countdownPanel.UIText.text = "Match Starting in " + countdown;
            LobbyManager.s_Singleton.countdownPanel.gameObject.SetActive(countdown != 0);
        }

        [ClientRpc]
        public void RpcUpdateRemoveButton()
        {
            CheckRemoveButton();
        }

        //====== Server Command

        //色はいらないです
        //[Command]
        //public void CmdColorChange()
        //{
        //    int idx = System.Array.IndexOf(Colors, playerColor);

        //    int inUseIdx = _colorInUse.IndexOf(idx);

        //    if (idx < 0) idx = 0;

        //    idx = (idx + 1) % Colors.Length;

        //    bool alreadyInUse = false;

        //    do
        //    {
        //        alreadyInUse = false;
        //        for (int i = 0; i < _colorInUse.Count; ++i)
        //        {
        //            if (_colorInUse[i] == idx)
        //            {//that color is already in use
        //                alreadyInUse = true;
        //                idx = (idx + 1) % Colors.Length;
        //            }
        //        }
        //    }
        //    while (alreadyInUse);

        //    if (inUseIdx >= 0)
        //    {//if we already add an entry in the colorTabs, we change it
        //        _colorInUse[inUseIdx] = idx;
        //    }
        //    else
        //    {//else we add it
        //        _colorInUse.Add(idx);
        //    }

        //    playerColor = Colors[idx];
        //}

        [Command]
        public void CmdNameChanged(string name)
        {
            playerName = name;
        }

        //キャラクター切り替えよう
        [Command]
        void CmdCharChanged(int index)
        {
            //クライアントに対応するキャラを割り当てる
            LobbyManager.s_Singleton.SetPlayer(GetComponent<NetworkIdentity>().connectionToClient,index);
        }

        //サーバー側でイメージ変更
        [Command]
        public void CmdOnChangeButtonImage(string imageString)
        {
            if(imageString == "face1")
            {
                charctorButton.GetComponent<Image>().sprite = face1;
            }
            else if (imageString == "face2")
            {
                charctorButton.GetComponent<Image>().sprite = face2;
            }
            else if (imageString == "face3")
            {
                charctorButton.GetComponent<Image>().sprite = face3;
            }
            else if (imageString == "face4")
            {
                charctorButton.GetComponent<Image>().sprite = face4;
            }
            else
            {

            }

            buttonImageString = imageString;
        }

        //Cleanup thing when get destroy (which happen when client kick or disconnect)
        public void OnDestroy()
        {
            //色
            //LobbyPlayerList._instance.RemovePlayer(this);
            //if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(-1);

            //int idx = System.Array.IndexOf(Colors, playerColor);

            //if (idx < 0)
            //    return;

            //for (int i = 0; i < _colorInUse.Count; ++i)
            //{
            //    if (_colorInUse[i] == idx)
            //    {//that color is already in use
            //        _colorInUse.RemoveAt(i);
            //        break;
            //    }
            //}

            //キャラモデル
           // LobbyPlayerList._instance.RemovePlayer(this);
           // if (LobbyManager.s_Singleton != null) LobbyManager.s_Singleton.OnPlayersNumberModified(-1);

           //CharIdx = System.Array.IndexOf(characterTypeModels, playerCharType);

           // if (CharIdx < 0)
           //     return;

           // for (int i = 0; i < charInUse.Count; ++i)
           // {
           //     if (charInUse[i] == CharIdx)
           //     {//that color is already in use
           //         charInUse.RemoveAt(i);
           //         break;
           //     }
           // }
        }
    }
}
