using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DailyPlayer : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    public Player player;
    private ChoosePlayer choosePlayer;
    private Color BLUE = new Color32(88, 136, 199, 255);

    void Update(){
        if (choosePlayer is not null){
            if (choosePlayer.playerChoosed == player){
                this.gameObject.GetComponent<Image>().color = BLUE;
                playerName.color = Color.white;
            } else {
                this.gameObject.GetComponent<Image>().color = Color.white;
                playerName.color = Color.black;
            }
        }
    }
    public void OnClick(){
        this.choosePlayer.playerChoosed = this.player;
    }
    public void Fill(Player player, ChoosePlayer choosePlayer){
        this.player = player;
        this.playerName.text = player.userName;
        this.choosePlayer = choosePlayer;
    }
}
