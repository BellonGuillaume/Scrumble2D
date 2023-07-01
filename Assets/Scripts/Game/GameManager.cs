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

    string GAME_STATE;

    Animator popUpAnimator;

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
        Debug.Log("ENTERRING START");
        Debug.Log("CHECKING STATE_MANAGER");
        if (StateManager.gameState != StateManager.GameState.INITIALISATION &&
            StateManager.gameState != StateManager.GameState.POKER_PLANNING){
            InitState();
        }
        Debug.Log("INITIALIZING ASSETS");
        this.popUpAnimator = popUpGO.GetComponent<Animator>();
        this.cardPicker = cardPick.GetComponent<CardPicker>();
        CreateUsers();
        CreateUserStories(StateManager.userStory);
        // CreateDailyCards();
        CreateProblemCards();
        // CreateReviewCards();
        Debug.Log("ASSETS INITIALIZED");
        StateManager.gameState = StateManager.GameState.PLAYER_TURN;
        Debug.Log("BEGIN FIRST TURN");
        BeginTurn(players[0]);
    }

    #region Turn
    void BeginTurn(Player player){
        this.currentPlayer = player;
        // StartTurnAnimation(player);
        StartCoroutine(StartChoiceTaskDebt());
        // UpdateRollView();
        StartCoroutine(StartRollDice());
        // if result == 1 and alreadyReRoll == false
        // --> StartRerollAnimation();
        // -> back to StartChoiceTaskDebt();
        // else if result == 6
        // --> StartProblemAnimation();
        // --> PickProblemCard();
        // if choice == debt
        // --> StartChangeDebtAnimation();
        // else
        // --> UpdateTasksView();
        // --> WaitForValidation();
    }

    IEnumerator StartChoiceTaskDebt(){
        while(StateManager.gameState != StateManager.GameState.PLAYER_TURN || StateManager.turnState != StateManager.TurnState.CHOICE){
            yield return null;
        }
        Debug.Log("-STARTING CHOICE");

        StartCoroutine(PopUpAnimateIn(this.turn));

        while(this.popUpAnimator.GetBool("TASK") == false && this.popUpAnimator.GetBool("DEBT") == false){
            yield return null;
        }
        if (this.popUpAnimator.GetBool("TASK")){
            this.popUpAnimator.ResetTrigger("TASK");

            StateManager.taskOrDebt = "TASK";
        } else if (this.popUpAnimator.GetBool("DEBT")){
            this.popUpAnimator.ResetTrigger("DEBT");

            StateManager.taskOrDebt = "DEBT";
        } else {
            Debug.Log("Nor Task nor debt are clicked");
            this.popUpAnimator.ResetTrigger("TASK");
            this.popUpAnimator.ResetTrigger("DEBT");
        }
        Debug.Log($"-CHOICE MADE : {StateManager.taskOrDebt}");
        
        StartCoroutine(PopUpAnimateOut(this.turn));
        Debug.Log("-END OF CHOICE");

        StateManager.turnState = StateManager.TurnState.ROLL;
    }
    IEnumerator StartRollDice(){
        while(StateManager.gameState != StateManager.GameState.PLAYER_TURN || StateManager.turnState != StateManager.TurnState.ROLL ){
            yield return null;
        }
        Debug.Log("-START ROLLING DICE");

        StartCoroutine(PopUpAnimateIn(this.roll));

        while(this.popUpAnimator.GetBool("ROLL") == false){
            yield return null;
        }
        Debug.Log("-DICE READY TO ROLL");

        StateManager.diceResult = this.roll.GetComponent<UIDice>().RollDice();
        Debug.Log($"DICE ROLLED : {StateManager.diceResult}");
        this.popUpAnimator.ResetTrigger("ROLL");
        Debug.Log("-END ROLL");
        
        StartCoroutine(PopUpAnimateOut(this.roll));

        StartCoroutine(EndPopUp());

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

    #region Utils
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
        // this.turn.SetActive(false);
        // this.roll.SetActive(true);
    }

    public void OutClick(){
        this.popUpGO.SetActive(false);
        this.cardPick.SetActive(false);
        this.turn.SetActive(false);
        this.roll.SetActive(false);
    }

    void InitState(){
        Debug.Log("-STATE_MANAGER INITIALIAZING");
        // StateManager.difficulty = StateManager.difficulty.EASY; TODO
        StateManager.difficulty = "EASY";
        StateManager.userStory = "GIFT SHOP";
        StateManager.gameName = "";
        StateManager.pokerPlanning = false;
        StateManager.playerNames = new List<string>{"Alice", "Bob", "Charles"};

        StateManager.gameState = StateManager.GameState.INITIALISATION;
        Debug.Log("-STATE_MANAGER INITIALIZED");
    }
    #endregion

    #region Animations
    IEnumerator PopUpAnimateIn(GameObject go){
        go.SetActive(true);
        this.popUpGO.SetActive(true);
        yield break;
    }
    IEnumerator PopUpAnimateOut(GameObject go){
        go.SetActive(false);
        yield break;
    }
    IEnumerator EndPopUp(){
        yield return new WaitForSeconds(0.5f);
        this.cardPick.SetActive(false);
        this.turn.SetActive(false);
        this.roll.SetActive(false);
        this.popUpGO.SetActive(false);
        yield break;
    }
    #endregion
}
