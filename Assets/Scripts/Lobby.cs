using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using TMPro;

public class Lobby : MonoBehaviour
{

    [SerializeField] LocalNextButton localNextButton;
    [SerializeField] GameObject playerUIPrefab;

    string serverName;
    int playerNumber;
    bool pokerPlanning;
    int difficulty;
    int userStory;

    List<string> playersName;
    List<GameObject> playersUI;

    [SerializeField] TMP_Text serverNameOut;
    [SerializeField] TMP_Text pokerPlanningOut;
    [SerializeField] TMP_Text difficultyOut;
    [SerializeField] TMP_Text userStoryOut;
    [SerializeField] TMP_Text playerNumberOut;

    // Update is called once per frame
    void Start() {
        
    }
    void Update()
    {
        this.serverName = localNextButton.GetServerName();
        this.playerNumber = localNextButton.GetPlayerNumber();
        this.pokerPlanning = localNextButton.GetPokerPlanning();
        this.difficulty = localNextButton.GetDifficulty();
        this.userStory = localNextButton.GetUserStory();

        this.serverNameOut.text = this.serverName;
        this.pokerPlanningOut.text = GetPokerPlanning(this.pokerPlanning);
        this.difficultyOut.text = GetDifficulty(this.difficulty);
        this.userStoryOut.text = GetUserStory(this.userStory);
        this.playerNumberOut.text = this.playerNumber.ToString();

        (this.playersName, this.playersUI) = CreatePlayers(this.playerNumber);
    }

    (List<string>, List<GameObject>) CreatePlayers(int playerNumber){
        List<string> players = new List<string>();
        List<GameObject> playersUI = new List<GameObject>();
        for (int i = 0; i < playerNumber; i++){
            // GameObject player = Instantiate(playerUIPrefab);
            // playersUI.Add(player);
            players.Add("Player " + i.ToString());
        }
        return (players, null);
    }

    string GetDifficulty(int difficulty){
        switch (difficulty){
            case 0:
                return "EASY";
            case 1:
                return "MEDIUM";
            default:
                return "HARD";
        }
    }

    string GetUserStory(int userStory){
        switch (userStory){
            case 0:
                return "GIFT SHOP";
            case 1:
                return "DIET COACH";
            case 2:
                return "TRAVEL DIARY";
            default:
                return "CUSTOM";
        }
    }

    string GetPokerPlanning(bool pokerPlanning){
        if (pokerPlanning){
            return "True";
        }
        return "False";
    }
}
