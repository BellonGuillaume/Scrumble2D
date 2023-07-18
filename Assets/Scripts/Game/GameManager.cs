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
    [SerializeField] CardHandler cardHandler;
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

    [SerializeField] Button taskValidation;

    Animator popUpAnimator;


    Player currentPlayer;
    CardPicker cardPicker;
    private Coroutine dayAnimationRoutine;
    public static List<UserStory> workingOn;
    public List<GameObject> doingAUS;

    private StringTable table;
    private System.Random random;


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
        random = new System.Random();
        table = LocalizationSettings.StringDatabase.GetTable("Game");
        this.popUpAnimator = popUpGO.GetComponent<Animator>();
        this.cardPicker = cardPick.GetComponent<CardPicker>();
        this.cardHandler.CreateDailyCards();
        this.cardHandler.CreateProblemCards();
        this.cardHandler.CreateReviewCards();
        workingOn = new List<UserStory>();
        doingAUS = new List<GameObject>();
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
        // animationManager.ShowInfo(table.GetEntry("Phase de Review").GetLocalizedString());
        animationManager.ShowInfo("Phase de Review");
        yield return new WaitUntil(() => EventManager.animate == false);
        // BeginReview();
        // animationManager.ShowInfo(table.GetEntry("Phase de Rétrospective").GetLocalizedString());
        animationManager.ShowInfo("Phase de Rétrospective");
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
                    this.doingAUS.Add(go);
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
            StartCoroutine(this.cardHandler.FirstPickDailyCard());
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
        if (StateManager.players[player.playerNumber-1].turnToPass > 0){
            animationManager.ShowInfo($"{player.userName} passe son tour");
            StateManager.players[player.playerNumber-1].turnToPass--;
            yield return new WaitUntil(() => EventManager.animate == false);
            yield break;
        }
        Debug.Log($"Begin turn of {player.userName}");
        StateManager.currentPlayer = player;
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
        if (StateManager.firstDiceResult == 6 || StateManager.secondDiceResult == 6 || true) {
            // animationManager.ProblemAnimation();
            // yield return new WaitUntil(() => EventManager.animate == false);
            animationManager.ShowInfo(table.GetEntry("ProblemCard").GetLocalizedString());
            yield return new WaitUntil(() => EventManager.animate == false);

            animationManager.HideResults();
            yield return new WaitUntil(() => EventManager.animate == false);
            StateManager.turnState = StateManager.TurnState.PROBLEM;

            StartCoroutine(cardHandler.FirstPickProblemCard());
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
        int taskCounter = 0;
        if (StateManager.firstTaskOrDebtChoice == "DEBT"){
            debtCounter += StateManager.firstDiceResult;
        }
        if (StateManager.firstTaskOrDebtChoice == "TASK"){
            taskCounter += StateManager.firstDiceResult;
        }
        if(StateManager.alreadyReRoll && StateManager.secondTaskOrDebtChoice == "DEBT"){
            debtCounter += StateManager.secondDiceResult;
        }
        if(StateManager.alreadyReRoll && StateManager.secondTaskOrDebtChoice == "TASK"){
            taskCounter += StateManager.secondDiceResult;
        }

        Debug.Log("-END RESULTS");

        animationManager.HideResults();
        yield return new WaitUntil(() => EventManager.animate == false);

        if (debtCounter > 0){
            // StartCoroutine(AddToDebt(0-debtCounter));
            animationManager.UpdateDebtScrollBar(this.debtSlider.value - debtCounter);
            yield return new WaitUntil(() => EventManager.animate == false);
        }

        if (taskCounter > 0){
            EventManager.handleAddingTask = true;
            StartCoroutine(AddTasks(taskCounter));
            yield return new WaitUntil(() => EventManager.handleAddingTask == false);
        }
        yield return new WaitForSeconds(2);
        StateManager.turnState = StateManager.TurnState.END_OF_TURN;
    }
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

    public IEnumerator AddTasks(int n){
        this.taskValidation.gameObject.SetActive(true);
        EventManager.taskToAdd = n;
        EventManager.taskAdded = false;
        int reasonsToContinue = 0;
        while(EventManager.taskAdded == false){
            if(n == 0){
                EventManager.taskAdded = true;
                break;
            }
            reasonsToContinue = 0;
            foreach (GameObject doingAUS in this.doingAUS){
                if(EventManager.taskToAdd == n && n > 0){
                    doingAUS.GetComponent<ArrowedUS>().ShowUpArrow();
                }
                else if(EventManager.taskToAdd == n && n < 0){
                    doingAUS.GetComponent<ArrowedUS>().ShowDownArrow();
                }
                else if(EventManager.taskToAdd == 0 && n > 0){
                    if(doingAUS.GetComponent<ArrowedUS>().delta == 0){
                        doingAUS.GetComponent<ArrowedUS>().HideArrows();
                    } else {
                        doingAUS.GetComponent<ArrowedUS>().ShowDownArrow();
                    }
                }
                else if(EventManager.taskToAdd == 0 && n < 0){
                    if(doingAUS.GetComponent<ArrowedUS>().delta == 0){
                        doingAUS.GetComponent<ArrowedUS>().HideArrows();
                    } else {
                        doingAUS.GetComponent<ArrowedUS>().ShowUpArrow();
                    }
                }
                else {
                    if(n < 0 && doingAUS.GetComponent<ArrowedUS>().delta == 0){
                        doingAUS.GetComponent<ArrowedUS>().ShowDownArrow();
                    } 
                    else if(n > 0 && doingAUS.GetComponent<ArrowedUS>().delta == 0){
                        doingAUS.GetComponent<ArrowedUS>().ShowUpArrow();
                    }
                    else {
                        doingAUS.GetComponent<ArrowedUS>().ShowArrows();
                    }
                }
                reasonsToContinue++;
                if (doingAUS.GetComponent<ArrowedUS>().userStory.currentTask == doingAUS.GetComponent<ArrowedUS>().userStory.maxTask){
                    doingAUS.GetComponent<ArrowedUS>().HideUpArrow();
                    if(n > 0)
                        reasonsToContinue--;
                }
                else if (doingAUS.GetComponent<ArrowedUS>().userStory.currentTask == 0){
                    doingAUS.GetComponent<ArrowedUS>().HideDownArrow();
                    if(n < 0)
                        reasonsToContinue--;
                }
                
            }
            Debug.Log($"Remaining task to add and reasons to continue : {EventManager.taskToAdd.ToString()}, {reasonsToContinue.ToString()}");
            if (EventManager.taskToAdd == 0 || reasonsToContinue <= 0){
                    this.taskValidation.interactable = true;
                } else {
                    this.taskValidation.interactable = false;
                }
            yield return null;
        }
        yield return new WaitUntil(() => EventManager.taskAdded == true);
        this.taskValidation.gameObject.SetActive(false);
        foreach (GameObject doingAUS in this.doingAUS){
            doingAUS.GetComponent<ArrowedUS>().HideArrows();
            doingAUS.GetComponent<ArrowedUS>().delta = 0;
        }
        EventManager.handleAddingTask = false;
    }

    public void OnTaskValidationClick(){
        EventManager.taskAdded = true;
    }

    public void OnSideClick(){
        this.sidePopUp.SetActive(true);
    }
    #endregion
}
