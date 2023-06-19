using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HostJoinUI : MonoBehaviour
{
    [SerializeField] Button joinButton;
    [SerializeField] Button hostButton;
    [SerializeField] TMP_InputField inputUsername;

    void Update(){
        if(inputUsername.text != ""){
            joinButton.enabled = true;
            hostButton.enabled = true;
            joinButton.GetComponent<Image>().color = Color.white;
            hostButton.GetComponent<Image>().color = Color.white;
        } else {
            joinButton.enabled = false;
            hostButton.enabled = false;
            joinButton.GetComponent<Image>().color = Color.grey;
            hostButton.GetComponent<Image>().color = Color.grey;
        }
    }
}
