using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    public static Player localPlayer;
    private string ip;
    [SyncVar] public string matchId;
    NetworkMatch networkMatch;

    void Start() {
        if(isLocalPlayer){
            localPlayer = this;
            Debug.Log($"Local Player");
        } else {
            Debug.Log($"Not local Player");
        }
        networkMatch = GetComponent<NetworkMatch>();
        ip = "localhost";
    }
    // HOST PART
    public void HostGame(){
        string matchId = MatchMaker.GetRandomMatchId();
        CmdHostGame(matchId);
    }

    [Command]
    void CmdHostGame(string _matchId){
        matchId = _matchId;
        if(MatchMaker.matchMaker.HostGame(_matchId, gameObject, ip)){
            Debug.Log($"<color=green>Game hosted successfully</color>");
            networkMatch.matchId = _matchId.ToGuid();
            TargetHostGame(true, _matchId);
        } else {
            Debug.Log($"<color=red>Game not hosted</color>");
            TargetHostGame(false, _matchId);
        }
    }

    [TargetRpc]
    void TargetHostGame(bool success, string _matchId){
        Debug.Log($"MatchID : {matchId} == {_matchId}");
        UILobby.uILobby.HostSuccess(success);
    }

    // JOIN PART
    public void JoinGame(string _matchId){
        CmdJoinGame(_matchId);
    }

    [Command]
    void CmdJoinGame(string _matchId){
        matchId = _matchId;
        if(MatchMaker.matchMaker.JoinGame(_matchId, gameObject)){
            Debug.Log($"<color=green>Game joined successfully</color>");
            networkMatch.matchId = _matchId.ToGuid();
            TargetJoinGame(true, _matchId);
        } else {
            Debug.Log($"<color=red>Game not joined</color>");
            TargetJoinGame(false, _matchId);
        }
    }

    [TargetRpc]
    void TargetJoinGame(bool success, string _matchId){
        Debug.Log($"MatchID : {matchId} == {_matchId}");
        UILobby.uILobby.JoinSuccess(success);
    }
}
