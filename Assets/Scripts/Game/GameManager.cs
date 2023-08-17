using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.Localization.Settings;

public class GameManager : MonoBehaviour
{
    [SerializeField] CardHandler cardHandler;
    [SerializeField] AnimationManager animationManager;
    [SerializeField] GameObject popUpGO;
    [SerializeField] GameObject sidePopUp;
    [SerializeField] ChoosePlayer choosePlayer;
    [SerializeField] GameObject cardPick;
    [SerializeField] GameObject turn;
    [SerializeField] GameObject roll;
    [SerializeField] GameObject results;
    [SerializeField] FilledChoiceHandler filledChoice;
    [SerializeField] GameObject tddd;
    [SerializeField] Transform placeHolders;
    [SerializeField] GameObject littleArrowUSPrefab;
    [SerializeField] Image infoTxt;

    [SerializeField] Slider debtSlider;
    [SerializeField] Slider daySlider;

    [SerializeField] Button taskValidation;

    [SerializeField] ReviewManager reviewManager;
    [SerializeField] SummaryManager summaryManager;
    [SerializeField] EndScreenManager endScreenManager;
    [SerializeField] BurndownChartManager burndownChartManager;

    [SerializeField] ScrumboardManager sideScrumManager;
    [SerializeField] ScrumboardManager summaryScrumManager;
    [SerializeField] RetrospectiveManager retrospectiveManager;

    [SerializeField] TMP_Text turnOfTxt;

    Animator popUpAnimator;


    Player currentPlayer;
    CardPicker cardPicker;
    private Coroutine dayAnimationRoutine;
    public static List<UserStory> workingOn;
    public List<GameObject> doingAUS;

    private System.Random random;

    void Start()
    {
        if (StateManager.gameState != StateManager.GameState.INITIALISATION) {
            StateManager.gameState = StateManager.GameState.INITIALISATION;
            InitState();
        }
        random = new System.Random();
        this.popUpAnimator = popUpGO.GetComponent<Animator>();
        this.cardPicker = cardPick.GetComponent<CardPicker>();
        this.cardHandler.CreateDailyCards();
        this.cardHandler.CreateProblemCards();
        this.cardHandler.CreateReviewCards();
        this.choosePlayer.CreateDailyPlayers();
        this.cardHandler.InitPermanentCard();
        this.sideScrumManager.CreateScrumboard();
        this.summaryScrumManager.CreateScrumboard();
        workingOn = new List<UserStory>();
        doingAUS = new List<GameObject>();
        switch (StateManager.difficulty){
            case StateManager.Difficulty.EASY :
                StateManager.debtFactor = 3;
                StateManager.currentDebt = 5;
                debtSlider.value = 5;
                break;
            case StateManager.Difficulty.NORMAL :
                StateManager.debtFactor = 4;
                StateManager.currentDebt = 15;
                debtSlider.value = 15;
                break;
            case StateManager.Difficulty.HARD :
                StateManager.debtFactor = 6;
                StateManager.currentDebt = 25;
                debtSlider.value = 25;
                break;
        }
        StateManager.gameState = StateManager.GameState.BEGIN_GAME;
        StateManager.startTime = DateTime.Now;
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame(){
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.BEGIN_GAME);
        animationManager.StartGame();
        yield return new WaitUntil(() => EventManager.animate == false);
        yield return new WaitForSeconds(2);
        int i = 1;
        while (StateManager.gameState != StateManager.GameState.END_OF_GAME){
            StartCoroutine(BeginSprint(i));
            yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.END_OF_SPRINT);
            Debug.Log(burndownChartManager.currentSprint.ToString());
            foreach(Day day in burndownChartManager.currentSprint.days){
                Debug.Log(day.ToString());
            }
            bool finished = true;
            foreach (UserStory userStory in StateManager.userStories){
                if (userStory.state != UserStory.State.DEPLOYED)
                    finished = false;
            }
            if(finished)
                StateManager.gameState = StateManager.GameState.END_OF_GAME;
            i++;
        }
        StartCoroutine(endScreenManager.HandleEndGame());
    }

    #region --------------------------------- Sprint ---------------------------------
    IEnumerator BeginSprint(int n){
        StateManager.sprintNumber++;
        InitSprintState();
        StateManager.gameState = StateManager.GameState.TDTD;
        StateManager.currentDay = 0;
        if (n > 1){
            animationManager.StartDayAnimation(11);
            yield return new WaitUntil(() => EventManager.animate == false);
        }
        StartCoroutine(ChooseToDoToDoing());
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.BEGIN_DAY);
        List<UserStory> userStories = new List<UserStory>();
        foreach(GameObject arrowedUS in this.doingAUS){
            userStories.Add(arrowedUS.GetComponent<UserStoryUI>().userStory);
        }
        burndownChartManager.NewSprint(n, userStories);
        if(StateManager.tasksOnBeginSprint == true){
            EventManager.permanentCardShowned = false;
            this.cardHandler.ShowTasksOnBeginSprintPermanent();
            yield return new WaitUntil(() => EventManager.permanentCardShowned == true);
            yield return new WaitForSeconds(2f);
            EventManager.readyToHidePermanent = true;
            yield return new WaitUntil(() => EventManager.permanentCardHidden == true);
            EventManager.permanentCardHidden = false;
            EventManager.handleAddingTask = true;
            StartCoroutine(AddTasks(5));
            yield return new WaitUntil(() => EventManager.handleAddingTask == false);
        }
        for (int j = 1; j <= 9; j++){
            StateManager.gameState = StateManager.GameState.BEGIN_DAY;
            StateManager.currentDay = j;
            StartCoroutine(BeginDay(j));
            yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.END_OF_DAY || StateManager.gameState == StateManager.GameState.WANT_TO_PASS);
            if (StateManager.gameState == StateManager.GameState.WANT_TO_PASS)
                break;
        }
        StateManager.gameState = StateManager.GameState.REVIEW;
        animationManager.StartDayAnimation(9);
        yield return new WaitUntil(() => EventManager.animate == false);
        animationManager.ShowInfo(GetString("Game", "ReviewPhase"));
        yield return new WaitUntil(() => EventManager.animate == false);
        StartCoroutine(reviewManager.handleReview());
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.REVIEW_CARDS);
        StartCoroutine(cardHandler.FirstPickReviewCard(EventManager.cardsToPick));
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.SUMMARY);
        StartCoroutine(summaryManager.HandleSummary());
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.RETROSPECTIVE);
        animationManager.StartDayAnimation(10);
        yield return new WaitUntil(() => EventManager.animate == false);
        StartCoroutine(retrospectiveManager.HandleRetrospective());
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.END_OF_SPRINT);
    }

    public IEnumerator ChooseToDoToDoing(bool withPopUp = true){
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.TDTD);
        animationManager.ShowTDDD(withPopUp);
        yield return new WaitUntil(() => this.popUpAnimator.GetBool("TDTD") == true);
        this.popUpAnimator.ResetTrigger("TDTD");

        AddDoingToWorking();
        animationManager.HideTDDD(withPopUp);
        yield return new WaitUntil(() => EventManager.animate == false);
        StateManager.gameState = StateManager.GameState.BEGIN_DAY;
    }

    public void AddDoingToWorking(){
        foreach (UserStory userStory in workingOn){
            foreach (Transform child in this.placeHolders){
                if (child.childCount == 0){
                    GameObject go = Instantiate(littleArrowUSPrefab);
                    go.GetComponent<UserStoryUI>().Fill(userStory);
                    go.GetComponent<ArrowedUS>().SetUserStory(userStory);
                    go.GetComponent<ArrowedUS>().HideArrows();
                    go.transform.SetParent(child.transform);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;
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
        animationManager.StartDayAnimation(n-1);
        if(EventManager.updateBurndownChart == true)
            burndownChartManager.UpdateBurndownChart();
        burndownChartManager.currentSprint.NewDay(n);
        yield return new WaitUntil(() => EventManager.animate == false);
        if (StateManager.oneTaskPerDay == true){
            EventManager.permanentCardShowned = false;
            this.cardHandler.ShowOneTaskPerDayPermanent();
            yield return new WaitUntil(() => EventManager.permanentCardShowned == true);
            yield return new WaitForSeconds(2f);
            EventManager.readyToHidePermanent = true;
            yield return new WaitUntil(() => EventManager.permanentCardHidden == true);
            EventManager.permanentCardHidden = false;
            EventManager.handleAddingTask = true;
            StartCoroutine(AddTasks(1));
            yield return new WaitUntil(() => EventManager.handleAddingTask == false);
        }
        if (n != 1){
            StartCoroutine(choosePlayer.ShowDailyPlayers());
            yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.PICK_DAILY);
            StartCoroutine(this.cardHandler.FirstPickDailyCard());
            yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.PLAYER_TURN);
            animationManager.ZoomOutPopUp(this.cardPick);
            this.popUpGO.SetActive(false);
            yield return new WaitUntil(() => EventManager.animate == false);

            bool isFinished = true;
            foreach (GameObject aus in doingAUS){
                if (aus.GetComponent<UserStoryUI>().userStory.state != UserStory.State.DONE)
                    isFinished = false;
            }
            if (isFinished && EventManager.allFilledChoiceMade == false){
                StartCoroutine(filledChoice.HandleFilledChoice());
                yield return new WaitUntil(() => EventManager.allFilledChoiceMade == true);
                if (filledChoice.debtClicked){
                    yield return new WaitForSeconds(1);
                }
                else if (filledChoice.moreUSClicked){
                    EventManager.allFilledChoiceMade = false;
                    yield return new WaitForSeconds(1);
                    StateManager.turnState = StateManager.TurnState.NEW_US_ADDED;
                    StateManager.gameState = StateManager.GameState.END_OF_DAY;
                    yield break;
                }
                else if (filledChoice.endSprintClicked){
                    EventManager.allFilledChoiceMade = false;
                    yield return new WaitForSeconds(1);
                    StateManager.turnState = StateManager.TurnState.WANT_TO_PASS;
                    StateManager.gameState = StateManager.GameState.WANT_TO_PASS;
                    yield break;
                }
            } else {
                if (!isFinished){
                    EventManager.allFilledChoiceMade = false;
                    EventManager.onlyDebt = false;
                }
                yield return new WaitForSeconds(1);
                StateManager.turnState = StateManager.TurnState.END_OF_TURN;
            }
        } else {
            StateManager.gameState = StateManager.GameState.PLAYER_TURN;
        }
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.PLAYER_TURN);
        foreach (Player player in StateManager.players){
            StateManager.turnState = StateManager.TurnState.BEGIN_TURN;
            StartCoroutine(BeginTurn(player));
            yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.END_OF_TURN || StateManager.turnState == StateManager.TurnState.NEW_US_ADDED || StateManager.turnState == StateManager.TurnState.WANT_TO_PASS);
            Debug.Log($"Day turned ready to clear");
            ClearTurn();
            if (StateManager.turnState == StateManager.TurnState.NEW_US_ADDED)
                break;
            if (StateManager.turnState == StateManager.TurnState.WANT_TO_PASS){
                StateManager.gameState = StateManager.GameState.WANT_TO_PASS;
                yield break;
            }
        }
        StateManager.gameState = StateManager.GameState.END_OF_DAY;
    }
    #endregion

    #region --------------------------------- Turn ---------------------------------
    // void BeginTurn(Player player){
    IEnumerator BeginTurn(Player player){
        yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.BEGIN_TURN);
        turnOfTxt.text = GetString("Game", "TurnOf") + " " + player.userName;
        if (player.turnToPass > 0){
            animationManager.ShowInfo($"{player.userName}" + " " + GetString("Game", "PassHisTurn"));
            player.turnToPass--;
            if (player.turnToPass <= 0 && player.oneMoreTaskPerRoll)
                StateManager.oneMoreTaskPerRoll = true;
            yield return new WaitUntil(() => EventManager.animate == false);
            StateManager.turnState = StateManager.TurnState.END_OF_TURN;
            yield break;
        }
        StateManager.currentPlayer = player;
        animationManager.StartTurnAnimation(player);
        yield return new WaitUntil(() => EventManager.animate == false);
        StateManager.turnState = StateManager.TurnState.CHOICE;
        Debug.Log($"Attempting to choice");
        StartCoroutine(StartChoiceTaskDebt());
        yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.ROLL);
        Debug.Log($"Attempting to roll");
        StartCoroutine(StartRollDice());
        yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.RESULT);
        Debug.Log($"Attempting to handle result");
        StartCoroutine(StartShowResult());
    }
    IEnumerator StartChoiceTaskDebt(){
        yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.CHOICE);
        
        if (EventManager.onlyDebt == true)
            turn.transform.GetChild(1).gameObject.SetActive(false);
        else
            turn.transform.GetChild(1).gameObject.SetActive(true);

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
        StateManager.turnState = StateManager.TurnState.ROLL;
    }
    IEnumerator StartRollDice(){
        yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.ROLL);

        animationManager.ZoomInPopUp(this.roll);
        yield return new WaitUntil(() => EventManager.animate == false);

        EventManager.rolled = false;
        StartCoroutine(this.roll.GetComponent<UIDice>().RollDice());
        yield return new WaitUntil(() => EventManager.rolled == true);
        EventManager.rolled = false;

        int result = this.roll.GetComponent<UIDice>().currentFace;
        
        if (!StateManager.alreadyReRoll){
            StateManager.firstDiceResult = result;
        }
        else {
            StateManager.secondDiceResult = result;
        }
        
        bool jinx =  StateManager.jinxed && (StateManager.firstDiceResult == 5 || StateManager.secondDiceResult == 5);
        if (jinx){
            EventManager.permanentCardShowned = false;
            this.cardHandler.ShowJinxPermanent();
            yield return new WaitUntil(() => EventManager.permanentCardShowned == true);
            yield return new WaitForSeconds(2f);
            EventManager.readyToHidePermanent = true;
            yield return new WaitUntil(() => EventManager.permanentCardHidden == true);
            EventManager.permanentCardHidden = false;
        }

        
        animationManager.ZoomOutPopUp(this.roll);
        yield return new WaitUntil(() => EventManager.animate == false);
        StateManager.turnState = StateManager.TurnState.RESULT;
    }
    IEnumerator StartShowResult(){
        yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.RESULT);
        if (StateManager.firstDiceResult == 1 && !StateManager.alreadyReRoll){
            // StartCoroutine(ReRollAnimation());
            animationManager.ShowInfo(GetString("Game", "Reroll"));
            yield return new WaitUntil(() => EventManager.animate == false);
            StateManager.alreadyReRoll = true;
            StateManager.turnState = StateManager.TurnState.CHOICE;
            StartCoroutine(StartChoiceTaskDebt());
            yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.ROLL);
            StartCoroutine(StartRollDice());
            yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.RESULT);
        }

        animationManager.ZoomInPopUp(this.results);
        yield return new WaitUntil(() => EventManager.animate == false);
        bool jinx =  StateManager.jinxed && (StateManager.firstDiceResult == 5 || StateManager.secondDiceResult == 5);
        if (StateManager.firstDiceResult == 6 || StateManager.secondDiceResult == 6 || jinx) {
            // animationManager.ProblemAnimation();
            // yield return new WaitUntil(() => EventManager.animate == false);
            animationManager.ShowInfo(GetString("Game", "ProblemCard"));
            yield return new WaitUntil(() => EventManager.animate == false);

            animationManager.HideResults();
            yield return new WaitUntil(() => EventManager.animate == false);
            StateManager.turnState = StateManager.TurnState.PROBLEM;

            StartCoroutine(cardHandler.FirstPickProblemCard());
            yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.RESULT);

            animationManager.ShowResults();
            yield return new WaitUntil(() => EventManager.animate == false);
        }

        string prompt = "";
        if (StateManager.firstTaskOrDebtChoice == "TASK"){
            prompt += GetString("Game", "YouWon") + " " + StateManager.firstDiceResult.ToString() + " ";
            if (StateManager.firstDiceResult > 1)
                prompt += GetString("Game", "Tasks");
            else
                prompt += GetString("Game", "Task");
        } else {
            prompt += GetString("Game", "YouEarned") + " " + StateManager.firstDiceResult.ToString() + " ";
            if (StateManager.firstDiceResult > 1)
                prompt += GetString("Game", "Debts");
            else
                prompt += GetString("Game", "Debt");
        }
        if(StateManager.alreadyReRoll){
            prompt += " " + GetString("Game", "And") + " " + StateManager.secondDiceResult.ToString() + " ";
            if (StateManager.secondTaskOrDebtChoice == "TASK"){
                if (StateManager.secondDiceResult > 1)
                    prompt += GetString("Game", "Tasks");
                else
                    prompt += GetString("Game", "Task");
            } else {
                if (StateManager.secondDiceResult > 1)
                    prompt += GetString("Game", "Debts");
                else
                    prompt += GetString("Game", "Debt");
            }
        }
        prompt+=".";
        this.results.GetComponent<Results>().ChangeText(prompt);

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

        animationManager.HideResults();
        yield return new WaitUntil(() => EventManager.animate == false);

        if (debtCounter > 0){
            animationManager.UpdateDebtScrollBar(this.debtSlider.value - debtCounter);
            yield return new WaitUntil(() => EventManager.animate == false);
        }

        if (taskCounter > 0){
            EventManager.handleAddingTask = true;
            StartCoroutine(AddTasks(taskCounter));
            yield return new WaitUntil(() => EventManager.handleAddingTask == false);
        }
        if (StateManager.oneMoreTaskPerRoll == true){
            EventManager.permanentCardShowned = false;
            this.cardHandler.ShowOneMoreTaskPerRollPermanent();
            yield return new WaitUntil(() => EventManager.permanentCardShowned == true);
            yield return new WaitForSeconds(2f);
            EventManager.readyToHidePermanent = true;
            yield return new WaitUntil(() => EventManager.permanentCardHidden == true);
            EventManager.permanentCardHidden = false;
            int bonusTasks = 1;
            if(StateManager.alreadyReRoll)
                bonusTasks++;
            EventManager.handleAddingTask = true;
            StartCoroutine(AddTasks(bonusTasks));
            yield return new WaitUntil(() => EventManager.handleAddingTask == false);
        }
        if (StateManager.currentPlayer.twoMoreTasksPerRoll == true){
            EventManager.permanentCardShowned = false;
            this.cardHandler.ShowTwoMoreTasksPerRollPermanent();
            yield return new WaitUntil(() => EventManager.permanentCardShowned == true);
            yield return new WaitForSeconds(2f);
            EventManager.readyToHidePermanent = true;
            yield return new WaitUntil(() => EventManager.permanentCardHidden == true);
            EventManager.permanentCardHidden = false;
            int bonusTasks = 1;
            if(StateManager.alreadyReRoll)
                bonusTasks++;
            EventManager.handleAddingTask = true;
            StartCoroutine(AddTasks(bonusTasks * 2));
            yield return new WaitUntil(() => EventManager.handleAddingTask == false);
        }
        if (StateManager.currentPlayer.decreaseDebtPerTurn == true){
            EventManager.permanentCardShowned = false;
            this.cardHandler.ShowDecreaseDebtPerTurnPermanent();
            yield return new WaitUntil(() => EventManager.permanentCardShowned == true);
            yield return new WaitForSeconds(2f);
            EventManager.readyToHidePermanent = true;
            yield return new WaitUntil(() => EventManager.permanentCardHidden == true);
            EventManager.permanentCardHidden = false;
            animationManager.UpdateDebtScrollBar(this.debtSlider.value - 1);
            yield return new WaitUntil(() => EventManager.animate == false);
        }
        yield return new WaitForSeconds(1);

        bool isFinished = true;
        foreach (GameObject aus in doingAUS){
            if (aus.GetComponent<UserStoryUI>().userStory.state != UserStory.State.DONE)
                isFinished = false;
        }
        if (isFinished && EventManager.allFilledChoiceMade == false){
            StartCoroutine(filledChoice.HandleFilledChoice());
            yield return new WaitUntil(() => EventManager.allFilledChoiceMade == true);
            if (filledChoice.debtClicked){
                yield return new WaitForSeconds(1);
                StateManager.turnState = StateManager.TurnState.END_OF_TURN;
            }
            else if (filledChoice.moreUSClicked){
                EventManager.allFilledChoiceMade = false;
                yield return new WaitForSeconds(1);
                StateManager.turnState = StateManager.TurnState.NEW_US_ADDED;
            }
            else if (filledChoice.endSprintClicked){
                EventManager.allFilledChoiceMade = false;
                yield return new WaitForSeconds(1);
                StateManager.turnState = StateManager.TurnState.WANT_TO_PASS;
            }
        } else {
            if (!isFinished){
                EventManager.allFilledChoiceMade = false;
                EventManager.onlyDebt = false;
            }
            yield return new WaitForSeconds(1);
            StateManager.turnState = StateManager.TurnState.END_OF_TURN;
        }
    }
    #endregion
    
    #region --------------------------------- Utils ---------------------------------
        void ClearTurn(){
        StateManager.ClearTurnState();
        this.results.GetComponent<Results>().ChangeText("");
        this.turnOfTxt.text = "";
    }

    void InitState(){
        StateManager.language = LocalizationSettings.SelectedLocale;
        StateManager.difficulty = StateManager.Difficulty.HARD;
        StateManager.category = StateManager.Category.GIFT_SHOP;
        StateManager.gameName = "";
        StateManager.pokerPlanning = false;
        StateManager.CreatePlayers(new List<string>{"Alice"});
        // StateManager.CreatePlayers(new List<string>{"Alice", "Bob", "Charles"});
        StateManager.CreateUserStories(StateManager.Category.GIFT_SHOP);
        foreach (UserStory userStory in StateManager.userStories){
            userStory.size = userStory.defaultSize;
        }

        StateManager.gameState = StateManager.GameState.INITIALISATION;
    }

    void InitSprintState(){
        StateManager.sprintBeginTime = DateTime.Now;
        StateManager.sprintStars = StateManager.starsNumber;
        StateManager.sprintDebt = StateManager.currentDebt;
        StateManager.sprintFinishedUS = StateManager.finishedUS;
        StateManager.sprintProblemCards = 0;
        StateManager.sprintLoosedTasks = 0;
        EventManager.onlyDebt = false;
        EventManager.allFilledChoiceMade = false;
    }

    public IEnumerator AddTasks(int n){
        // if (n>0)
        //     n = 1000*n;
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
            if (EventManager.taskToAdd == 0 || reasonsToContinue <= 0){
                    this.taskValidation.interactable = true;
                } else {
                    this.taskValidation.interactable = false;
                }
            yield return null;
        }
        yield return new WaitUntil(() => EventManager.taskAdded == true);
        this.taskValidation.gameObject.SetActive(false);
        if (n < 0){
            EventManager.allFilledChoiceMade = false;
            EventManager.onlyDebt = false;
            StateManager.loosedTasks -= n - EventManager.taskToAdd;
            StateManager.sprintLoosedTasks -= n - EventManager.taskToAdd;
            burndownChartManager.currentSprint.currentDay.AddTasks(n - EventManager.taskToAdd);
        }
        else if (n > 0){
            StateManager.totalTasks += n - EventManager.taskToAdd;
            burndownChartManager.currentSprint.currentDay.AddTasks(n - EventManager.taskToAdd);
        }
        foreach (GameObject doingAUS in this.doingAUS){
            doingAUS.GetComponent<ArrowedUS>().HideArrows();
            doingAUS.GetComponent<ArrowedUS>().delta = 0;
        }
        EventManager.handleAddingTask = false;
    }

    public void OnTaskValidationClick(){
        EventManager.taskAdded = true;
    }

    public string GetString(string tableName, string stringKey){
        return LocalizationSettings.StringDatabase.GetLocalizedString(tableName, stringKey);
    }
    #endregion
}
