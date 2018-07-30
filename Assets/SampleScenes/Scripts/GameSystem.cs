using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class GameSystem : MonoBehaviour
{

    //　スタートボタンを押したら実行する
    public void GameStart()
    {
        SceneManager.LoadScene("NetworkLobby");
    }
    //　ゲーム終了ボタンを押したら実行する
    public void GameEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
		Application.OpenURL("http://www.yahoo.co.jp/");
#else
		Application.Quit();
#endif
    }
    public void Title()
    {
        SceneManager.LoadScene("title");
        //LobbyManager.s_Singleton.ServerReturnToLobby();
    }
    public void Lobby()
    {
        LobbyManager.s_Singleton.ServerReturnToLobby();
    }
}