using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class Lobby : MonoBehaviour
{

    [SerializeField] Local local;
    [SerializeField] GameObject playerUIPrefab;
    [SerializeField] GameObject gridLayout;
    [SerializeField] Button playButton;
    [SerializeField] Button removePlayerButton;
    [SerializeField] Button addPlayerButton;
    [SerializeField] LocalizedString[] difficultyOptions;
    [SerializeField] LocalizedString[] userStoryOptions;
    [SerializeField] LocalizedString[] pokerPlanningOptions;
    [SerializeField] LocalizedString playerText;

    string serverName;
    int playerNumber;
    bool pokerPlanning;
    StateManager.Difficulty difficulty;
    StateManager.Category userStory;

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
        if (StateManager.language != LocalizationSettings.SelectedLocale){
            RefreshPlayersUI();
            StateManager.language = LocalizationSettings.SelectedLocale;
        }
        this.serverName = local.GetServerName();
        this.playerNumber = playersUI.Count;
        this.pokerPlanning = local.GetPokerPlanning();
        this.difficulty = local.GetDifficulty();
        this.userStory = local.GetUserStory();

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
        if (playersUI.Count >= 3 && playersUI.Count <= 9)
            playButton.interactable = true;
        else
            playButton.interactable = false;

        UpdateNames();
        
    }

    string GetDifficulty(StateManager.Difficulty difficulty){
        switch (difficulty){
            case StateManager.Difficulty.EASY:
                return difficultyOptions[0].GetLocalizedString();
            case StateManager.Difficulty.HARD:
                return difficultyOptions[2].GetLocalizedString();
            default:
                return difficultyOptions[1].GetLocalizedString();
        }
    }

    string GetUserStory(StateManager.Category userStory){
        switch (userStory){
            case StateManager.Category.GIFT_SHOP:
                return userStoryOptions[0].GetLocalizedString();
            case StateManager.Category.DIET_COACH:
                return userStoryOptions[1].GetLocalizedString();
            case StateManager.Category.TRAVEL_DIARY:
                return userStoryOptions[2].GetLocalizedString();
            case StateManager.Category.KNOWLEDGE_MANAGEMENT:
                return userStoryOptions[3].GetLocalizedString();
            case StateManager.Category.CUSTOM:
                return userStoryOptions[4].GetLocalizedString();
            default:
                return "";
        }
    }

    string GetPokerPlanning(bool pokerPlanning){
        if (pokerPlanning){
            return pokerPlanningOptions[0].GetLocalizedString();
        }
        return pokerPlanningOptions[1].GetLocalizedString();
    }

    public void AddPlayer(){
        GameObject player = Instantiate(playerUIPrefab) as GameObject;
        player.transform.SetParent(gridLayout.transform);
        player.transform.localScale = Vector3.one;
        playersUI.Add(player);

        UIPlayer playerUI = player.GetComponent<UIPlayer>();
        playerUI.text.text = playerText.GetLocalizedString() + " " + playersUI.Count + " :";
        playerUI.inputField.placeholder.GetComponent<TMP_Text>().text = playerText.GetLocalizedString() + " " + playersUI.Count;
        this.playersName.Add(playerText.GetLocalizedString() + " " + playersUI.Count);
    }

    public void RemovePLayer(){
        GameObject player = playersUI[playersUI.Count-1];
        playersUI.RemoveAt(playersUI.Count - 1);
        playersName.RemoveAt(playersName.Count - 1);
        Destroy(player);
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
        StateManager.difficulty = this.difficulty;
        StateManager.category = this.userStory;
        StateManager.gameName = this.serverNameOut.text;
        StateManager.pokerPlanning = this.pokerPlanning;
        StateManager.CreatePlayers(this.playersName);
        if (StateManager.category == StateManager.Category.CUSTOM){
            StateManager.gameState = StateManager.GameState.CUSTOM_POKER_PLANNING;
            SceneManager.LoadSceneAsync("CustomPokerPlanning");
        } else {
            StateManager.CreateUserStories(StateManager.category);
            if(this.pokerPlanning){
                StateManager.gameState = StateManager.GameState.POKER_PLANNING;
                SceneManager.LoadSceneAsync("PokerPlanning");
            } else {
                foreach (UserStory userStory in StateManager.userStories){
                    userStory.size = userStory.defaultSize;
                }
                StateManager.gameState = StateManager.GameState.INITIALISATION;
                SceneManager.LoadSceneAsync("Game");
            }
        }
        
    }

    public void RefreshPlayersUI(){
        int count = 1;
        foreach (GameObject playerUI in playersUI){
            playerUI.transform.GetChild(1).GetComponent<TMP_Text>().text = playerText.GetLocalizedString() + " " + count.ToString() + " :";
            if (playerUI.transform.GetChild(0).GetComponent<TMP_InputField>().text == ""){

                playerUI.transform.GetChild(0).GetComponent<TMP_InputField>().placeholder.GetComponent<TMP_Text>().text = playerText.GetLocalizedString() + " " + count.ToString();
            }
            count++;
        }
    }
}
