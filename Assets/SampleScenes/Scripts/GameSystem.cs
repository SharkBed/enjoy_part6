
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    public void Start()
    {
        AudioManager.Instance.PlayBGM("Title");
    }

    public void Update()
    {
    }

    //　スタートボタンを押したら実行する
    public void GameStart()
    {
        AudioManager.Instance.StopBGM();
        SceneManager.LoadScene("NetworkLobby");
    }
}