using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class GameManager : MonoBehaviour
{
    [SerializeField] AnimationManager animationManager;
    [SerializeField] GameObject popUpGO;
    [SerializeField] GameObject sidePopUp;
    [SerializeField] GameObject cardPick;
    [SerializeField] GameObject turn;
    [SerializeField] GameObject roll;
    [SerializeField] GameObject results;
    [SerializeField] GameObject tddd;
    [SerializeField] Transform placeHolders;
    [SerializeField] GameObject userStoryUIPrefab;
    [SerializeField] GameObject littleArrowUSPrefab;
    [SerializeField] Image infoTxt;

    [SerializeField] Slider debtSlider;
    [SerializeField] Slider daySlider;

    Animator popUpAnimator;
    List<Card> dailyCards;
    List<Card> problemCards;
    List<Card> reviewCards;

    List<Card> remainingDailyCards;
    List<Card> remainingProblemCards;
    List<Card> remainingReviewCards;

    List<Card> discardedDailyCards;
    List<Card> discardedProblemCards;
    List<Card> discardedReviewCards;

    Player currentPlayer;
    CardPicker cardPicker;
    private Coroutine dayAnimationRoutine;
    public static List<UserStory> workingOn;

    private StringTable table;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Begin Assets initialization");
        // StateManager.gameState = StateManager.GameState.INITIALISATION;
        if (StateManager.gameState != StateManager.GameState.INITIALISATION) {
            Debug.Log("NEED TO INITSTATE");
            StateManager.gameState = StateManager.GameState.INITIALISATION;
            InitState();
        }
        table = LocalizationSettings.StringDatabase.GetTable("Game");
        this.popUpAnimator = popUpGO.GetComponent<Animator>();
        this.cardPicker = cardPick.GetComponent<CardPicker>();
        CreateDailyCards();
        CreateProblemCards();
        CreateReviewCards();
        workingOn = new List<UserStory>();
        StateManager.gameState = StateManager.GameState.BEGIN_GAME;
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame(){
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.BEGIN_GAME);
        Debug.Log("Begin Game");
        animationManager.StartGame();
        yield return new WaitUntil(() => EventManager.animate == false);
        yield return new WaitForSeconds(2);
        int i = 1;
        while (true){
            StartCoroutine(BeginSprint(i));
            yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.END_OF_SPRINT);
            i++;
        }
    }

    #region --------------------------------- Sprint ---------------------------------
    IEnumerator BeginSprint(int n){
        Debug.Log($"BEGIN SPRINT {n.ToString()}");
        StateManager.gameState = StateManager.GameState.TDTD;
        StateManager.currentDay = 0;
        StartCoroutine(ChooseToDoToDoing());
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.BEGIN_DAY);
        for (int j = 1; j <= 9; j++){
            StateManager.gameState = StateManager.GameState.BEGIN_DAY;
            StateManager.currentDay = j;
            StartCoroutine(BeginDay(j));
            yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.END_OF_DAY);
            Debug.Log($"End of day {j} reached");
        }
        animationManager.ShowInfo(table.GetEntry("Phase de Review").GetLocalizedString());
        yield return new WaitUntil(() => EventManager.animate == false);
        // BeginReview();
        animationManager.ShowInfo(table.GetEntry("Phase de Rétrospective").GetLocalizedString());
        yield return new WaitUntil(() => EventManager.animate == false);
        // BeginRetrospective();
        StateManager.gameState = StateManager.GameState.END_OF_SPRINT;
    }

    public void BeginReview(){
        // ShowResults();
        // PickReviewCard();
        // UpdateDetteEndTurn();
        // IncreaseScore();
    }

    IEnumerator ChooseToDoToDoing(){
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.TDTD);
        animationManager.ShowTDDD();
        yield return new WaitUntil(() => this.popUpAnimator.GetBool("TDTD") == true);
        this.popUpAnimator.ResetTrigger("TDTD");

        AddDoingToWorking();
        animationManager.HideTDDD();
        yield return new WaitUntil(() => EventManager.animate == false);
        StateManager.gameState = StateManager.GameState.BEGIN_DAY;
    }

    public void AddDoingToWorking(){
        foreach (UserStory userStory in workingOn){
            foreach (Transform child in this.placeHolders){
                if (child.childCount == 0){
                    GameObject go = Instantiate(littleArrowUSPrefab);
                    go.GetComponent<UserStoryUI>().Fill(userStory);
                    go.GetComponent<ArrowedUS>().HideArrows();
                    go.transform.SetParent(child.transform);
                    go.transform.localPosition = Vector3.zero;
                    break;
                }
            }
        }
        workingOn.Clear();
    }
    #endregion

    #region --------------------------------- Day ---------------------------------
    IEnumerator BeginDay(int n){
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.BEGIN_DAY);
        Debug.Log($"BEGIN Day {n}");
        animationManager.StartDayAnimation(n);
        yield return new WaitUntil(() => EventManager.animate == false);
        if (n != 1){
            StateManager.gameState = StateManager.GameState.PICK_DAILY;
            StartCoroutine(FirstPickDailyCard());
            yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.PLAYER_TURN);
            animationManager.ZoomOutPopUp(this.cardPick);
            this.popUpGO.SetActive(false);
            yield return new WaitUntil(() => EventManager.animate == false);
        } else {
            StateManager.gameState = StateManager.GameState.PLAYER_TURN;
        }
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.PLAYER_TURN);
        foreach (Player player in StateManager.players){
            StateManager.turnState = StateManager.TurnState.BEGIN_TURN;
            StartCoroutine(BeginTurn(player));
            yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.END_OF_TURN);
            ClearTurn();
        }
        StateManager.gameState = StateManager.GameState.END_OF_DAY;
    }
    #endregion

    #region --------------------------------- Turn ---------------------------------
    // void BeginTurn(Player player){
    IEnumerator BeginTurn(Player player){
        yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.BEGIN_TURN);
        Debug.Log($"Begin turn of {player.userName}");
        this.currentPlayer = player;
        animationManager.StartTurnAnimation(player);
        yield return new WaitUntil(() => EventManager.animate == false);
        StateManager.turnState = StateManager.TurnState.CHOICE;
        StartCoroutine(StartChoiceTaskDebt());
        yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.ROLL);
        StartCoroutine(StartRollDice());
        yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.RESULT);
        StartCoroutine(StartShowResult());
    }

    IEnumerator StartChoiceTaskDebt(){
        yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.CHOICE);
        Debug.Log("STARTING CHOICE");
        
        animationManager.ShowChoice();
        yield return new WaitUntil(() => EventManager.animate == false);

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
        
        animationManager.ZoomOutPopUp(this.turn);
        yield return new WaitUntil(() => EventManager.animate == false);

        Debug.Log("-END OF CHOICE");
        StateManager.turnState = StateManager.TurnState.ROLL;
    }
    IEnumerator StartRollDice(){
        yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.ROLL);
        Debug.Log("STARTING ROLL");

        animationManager.ZoomInPopUp(this.roll);
        yield return new WaitUntil(() => EventManager.animate == false);

        while(this.popUpAnimator.GetBool("ROLL") == false){
            yield return null;
        }

        int result = this.roll.GetComponent<UIDice>().RollDice();
        

        if (!StateManager.alreadyReRoll){
            StateManager.firstDiceResult = result;
        }
        else {
            StateManager.secondDiceResult = result;
        }

        this.popUpAnimator.ResetTrigger("ROLL");

        yield return new WaitForSeconds(0.5f);
        
        animationManager.ZoomOutPopUp(this.roll);
        yield return new WaitUntil(() => EventManager.animate == false);
        Debug.Log("-END ROLL");
        StateManager.turnState = StateManager.TurnState.RESULT;
    }
    IEnumerator StartShowResult(){
        yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.RESULT);
        Debug.Log("-START RESULTS");
        if (StateManager.firstDiceResult == 1 && !StateManager.alreadyReRoll){
            Debug.Log("--RESULT => REROLL");
            // StartCoroutine(ReRollAnimation());
            animationManager.ShowInfo(table.GetEntry("Reroll").GetLocalizedString());
            yield return new WaitUntil(() => EventManager.animate == false);
            StateManager.alreadyReRoll = true;
            StateManager.turnState = StateManager.TurnState.CHOICE;
            StartCoroutine(StartChoiceTaskDebt());
            yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.ROLL);
            StartCoroutine(StartRollDice());
            yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.RESULT);
            Debug.Log("--END REROLL");
        }

        Debug.Log("-SHOWING RESULTS");
        animationManager.ZoomInPopUp(this.results);
        yield return new WaitUntil(() => EventManager.animate == false);
        if (StateManager.firstDiceResult == 6 || StateManager.secondDiceResult == 6) {
            // animationManager.ProblemAnimation();
            // yield return new WaitUntil(() => EventManager.animate == false);
            animationManager.ShowInfo(table.GetEntry("ProblemCard").GetLocalizedString());
            yield return new WaitUntil(() => EventManager.animate == false);

            animationManager.HideResults();
            yield return new WaitUntil(() => EventManager.animate == false);
            StateManager.turnState = StateManager.TurnState.PROBLEM;

            StartCoroutine(FirstPickProblemCard());
            yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.RESULT);

            animationManager.ShowResults();
            yield return new WaitUntil(() => EventManager.animate == false);
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

        animationManager.HideResults();
        yield return new WaitUntil(() => EventManager.animate == false);

        if (debtCounter > 0){
            // StartCoroutine(AddToDebt(0-debtCounter));
            animationManager.UpdateDebtScrollBar(this.debtSlider.value - debtCounter);
            yield return new WaitUntil(() => EventManager.animate == false);
        }
        yield return new WaitForSeconds(2);
        StateManager.turnState = StateManager.TurnState.END_OF_TURN;
    }
    #endregion

    #region --------------------------------- Initialisation ---------------------------------
    void CreateDailyCards(){
        string path = "";
        if (StateManager.language == LocalizationSettings.AvailableLocales.GetLocale("en")){
            path = Application.streamingAssetsPath + "/Cards/DailyCards_EN.json";
        } else {
            path = Application.streamingAssetsPath + "/Cards/DailyCards_FR.json";
        }
        string dailyCardsStr = File.ReadAllText(path);
        this.dailyCards = JsonConvert.DeserializeObject<List<Card>>(dailyCardsStr);
        this.discardedDailyCards = new List<Card>();
        foreach (Card card in this.dailyCards){
            Debug.Log(card.ToString());
        }
    }

    void CreateProblemCards(){
        string path = "";
        if (StateManager.language == LocalizationSettings.AvailableLocales.GetLocale("en")){
            path = Application.streamingAssetsPath + "/Cards/ProblemCards_EN.json";
        } else {
            path = Application.streamingAssetsPath + "/Cards/ProblemCards_FR.json";
        }
        string problemCardsStr = File.ReadAllText(path);
        this.problemCards = JsonConvert.DeserializeObject<List<Card>>(problemCardsStr);
        this.discardedProblemCards = new List<Card>();
        foreach (Card card in this.problemCards){
            Debug.Log(card.ToString());
        }
    }

    void CreateReviewCards(){
        string path = "";
        if (StateManager.language == LocalizationSettings.AvailableLocales.GetLocale("en")){
            path = Application.streamingAssetsPath + "/Cards/ReviewCards_EN.json";
        } else {
            path = Application.streamingAssetsPath + "/Cards/ReviewCards_FR.json";
        }
        string reviewCardsStr = File.ReadAllText(path);
        this.reviewCards = JsonConvert.DeserializeObject<List<Card>>(reviewCardsStr);
        this.discardedReviewCards = new List<Card>();
        foreach (Card card in this.reviewCards){
            Debug.Log(card.ToString());
        }
    }
    #endregion

    #region --------------------------------- Cards ---------------------------------
    IEnumerator FirstPickDailyCard(){
        EventManager.cardsToPick = 1;
        for (int i = 0; i < 3; i++){
            if (this.dailyCards.Count < 1){
                this.dailyCards.AddRange(this.discardedDailyCards);
                this.discardedDailyCards = new List<Card>();
            }
            int index = Random.Range(0, this.dailyCards.Count);
            Card choosenCard = this.dailyCards[i];
            this.cardPicker.AddCart(choosenCard);
            this.discardedDailyCards.Add(choosenCard);
            this.dailyCards.Remove(choosenCard);
        }
        StartCoroutine(HandleCards());
        yield return new WaitUntil(() => EventManager.handleCards == false);
        StateManager.gameState = StateManager.GameState.PLAYER_TURN;
    }

    IEnumerator FirstPickProblemCard(){
        yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.PROBLEM);
        EventManager.cardsToPick = 1;
        for (int i = 0; i < 3; i++){
            if (this.problemCards.Count <= 0){
                this.problemCards.AddRange(this.discardedProblemCards);
                this.discardedProblemCards = new List<Card>();
            }
            int index = Random.Range(0, this.problemCards.Count);
            Card choosenCard = this.problemCards[i];
            this.cardPicker.AddCart(choosenCard);
            this.discardedProblemCards.Add(choosenCard);
            this.problemCards.Remove(choosenCard);
        }
        StartCoroutine(HandleCards());
        yield return new WaitUntil(() => EventManager.handleCards == false);
        StateManager.turnState = StateManager.TurnState.RESULT;
    }

    IEnumerator FirstPickReviewCard(){
        EventManager.cardsToPick = 1;
        for (int i = 0; i < 3; i++){
            if (this.reviewCards.Count < 1){
                this.reviewCards.AddRange(this.discardedReviewCards);
                this.discardedReviewCards = new List<Card>();
            }
            int index = Random.Range(0, this.reviewCards.Count);
            Card choosenCard = this.reviewCards[i];
            this.cardPicker.AddCart(choosenCard);
            this.discardedReviewCards.Add(choosenCard);
            this.reviewCards.Remove(choosenCard);
        }
        StartCoroutine(HandleCards());
        yield return new WaitUntil(() => EventManager.handleCards == false);
        StateManager.gameState = StateManager.GameState.RETROSPECTIVE;
    }

    IEnumerator HandleCards(){
        EventManager.handleCards = true;
        animationManager.ShowCardPick();
        yield return new WaitUntil(() => EventManager.animate == false);
        while (EventManager.cardsToPick > 0 || this.cardPicker.choosenCard != null){
            yield return new WaitUntil(() => this.cardPicker.choosenCard != null);
            yield return new WaitForSeconds(2);
            Debug.Log($"Handle Card : {this.cardPicker.choosenCard.GetComponent<UICard>().card.ToString()}");
            StartCoroutine(HandleSingleCard(this.cardPicker.choosenCard.GetComponent<UICard>().card));
            yield return new WaitUntil(() => EventManager.handleSingleCard == false);
            this.cardPicker.choosenCard = null;
        }
        animationManager.HideCardPick();
        this.cardPicker.Reset();
        yield return new WaitUntil(() => EventManager.animate == false);
        EventManager.handleCards = false;
    }
    
    IEnumerator HandleSingleCard(Card card){
        EventManager.handleSingleCard = true;

        switch (card.typeOfCard){
            case Card.TypeOfCard.Simple :
                StartCoroutine(HandleSimpleAction(card.firstAction, card.firstValue));
                yield return new WaitUntil(() => EventManager.handleSimpleAction == false);
                break;
            default :
                break;
        }

        EventManager.handleSingleCard = false;
        yield break;
    }

    #region - - - - - - - - - - - - - - - - - Handle CardType - - - - - - - - - - - - - - - - -
    IEnumerator HandleSimpleAction(Card.Action action, float value){
        EventManager.handleSimpleAction = true;

        switch (action){
            case Card.Action.IncreaseTask :
            
            break;
            case Card.Action.DecreaseTask :
            
            break;
            case Card.Action.IncreaseDebt :
            
            break;
            case Card.Action.DecreaseDebt :
            
            break;
            case Card.Action.IncreaseTaskPerPlayer :
            
            break;
            case Card.Action.IncreaseDebtPerPlayer :
            
            break;
            case Card.Action.DecreaseTaskPerPlayer :
            
            break;
            case Card.Action.DecreaseDebtPerPlayer :
            
            break;
            case Card.Action.IncreaseTaskPerDevelopper :
            
            break;
            case Card.Action.IncreaseDebtPerDevelopper :
            
            break;
            case Card.Action.DecreaseTaskPerDevelopper :
            
            break;
            case Card.Action.DecreaseDebtPerDevelopper :
            
            break;
            case Card.Action.MultiplieDebt :
            
            break;
            case Card.Action.RaiseTaskPerCurrentDebt :
            
            break;
            case Card.Action.CurrentPlayerPassATurn :
            
            break;
            case Card.Action.NextPlayerPassATurn :
            
            break;
            case Card.Action.AllPlayersPassATurn :
            
            break;
            case Card.Action.PickDailycards :
            
            break;
            case Card.Action.PickProblemCards :
            
            break;
            case Card.Action.PickReviewCards :
            
            break;
            default :
                break;

        }

        EventManager.handleSimpleAction = false;
        yield break;
    }
    #endregion
    #region - - - - - - - - - - - - - - - - - Card Actions - - - - - - - - - - - - - - - - -
    IEnumerator IncreaseTask(int n){
        yield break;
    }
    IEnumerator DecreaseTask(int n){
        yield break;
    }
    IEnumerator IncreaseDebt(int n){
        animationManager.UpdateDebtScrollBar(this.debtSlider.value + n);
        yield return new WaitUntil(() => EventManager.animate == false);
    }
    IEnumerator DecreaseDebt(int n){
        animationManager.UpdateDebtScrollBar(this.debtSlider.value - n);
        yield return new WaitUntil(() => EventManager.animate == false);
    }
    IEnumerator IncreaseTaskPerPlayer(int n){
        yield break;
    }
    IEnumerator IncreaseDebtPerPlayer(int n){
        animationManager.UpdateDebtScrollBar(this.debtSlider.value + (n * StateManager.players.Count + 2));
        yield return new WaitUntil(() => EventManager.animate == false);
    }
    IEnumerator DecreaseTaskPerPlayer(int n){
        yield break;
    }
    IEnumerator DecreaseDebtPerPlayer(int n){
        animationManager.UpdateDebtScrollBar(this.debtSlider.value - (n * StateManager.players.Count + 2));
        yield return new WaitUntil(() => EventManager.animate == false);
    }
    IEnumerator IncreaseTaskPerDevelopper(int n){
        yield break;
    }
    IEnumerator IncreaseDebtPerDevelopper(int n){
        animationManager.UpdateDebtScrollBar(this.debtSlider.value + (n * StateManager.players.Count));
        yield return new WaitUntil(() => EventManager.animate == false);
    }
    IEnumerator DecreaseTaskPerDevelopper(int n){
        yield break;
    }
    IEnumerator DecreaseDebtPerDevelopper(int n){
        animationManager.UpdateDebtScrollBar(this.debtSlider.value - (n * StateManager.players.Count));
        yield return new WaitUntil(() => EventManager.animate == false);
    }
    IEnumerator MultiplieDebt(float n){
        animationManager.UpdateDebtScrollBar(this.debtSlider.value * n);
        yield return new WaitUntil(() => EventManager.animate == false);
    }
    IEnumerator RaiseTaskPerCurrentDebt(){
        yield break;
    }
    IEnumerator CurrentPlayerPassATurn(int n){
        StateManager.players[currentPlayer.playerNumber-1].turnToPass = n;
        yield break;
    }
    IEnumerator NextPlayerPassATurn(int n){
        StateManager.players[currentPlayer.nextPlayerNumber-1].turnToPass = n;
        yield break;
    }
    IEnumerator AllPlayersPassATurn(int n){
        foreach (Player player in StateManager.players){
            player.turnToPass = n;
        }
        yield break;
    }
    IEnumerator PickProblemCards(int n){
        EventManager.cardsToPick += n;
        // animate updeck
        yield return new WaitUntil(() => EventManager.animate == false);
        for (int i = 0; i < n; i++){
            if (this.problemCards.Count < 1){
                this.problemCards.AddRange(this.discardedProblemCards);
                this.discardedProblemCards = new List<Card>();
            }
            int index = Random.Range(0, this.problemCards.Count);
            Card choosenCard = this.problemCards[i];
            this.cardPicker.AddCart(choosenCard);
            yield return new WaitUntil(() => EventManager.animate == false);
            this.discardedProblemCards.Add(choosenCard);
            this.problemCards.Remove(choosenCard);
        }
        // animate downdeck
        yield return new WaitUntil(() => EventManager.animate == false);
    }
    IEnumerator PickDailycards(int n){
        EventManager.cardsToPick += n;
        // animate updeck
        yield return new WaitUntil(() => EventManager.animate == false);
        for (int i = 0; i < n; i++){
            if (this.dailyCards.Count < 1){
                this.dailyCards.AddRange(this.discardedDailyCards);
                this.discardedDailyCards = new List<Card>();
            }
            int index = Random.Range(0, this.dailyCards.Count);
            Card choosenCard = this.dailyCards[i];
            this.cardPicker.AddCart(choosenCard);
            yield return new WaitUntil(() => EventManager.animate == false);
            this.discardedDailyCards.Add(choosenCard);
            this.dailyCards.Remove(choosenCard);
        }
        // animate downdeck
        yield return new WaitUntil(() => EventManager.animate == false);
    }
    IEnumerator PickReviewCards(int n){
        EventManager.cardsToPick += n;
        // animate updeck
        yield return new WaitUntil(() => EventManager.animate == false);
        for (int i = 0; i < n; i++){
            if (this.reviewCards.Count < 1){
                this.reviewCards.AddRange(this.discardedReviewCards);
                this.discardedReviewCards = new List<Card>();
            }
            int index = Random.Range(0, this.reviewCards.Count);
            Card choosenCard = this.reviewCards[i];
            this.cardPicker.AddCart(choosenCard);
            yield return new WaitUntil(() => EventManager.animate == false);
            this.discardedReviewCards.Add(choosenCard);
            this.reviewCards.Remove(choosenCard);
        }
        // animate downdeck
        yield return new WaitUntil(() => EventManager.animate == false);
    }
    #endregion
    #endregion
    
    #region --------------------------------- Utils ---------------------------------
        void ClearTurn(){
        Debug.Log("Begin the clear");
        StateManager.ClearTurnState();
        this.results.GetComponent<Results>().ChangeText("");
    }

    void InitState(){
        Debug.Log("-STATE_MANAGER INITIALIAZING");
        // StateManager.difficulty = StateManager.difficulty.EASY; TODO
        StateManager.difficulty = StateManager.Difficulty.EASY;
        StateManager.category = StateManager.Category.GIFT_SHOP;
        StateManager.gameName = "";
        StateManager.pokerPlanning = false;
        StateManager.CreatePlayers(new List<string>{"Alice", "Bob", "Charles"});
        StateManager.CreateUserStories(StateManager.Category.GIFT_SHOP);

        StateManager.gameState = StateManager.GameState.INITIALISATION;
        Debug.Log("-STATE_MANAGER INITIALIZED");
    }

    public void OnSideClick(){
        this.sidePopUp.SetActive(true);
    }
    #endregion
}
