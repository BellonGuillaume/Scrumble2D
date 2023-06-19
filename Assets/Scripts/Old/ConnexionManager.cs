using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Net;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;

public class ConnexionManager : MonoBehaviour
{
    [SerializeField] NetworkManager networkManager;
    [Scene] [SerializeField] string lobbyScene;

    public void JoinLocal(){
        networkManager.networkAddress = "localhost";
        Debug.Log($"IP Address : {networkManager.networkAddress}");
    }

    public void JoinLan(){
        networkManager.networkAddress = GetLocalIPv4();
        Debug.Log($"IP Address : {networkManager.networkAddress}");
    }

    public void JoinWan(){
        networkManager.networkAddress = GetExternalIPv4();
        Debug.Log($"IP Address : {networkManager.networkAddress}");
    }

    public string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.First(
                f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .ToString();
    }

    private string GetExternalIPv4(){
        var url = "https://api.ipify.org/";

        WebRequest request = WebRequest.Create(url);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream dataStream = response.GetResponseStream();
        using StreamReader reader = new StreamReader(dataStream);
        var ip = reader.ReadToEnd();
        reader.Close();

        return ip;
    }

    public void GoToLobby(){
        SceneManager.LoadScene(lobbyScene);
    }
    
}
