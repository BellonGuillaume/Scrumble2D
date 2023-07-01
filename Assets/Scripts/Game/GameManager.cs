using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject popUpGO;
    [SerializeField] GameObject cardPick;
    [SerializeField] GameObject turn;
    [SerializeField] GameObject roll;
    [SerializeField] GameObject userStoryUIPrefab;
    List<UserStory> userStories;
    List<Player> players = new List<Player>();
    List<Card> dailyCards;
    List<Card> problemCards;
    List<Card> reviewCards;

    Player currentPlayer;
    CardPicker cardPicker;


    // Start is called before the first frame update
    void Start()
    {
        this.cardPicker = cardPick.GetComponent<CardPicker>();
        Debug.Log(this.cardPicker is null);
        CreateUsers();
        CreateUserStories(StateManager.userStory);
        // CreateDailyCards();
        CreateProblemCards();
        // CreateReviewCards();
        BeginTurn(players[0]);
    }

    #region Turn
    void BeginTurn(Player player){
        this.currentPlayer = player;
        // StartTurnAnimation();
        // StartChoiceTaskDebt();
        // --> StartRollDice();
        //

    }
    #endregion

    #region Initialisation
    void CreateUsers(){
        if (StateManager.playerNames is null){
            this.players.Add(new Player("Alice", 1, 2));
            this.players.Add(new Player("Bob", 2, 3));
            this.players.Add(new Player("Charles", 3, 1));
            return;
        }
        for (int i = 0; i < StateManager.playerNames.Count; i++){
            if(i + 1 == StateManager.playerNames.Count){
                this.players.Add(new Player(StateManager.playerNames[i], i+1, 1));
            } else {
                this.players.Add(new Player(StateManager.playerNames[i], i+1, i+2));
            }
        }
    }

    void CreateUserStories(string userStory){
        string path = Application.dataPath;
        if (StateManager.userStory == "GIFT SHOP"){
            path += "/UserStories/GIFT SHOP.json";
        } else if (StateManager.userStory == "DIET COACH"){
            path += "/UserStories/DIET COACH.json";
        } else if (StateManager.userStory == "TRAVEL DIARY"){
            path += "/UserStories/TRAVEL DIARY.json";
        } else {
            path += "/UserStories/GIFT SHOP.json";
            // throw new System.Exception();
        }
        string userStoriesStr = File.ReadAllText(path);
        this.userStories = JsonConvert.DeserializeObject<List<UserStory>>(userStoriesStr);

        // Debug.Log("Values of the user stories :\n");
        // for (int i = 0; i < userStories.Count; i++){
        //     Debug.Log(userStories[i].ToString());
        // }
    }
    void CreateDailyCards(){
        string path = Application.dataPath + "/Cards/DailyCards.json";
        string dailyCardsStr = File.ReadAllText(path);
        this.dailyCards = JsonConvert.DeserializeObject<List<Card>>(dailyCardsStr);
    }

    void CreateProblemCards(){
        string path = Application.dataPath + "/Cards/ProblemCards.json";
        string problemCardsStr = File.ReadAllText(path);
        this.problemCards = JsonConvert.DeserializeObject<List<Card>>(problemCardsStr);

        // Debug.Log("Here are the problem cards");
        // for (int i = 0; i < problemCards.Count; i++){
        //     Debug.Log(problemCards[i].ToString());
        // }
    }

    void CreateReviewCards(){
        string path = Application.dataPath + "/Cards/ReviewCards.json";
        string reviewCardsStr = File.ReadAllText(path);
        this.reviewCards = JsonConvert.DeserializeObject<List<Card>>(reviewCardsStr);
    }
    #endregion

    public void PickDailyCard(){
        CardPicker.typeOfCard = "DAILY";
        this.cardPick.SetActive(true);
        cardPicker.flipToVersoAll();
        this.popUpGO.SetActive(true);
    }

    public void PickProblemCard(){
        CardPicker.typeOfCard = "PROBLEM";
        this.cardPick.SetActive(true);
        cardPicker.flipToVersoAll();
        this.popUpGO.SetActive(true);
    }

    public void PickReviewCard(){
        CardPicker.typeOfCard = "REVIEW";
        this.cardPick.SetActive(true);
        cardPicker.flipToVersoAll();
        this.popUpGO.SetActive(true);
    }

    public void PlayTurn(){
        this.turn.SetActive(true);
        this.popUpGO.SetActive(true);
    }

    public void ChooseTaskOrDebt(string choice){
        this.turn.SetActive(false);
        this.roll.SetActive(true);
    }

    public void RollDice(){
        int result = Random.Range(1, 7);
        UIDice.currentFace = result;
    }

    public void OutClick(){
        this.popUpGO.SetActive(false);
        this.cardPick.SetActive(false);
        this.turn.SetActive(false);
        this.roll.SetActive(false);
    }
}
