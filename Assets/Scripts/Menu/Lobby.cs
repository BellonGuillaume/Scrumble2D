using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class Lobby : MonoBehaviour
{

    [SerializeField] LocalNextButton localNextButton;
    [SerializeField] GameObject playerUIPrefab;
    [SerializeField] GameObject gridLayout;
    [SerializeField] Button playButton;
    [SerializeField] Button removePlayerButton;
    [SerializeField] Button addPlayerButton;

    string serverName;
    int playerNumber;
    bool pokerPlanning;
    int difficulty;
    int userStory;

    List<string> playersName = new List<string>();
    List<GameObject> playersUI = new List<GameObject>();

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
        this.playerNumber = playersUI.Count;
        this.pokerPlanning = localNextButton.GetPokerPlanning();
        this.difficulty = localNextButton.GetDifficulty();
        this.userStory = localNextButton.GetUserStory();

        this.serverNameOut.text = this.serverName;
        this.pokerPlanningOut.text = GetPokerPlanning(this.pokerPlanning);
        this.difficultyOut.text = GetDifficulty(this.difficulty);
        this.userStoryOut.text = GetUserStory(this.userStory);
        this.playerNumberOut.text = this.playerNumber.ToString();

        if (playersUI.Count <= 0){
            removePlayerButton.interactable = false;
            addPlayerButton.interactable = true;
        } else if (playersUI.Count >= 9){
            removePlayerButton.interactable = true;
            addPlayerButton.interactable = false;
        } else {
            removePlayerButton.interactable = true;
            addPlayerButton.interactable = true;
        }

        UpdateNames();
        
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

    public void AddPlayer(){
        GameObject player = Instantiate(playerUIPrefab) as GameObject;
        player.transform.SetParent(gridLayout.transform);
        playersUI.Add(player);

        UIPlayer playerUI = player.GetComponent<UIPlayer>();
        playerUI.text.text = "Player " + playersUI.Count + " :";
        playerUI.inputField.placeholder.GetComponent<TMP_Text>().text = "Player " + playersUI.Count;
        this.playersName.Add("Player " + playersUI.Count);
    }

    public void RemovePLayer(){
        Destroy(playersUI.LastOrDefault());
        playersUI.RemoveAt(playersUI.Count - 1);
    }

    void UpdateNames(){
        for (int i = 0; i < playersUI.Count; i++){
            GameObject player = playersUI[i];
            UIPlayer playerUI = player.GetComponent<UIPlayer>();
            string name = playerUI.inputField.text;
            string placeholder = playerUI.inputField.placeholder.GetComponent<TMP_Text>().text;
            if (string.IsNullOrWhiteSpace(name)){
                this.playersName[i] = placeholder;
            } else {
                this.playersName[i] = name;
            }
        }
    }

    public void LaunchGame(){
        StateManager.difficulty = this.difficultyOut.text;
        StateManager.category = this.userStoryOut.text;
        StateManager.gameName = this.serverNameOut.text;
        StateManager.pokerPlanning = this.pokerPlanning;
        StateManager.CreateUserStories(StateManager.category);
        StateManager.CreatePlayers(this.playersName);
        if(this.pokerPlanning){
            StateManager.gameState = StateManager.GameState.POKER_PLANNING;
        } else {
            StateManager.gameState = StateManager.GameState.INITIALISATION;
        }

        Debug.Log($"Here are the values :\n" +
                    $"Difficulty : {StateManager.difficulty}\n" +
                    $"Category : {StateManager.category}\n" +
                    $"GameName : {StateManager.gameName}\n" +
                    $"PokerPlanning : {StateManager.pokerPlanning}\n" +
                    $"State  : {StateManager.gameState}\n"
                );
        SceneManager.LoadScene("Game");
    }
}
