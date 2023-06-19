using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Mirror;

[System.Serializable]
public class Match{
    public string ip;
    public string matchId;
    public GameObject host;
    public List<GameObject> players = new List<GameObject>();

    public Match(string matchId, GameObject player, string ip){
        this.matchId =  matchId;
        players.Add(player);
        host = player;
        this.ip = ip;
    }

    public Match(){ }
}

public class MatchMaker : NetworkBehaviour
{
    public static MatchMaker matchMaker;

    public readonly SyncList<Match> matches = new SyncList<Match>();
    public readonly SyncList<string> matchIds = new SyncList<string>();

    void Start(){
        matchMaker = this;
    }
    public bool HostGame(string _matchId, GameObject _player, string _ip){
        if(!matchIds.Contains(_matchId)){
            matchIds.Add(_matchId);
            matches.Add(new Match(_matchId, _player, _ip));
            Debug.Log($"Match generated");
            return true;
        } else {
            Debug.Log($"Match ID already exists");
            return false;
        }
    }

        public bool JoinGame(string _matchId, GameObject _player){
        if(matchIds.Contains(_matchId)){
            for (int i = 0; i < matches.Count; i++)
            {
                if(matches[i].matchId == _matchId){
                    matches[i].players.Add(_player);
                    break;
                }
            }
            Debug.Log($"Match joined");
            return true;
        } else {
            Debug.Log($"Match ID does not exists");
            return false;
        }
    }
    public static string GetRandomMatchId(){
        string _id = string.Empty;
        for (int i = 0; i < 6; i++){
            int random = UnityEngine.Random.Range(0, 36);
            if (random < 26){
                _id += (char)(random + 65);
            } else {
                _id += (random - 26).ToString();
            }
        }
        Debug.Log($"Random Match ID : {_id}");
        return _id;
    }
}

public static class MatchExtensions{
    public static Guid ToGuid(this string id){
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hashBytes = provider.ComputeHash(inputBytes);

        return new Guid(hashBytes);
    }
}
