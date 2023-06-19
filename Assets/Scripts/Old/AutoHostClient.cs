using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AutoHostClient : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;

    // void Start(){
    //     if(!Application.isBatchMode){
    //         Debug.Log($"CLIENT BUILD");
    //         networkManager.StartClient();

    //     } else {
    //         Debug.Log($"SERVER BUILD");
    //     }
    // }

    public void JoinLocal(){
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }

    
}
