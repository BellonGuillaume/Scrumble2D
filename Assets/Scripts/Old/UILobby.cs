using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;


public class UILobby : MonoBehaviour
{
    public static UILobby uILobby;
    [SerializeField] GameObject lobbyUI;
    [SerializeField] GameObject hostJoinUI;
    [SerializeField] TextMeshProUGUI ipText;
    [SerializeField] TMP_InputField ipToJoin;
    NetworkManager networkManager;
    void Start(){
        uILobby = this;
        ipText.text = NetworkManager.singleton.networkAddress;
    }
    public void Host(){
        NetworkManager.singleton.networkAddress = ipText.text;
        Debug.Log($"About to start the server with address : {ipText.text}");
        NetworkManager.singleton.StartHost();
        PlayerOld.localPlayer.HostGame();
    }

    public void Join(){
        string text = ipToJoin.text;
        PlayerOld.localPlayer.JoinGame(text);
    }

    public void HostSuccess(bool success){
        if(success){
            lobbyUI.SetActive(true);
            hostJoinUI.SetActive(false);
        } else {
            Debug.Log($"Cannot host");
        }
    }

    public void JoinSuccess(bool success){
        if(success){
            lobbyUI.SetActive(true);
            hostJoinUI.SetActive(false);
        } else {
            Debug.Log($"Cannot join");
        }
    }
}
