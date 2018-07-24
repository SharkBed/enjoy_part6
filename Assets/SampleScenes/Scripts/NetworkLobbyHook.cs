using UnityEngine;
using Prototype.NetworkLobby;
using System.Collections;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook 
{
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer>();
       
        NetWorkEDO netEDO= gamePlayer.GetComponent<NetWorkEDO>();

        netEDO.playerName = lobby.playerName;
        //netEDO.color = lobby.playerColor;

        //netEDO.playerModelObject = lobby.playerCharType;
        //netEDO.charctorNumbers = lobby.CharIdx;

        //体力値
        netEDO.lifeCount = 3;
    }
}
