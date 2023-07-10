using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject popUpGO;
    [SerializeField] GameObject sidePopUp;
    [SerializeField] GameObject cardPick;
    [SerializeField] GameObject turn;
    [SerializeField] GameObject roll;
    [SerializeField] GameObject results;
    [SerializeField] GameObject userStoryUIPrefab;

    [SerializeField] Slider debtSlider;
    [SerializeField] Slider daySlider;

    Animator popUpAnimator;
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
        for (int i = 0; i < StateManager.userStories.Count; i++){
            Debug.Log(StateManager.userStories[i].ToString());
        }
        Debug.Log("INITIALIZING ASSETS");
        this.popUpAnimator = popUpGO.GetComponent<Animator>();
        this.cardPicker = cardPick.GetComponent<CardPicker>();
        // CreateDailyCards();
        CreateProblemCards();
        // CreateReviewCards();
        Debug.Log("ASSETS INITIALIZED");
        StateManager.gameState = StateManager.GameState.PLAYER_TURN;
        Debug.Log("BEGIN FIRST TURN");
        // BeginTurn(players[0]);
    }

    #region --------------------------------- Turn ---------------------------------
    // void BeginTurn(Player player){
    public void BeginTurn(){
        StateManager.turnState = StateManager.TurnState.CHOICE;
        Player player = StateManager.players[0];
        this.currentPlayer = player;
        // StartTurnAnimation(player);
        StartCoroutine(StartChoiceTaskDebt());
        // UpdateRollView();
        StartCoroutine(StartRollDice());
        StartCoroutine(StartShowResult());
        // if choice == debt
        // --> StartChangeDebtAnimation();
        // else
        // --> UpdateTasksView();
        // --> WaitForValidation();
        StartCoroutine(EndOfTurn());
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
            if(!StateManager.alreadyReRoll){
                StateManager.firstTaskOrDebtChoice = "TASK";
            } else {
                StateManager.secondTaskOrDebtChoice = "TASK";
            }
        } else if (this.popUpAnimator.GetBool("DEBT")){
            this.popUpAnimator.ResetTrigger("DEBT");
            if(!StateManager.alreadyReRoll){
                StateManager.firstTaskOrDebtChoice = "DEBT";
            } else {
                StateManager.secondTaskOrDebtChoice = "DEBT";
            }
        } else {
            Debug.Log("Nor Task nor debt are clicked");
            this.popUpAnimator.ResetTrigger("TASK");
            this.popUpAnimator.ResetTrigger("DEBT");
        }
        Debug.Log($"-CHOICE MADE : {StateManager.firstTaskOrDebtChoice}");
        
        StartCoroutine(PopUpAnimateOut(this.turn));
        Debug.Log("-END OF CHOICE");

        StateManager.turnState = StateManager.TurnState.ROLL;
    }
    IEnumerator StartRollDice(){
        while(StateManager.gameState != StateManager.GameState.PLAYER_TURN || StateManager.turnState != StateManager.TurnState.ROLL ){
            yield return null;
        }
        Debug.Log("-STARTING DICE");

        StartCoroutine(PopUpAnimateIn(this.roll));

        while(this.popUpAnimator.GetBool("ROLL") == false){
            yield return null;
        }
        Debug.Log("-DICE TRIGGER TO ROLL");

        int result = this.roll.GetComponent<UIDice>().RollDice();
        

        if (!StateManager.alreadyReRoll){
            StateManager.firstDiceResult = result;
            Debug.Log($"-FIRST DICE ROLLED : {StateManager.firstDiceResult}");
        }
        else {
            StateManager.secondDiceResult = result;
            Debug.Log($"-SECOND DICE ROLLED : {StateManager.secondDiceResult}");
        }

        this.popUpAnimator.ResetTrigger("ROLL");
        Debug.Log("-END ROLL");
        
        StartCoroutine(PopUpAnimateOut(this.roll));

        StateManager.turnState = StateManager.TurnState.RESULT;
    }
    IEnumerator StartShowResult(){
        while(StateManager.gameState != StateManager.GameState.PLAYER_TURN || StateManager.turnState != StateManager.TurnState.RESULT ){
            yield return null;
        }
        Debug.Log("-START RESULTS");
        if (StateManager.firstDiceResult == 1 && !StateManager.alreadyReRoll){
            Debug.Log("--RESULT => REROLL");
            // StartCoroutine(ReRollAnimation());
            StateManager.alreadyReRoll = true;
            StateManager.turnState = StateManager.TurnState.CHOICE;
            StartCoroutine(StartChoiceTaskDebt());
            StartCoroutine(StartRollDice());
            while(StateManager.gameState != StateManager.GameState.PLAYER_TURN || StateManager.turnState != StateManager.TurnState.RESULT ){
                yield return null;
            }
            Debug.Log("--END REROLL");
        }

        Debug.Log("-SHOWING RESULTS");
        StartCoroutine(PopUpAnimateIn(this.results));

        if (StateManager.firstDiceResult == 6 || StateManager.secondDiceResult == 6) {
            Debug.Log("--RESULT => PROBLEM");
            // StartCoroutine(ProblemAnimation());
            StartCoroutine(PopUpAnimateOut(this.results));
            StateManager.turnState = StateManager.TurnState.PROBLEM;
            StartCoroutine(PickProblemCard());
            while(StateManager.gameState != StateManager.GameState.PLAYER_TURN || StateManager.turnState != StateManager.TurnState.RESULT ){
                yield return null;
            }
            StartCoroutine(PopUpAnimateIn(this.results));

            // TODO : HANDLE PROBLEM RESULT

            Debug.Log("--END PROBLEM");
        }

        if(StateManager.alreadyReRoll){
            this.results.GetComponent<Results>().ChangeText($"Vous avez gagné {StateManager.firstDiceResult} {StateManager.firstTaskOrDebtChoice} " +
                                                            $"et {StateManager.secondDiceResult} {StateManager.secondTaskOrDebtChoice} ");
        } else {
            this.results.GetComponent<Results>().ChangeText($"Vous avez gagné {StateManager.firstDiceResult} {StateManager.firstTaskOrDebtChoice}");
        }

        yield return new WaitForSeconds(2);
        int debtCounter = 0;
        if (StateManager.firstTaskOrDebtChoice == "DEBT"){
            debtCounter += StateManager.firstDiceResult;
        }
        if(StateManager.alreadyReRoll && StateManager.secondTaskOrDebtChoice == "DEBT"){
            debtCounter += StateManager.secondDiceResult;
        }

        Debug.Log("-END RESULTS");

        StartCoroutine(PopUpAnimateOut(this.results));
        if (debtCounter > 0){
            StartCoroutine(AddToDebt(0-debtCounter));
        }
        StateManager.turnState = StateManager.TurnState.END_OF_TURN;
        
    }
    IEnumerator EndOfTurn(){
        while(StateManager.gameState != StateManager.GameState.PLAYER_TURN || StateManager.turnState != StateManager.TurnState.END_OF_TURN ){
            yield return null;
        }
        Debug.Log("-START END OF TURN");
        if (CardPicker.initialized){
            Debug.Log("--RESETING CARDPICKER");
            this.cardPick.GetComponent<CardPicker>().Reset();
            this.cardPick.SetActive(false);
            Debug.Log("--CARDPICKER RESETED");
        }
        this.turn.SetActive(false);
        this.roll.SetActive(false);
        if (Results.initialized){
            Debug.Log("--RESETING RESULTS");
            this.results.GetComponent<Results>().Reset();
            this.results.SetActive(false);
            Debug.Log("--RESULTS RESETED");
        }
        Debug.Log("--RESETING TURNSTATE");
        StateManager.ClearTurnState();
        Debug.Log("--TURNSTATE RESETED");

        Debug.Log("-END OF TURN");
        
        StartCoroutine(EndPopUp());
    }
    #endregion

    #region --------------------------------- Initialisation ---------------------------------
    void CreateDailyCards(){
        string path = Application.streamingAssetsPath + "/Cards/DailyCards.json";
        string dailyCardsStr = File.ReadAllText(path);
        this.dailyCards = JsonConvert.DeserializeObject<List<Card>>(dailyCardsStr);
    }

    void CreateProblemCards(){
        string path = Application.streamingAssetsPath + "/Cards/ProblemCards.json";
        string problemCardsStr = File.ReadAllText(path);
        this.problemCards = JsonConvert.DeserializeObject<List<Card>>(problemCardsStr);
    }

    void CreateReviewCards(){
        string path = Application.streamingAssetsPath + "/Cards/ReviewCards.json";
        string reviewCardsStr = File.ReadAllText(path);
        this.reviewCards = JsonConvert.DeserializeObject<List<Card>>(reviewCardsStr);
    }
    #endregion

    #region --------------------------------- Utils ---------------------------------
    public void PickDailyCard(){
        CardPicker.typeOfCard = "DAILY";
        int index = Random.Range(0, this.dailyCards.Count);
        CardPicker.cardDescription = this.dailyCards[index].description;
        CardPicker.cardResult = this.dailyCards[index].result;
        this.cardPick.SetActive(true);
        this.popUpGO.SetActive(true);
    }

    IEnumerator PickProblemCard(){
        Debug.Log("--START PICKPROBLEMCARD");
        CardPicker.typeOfCard = "PROBLEM";
        int index = Random.Range(0, this.problemCards.Count);
        CardPicker.cardDescription = this.problemCards[index].description;
        CardPicker.cardResult = this.problemCards[index].result;
        Debug.Log("--PROBLEM CARD CHOOSED");

        StartCoroutine(PopUpAnimateIn(this.cardPick));

        while(this.popUpAnimator.GetBool("PICKED") == false){
            yield return null;
        }
        Debug.Log("--PROBLEM CARD PICKED");
        yield return new WaitForSeconds(3);
        this.popUpAnimator.ResetTrigger("PICKED");

        StartCoroutine(PopUpAnimateOut(this.cardPick));

        Debug.Log("--END PICKPROBLEMCARD");
        StateManager.turnState = StateManager.TurnState.RESULT;
    }

    public void PickReviewCard(){
        CardPicker.typeOfCard = "REVIEW";
        int index = Random.Range(0, this.reviewCards.Count);
        CardPicker.cardDescription = this.reviewCards[index].description;
        CardPicker.cardResult = this.reviewCards[index].result;
        this.cardPick.SetActive(true);
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
        StateManager.ClearTurnState();
        this.sidePopUp.SetActive(false);
    }

    void InitState(){
        Debug.Log("-STATE_MANAGER INITIALIAZING");
        // StateManager.difficulty = StateManager.difficulty.EASY; TODO
        StateManager.difficulty = "EASY";
        StateManager.category = "GIFT SHOP";
        StateManager.gameName = "";
        StateManager.pokerPlanning = false;
        StateManager.CreatePlayers(new List<string>{"Alice", "Bob", "Charles"});
        StateManager.CreateUserStories("GIFT SHOP");

        StateManager.gameState = StateManager.GameState.INITIALISATION;
        Debug.Log("-STATE_MANAGER INITIALIZED");
    }

    public void OnSideClick(){
        this.sidePopUp.SetActive(true);
    }
    #endregion

    #region --------------------------------- Animations ---------------------------------
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
        this.popUpGO.SetActive(false);
        yield break;
    }

    IEnumerator AddToDebt(int value){
        Debug.Log("--START DEBT ANIMATION");
        Vector3 initPos = this.debtSlider.transform.position;
        Vector3 initScale = this.debtSlider.transform.localScale;
        this.debtSlider.transform.localPosition = new Vector3(960, 540, 0);
        this.debtSlider.transform.localScale = new Vector3(3.5f, 3.5f, 1f);
        Debug.Log("--DEBT SLIDER MOVED AND SCALED");
        yield return new WaitForSeconds(1);
        this.debtSlider.value = this.debtSlider.value + value;
        Debug.Log("--DEBT SLIDER'S VALUE CHANGED");
        yield return new WaitForSeconds(1);
        this.debtSlider.transform.position = initPos;
        this.debtSlider.transform.localScale = initScale;
        Debug.Log("--DEBT SLIDER REMOVED AND RESCALED");
        Debug.Log("--END DEBT ANIMATION");
        yield break;
    }
    #endregion
}
