using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CardHandler : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    List<Card> dailyCards;
    List<Card> problemCards;
    List<Card> reviewCards;

    List<Card> remainingDailyCards = new List<Card>();
    List<Card> remainingProblemCards = new List<Card>();
    List<Card> remainingReviewCards = new List<Card>();

    List<Card> discardedDailyCards;
    List<Card> discardedProblemCards;
    List<Card> discardedReviewCards;

    List<Card> pickedCards = new List<Card>();

    [SerializeField] CardPicker cardPicker;
    [SerializeField] AnimationManager animationManager;
    [SerializeField] Slider debtSlider;
    [SerializeField] Button okButton;
    [SerializeField] Button choiceOneButton;
    [SerializeField] Button choiceTwoButton;
    [SerializeField] Button acceptButton;
    [SerializeField] Button declineButton;
    [SerializeField] UIDice uiDice;

    private System.Random random;

    void Start(){
        this.random = new System.Random();
    }

    IEnumerator HandleCards(){
        animationManager.ShowCardPick();
        yield return new WaitUntil(() => EventManager.animate == false);
        while (EventManager.cardsToPick > 0 || this.cardPicker.choosenCard != null){
            yield return new WaitUntil(() => this.cardPicker.choosenCard != null);
            yield return new WaitForSeconds(2);
            Debug.Log($"Handle Card : {this.cardPicker.choosenCard.GetComponent<UICard>().card.ToString()}");
            EventManager.handleSingleCard = true;
            StartCoroutine(HandleSingleCard(this.cardPicker.choosenCard.GetComponent<UICard>().card));
            yield return new WaitUntil(() => EventManager.handleSingleCard == false);
            this.cardPicker.choosenCard = null;
        }
        this.cardPicker.Reset();
        EventManager.handleCards = false;
    }
    
    IEnumerator HandleSingleCard(Card card){
        int doubleValue = 1;
        if (StateManager.skipProblemOrDoubleDaily > 0 && card.positive == true){
            StateManager.skipProblemOrDoubleDaily--;
            doubleValue = 2;
        }
        if(StateManager.noMoreTestIssues == true && card.test == true){
            // montrer la carte permanente
            // ne rien faire
        } else if (StateManager.skipProblemOrDoubleDaily > 0 && card.category == Card.CategoryOfCard.PROBLEM){
            StateManager.skipProblemOrDoubleDaily--;
            // animate end of card
            yield return new WaitForSeconds(1f);
        } else {
            switch (card.typeOfCard){
                case Card.TypeOfCard.Simple :
                    EventManager.handleSimpleAction = true;
                    StartCoroutine(HandleSimpleAction(card.firstAction, card.firstValue * doubleValue));
                    yield return new WaitUntil(() => EventManager.handleSimpleAction == false);
                    break;
                case Card.TypeOfCard.Multiple :
                    EventManager.handleMultipleActions = true;
                    StartCoroutine(HandleMultipleActions(card.firstAction, card.firstValue * doubleValue, card.secondAction, card.secondValue * doubleValue));
                    yield return new WaitUntil(() => EventManager.handleMultipleActions == false);
                    break;
                case Card.TypeOfCard.Proposition :
                    EventManager.handlePropositionAction = true;
                    StartCoroutine(HandlePropositionAction(card.firstAction, card.firstValue * doubleValue, card.secondAction, card.secondValue * doubleValue, card.thirdAction, card.thirdValue * doubleValue));
                    yield return new WaitUntil(() => EventManager.handlePropositionAction == false);
                    break;
                case Card.TypeOfCard.Information :
                    EventManager.handleInformationAction = true;
                    StartCoroutine(HandleInformationAction());
                    yield return new WaitUntil(() => EventManager.handleInformationAction == false);
                    break;
                case Card.TypeOfCard.Choice :
                    EventManager.handleChoiceActions = true;
                    StartCoroutine(HandleChoiceActions(card.firstAction, card.firstValue * doubleValue, card.secondAction, card.secondValue * doubleValue));
                    yield return new WaitUntil(() => EventManager.handleChoiceActions == false);
                    break;
                case Card.TypeOfCard.Question :
                    EventManager.handleQuestionActions = true;
                    StartCoroutine(HandleQuestionActions(card.questionId));
                    yield return new WaitUntil(() => EventManager.handleQuestionActions == false);
                    break;
                case Card.TypeOfCard.Permanent :
                    EventManager.handlePermanentAction = true;
                    StartCoroutine(HandlePermanentAction(card.permanent));
                    yield return new WaitUntil(() => EventManager.handlePermanentAction == false);
                    break;
                default :
                    break;
            }
        }
        if (EventManager.cardsToPick != 0){
            animationManager.ShowCardPick();
            yield return new WaitUntil(() => EventManager.animate == false);
        }
        EventManager.handleSingleCard = false;
        yield break;
    }

    #region ###### Cards functions ######
    #region - - - - - - - - - - - - - - - - - Handle CardType - - - - - - - - - - - - - - - - -
    IEnumerator HandleAtomicAction(Card.Action action, float value){
        switch (action){
            case Card.Action.IncreaseTask :
                StartCoroutine(IncreaseTask((int) value));
                break;
            case Card.Action.DecreaseTask :
                StartCoroutine(IncreaseTask((int) -value));
                break;
            case Card.Action.IncreaseDebt :
                StartCoroutine(IncreaseDebt((int) value));
                break;
            case Card.Action.DecreaseDebt :
                StartCoroutine(IncreaseDebt((int) -value));
                break;
            case Card.Action.IncreaseTaskPerPlayer :
                StartCoroutine(IncreaseTask(((int) value) * (StateManager.players.Count + 2)));
                break;
            case Card.Action.IncreaseDebtPerPlayer :
                StartCoroutine(IncreaseDebt(((int) value) * (StateManager.players.Count + 2)));
                break;
            case Card.Action.DecreaseTaskPerPlayer :
                StartCoroutine(IncreaseTask(((int) -value) * (StateManager.players.Count + 2)));
                break;
            case Card.Action.DecreaseDebtPerPlayer :
                StartCoroutine(IncreaseDebt(((int) -value) * (StateManager.players.Count + 2)));
                break;
            case Card.Action.IncreaseTaskPerDevelopper :
                StartCoroutine(IncreaseTask(((int) value) * (StateManager.players.Count)));
                break;
            case Card.Action.IncreaseDebtPerDevelopper :
                StartCoroutine(IncreaseDebt(((int) value) * (StateManager.players.Count)));
                break;
            case Card.Action.DecreaseTaskPerDevelopper :
                StartCoroutine(IncreaseTask(((int) -value) * (StateManager.players.Count)));
                break;
            case Card.Action.DecreaseDebtPerDevelopper :
                StartCoroutine(IncreaseDebt(((int) -value) * (StateManager.players.Count)));
                break;
            case Card.Action.MultiplieDebt :
                StartCoroutine(MultiplieDebt(value));
                break;
            case Card.Action.DecreaseTaskPerCurrentDebt :
                StartCoroutine(DecreaseTaskPerCurrentDebt());
                break;
            case Card.Action.IncreaseTaskPerRoll : {
                EventManager.rolled = false;
                StartCoroutine(this.uiDice.RollDice());
                yield return new WaitUntil(() => EventManager.rolled == true);
                EventManager.rolled = false;
                int diceResult = this.uiDice.currentFace;
                StartCoroutine(IncreaseTask(diceResult));
                break;
            }
            case Card.Action.DecreaseTaskPerRoll : {
                EventManager.rolled = false;
                StartCoroutine(this.uiDice.RollDice());
                yield return new WaitUntil(() => EventManager.rolled == true);
                EventManager.rolled = false;
                int diceResult = this.uiDice.currentFace;
                StartCoroutine(IncreaseTask(-diceResult));
                break;
            }
            case Card.Action.IncreaseDebtPerRoll : {
                EventManager.rolled = false;
                StartCoroutine(this.uiDice.RollDice());
                yield return new WaitUntil(() => EventManager.rolled == true);
                EventManager.rolled = false;
                int diceResult = this.uiDice.currentFace;
                StartCoroutine(IncreaseDebt(diceResult));
                break;
            }
            case Card.Action.DecreaseDebtPerRoll : {
                EventManager.rolled = false;
                StartCoroutine(this.uiDice.RollDice());
                yield return new WaitUntil(() => EventManager.rolled == true);
                EventManager.rolled = false;
                int diceResult = this.uiDice.currentFace;
                StartCoroutine(IncreaseDebt(-diceResult));
                break;
            }
            case Card.Action.CurrentPlayerPassATurn :
                StartCoroutine(CurrentPlayerPassATurn((int) value));
                break;
            case Card.Action.NextPlayerPassATurn :
                StartCoroutine(NextPlayerPassATurn((int) value));
                break;
            case Card.Action.AllPlayersPassATurn :
                StartCoroutine(AllPlayersPassATurn((int) value));
                break;
            case Card.Action.PickDailyCards :
                StartCoroutine(PickDailyCards((int) value));
                break;
            case Card.Action.PickProblemCards :
                StartCoroutine(PickProblemCards((int)value));
                break;
            case Card.Action.PickReviewCards :
                StartCoroutine(PickReviewCards((int) value));
                break;
            case Card.Action.PickProblemCardsPerRoll : {
                EventManager.rolled = false;
                StartCoroutine(this.uiDice.RollDice());
                yield return new WaitUntil(() => EventManager.rolled == true);
                EventManager.rolled = false;
                int diceResult = this.uiDice.currentFace;
                if (diceResult <= 3){
                    StartCoroutine(PickProblemCards(diceResult));
                } else {
                    EventManager.action = false;
                }
                break;
            }
            case Card.Action.GetRidOfJinxCard :
                if (StateManager.jinxed == true){
                    // discard jinx card,
                    StateManager.jinxed = false;
                }
                EventManager.action = false;
                break;
            case Card.Action.SkipProblemOrDoubleDaily :
                StateManager.skipProblemOrDoubleDaily++;
                EventManager.action = false;
                break;
            case Card.Action.DecreaseMaxTaskNextSprint :
                EventManager.action = false;
                break;
            case Card.Action.IncreaseTaskNextSprint :
                // EventManager.taskToAdd(value);
                EventManager.action = false;
                break;
            default :
                EventManager.action = false;
                break;
        }
        yield return new WaitUntil(() => EventManager.action == false);
        EventManager.handleSimpleAction = false;
    }
    IEnumerator HandleSimpleAction(Card.Action action, float value){
        this.okButton.gameObject.SetActive(true);
        yield return new WaitUntil(() => EventManager.okPressed == true);
        EventManager.okPressed = false;
        this.okButton.gameObject.SetActive(false);
        EventManager.action = true;
        StartCoroutine(HandleAtomicAction(action, value));
        yield return new WaitUntil(() => EventManager.action == false);
        EventManager.handleSimpleAction = false;
    }
    IEnumerator HandleMultipleActions(Card.Action firstAction, float firstValue, Card.Action secondAction, float secondValue){
        this.okButton.gameObject.SetActive(true);
        yield return new WaitUntil(() => EventManager.okPressed == true);
        EventManager.okPressed = false;
        this.okButton.gameObject.SetActive(false);
        EventManager.action = true;
        StartCoroutine(HandleAtomicAction(firstAction, firstValue));
        yield return new WaitUntil(() => EventManager.action == false);
        EventManager.action = true;
        StartCoroutine(HandleAtomicAction(secondAction, secondValue));
        yield return new WaitUntil(() => EventManager.action == false);
        EventManager.handleMultipleActions = false;
    }
    IEnumerator HandlePropositionAction(Card.Action firstAction, float firstValue, Card.Action secondAction, float secondValue, Card.Action thirdAction, float thirdValue){
        this.acceptButton.gameObject.SetActive(true);
        this.declineButton.gameObject.SetActive(true);
        yield return new WaitUntil(() => EventManager.acceptPressed == true || EventManager.declinePressed == true);
        bool accept = EventManager.acceptPressed;
        EventManager.acceptPressed = false;
        EventManager.declinePressed = false;
        this.acceptButton.gameObject.SetActive(false);
        this.declineButton.gameObject.SetActive(false);
        if(accept){
            EventManager.action = true;
            StartCoroutine(HandleAtomicAction(firstAction, firstValue));
            yield return new WaitUntil(() => EventManager.action == false);
            EventManager.action = true;
            StartCoroutine(HandleAtomicAction(secondAction, secondValue));
            yield return new WaitUntil(() => EventManager.action == false);
            EventManager.action = true;
            StartCoroutine(HandleAtomicAction(thirdAction, thirdValue));
            yield return new WaitUntil(() => EventManager.action == false);
        }
        EventManager.handlePropositionAction = false;
    }
    IEnumerator HandleInformationAction(){
        this.okButton.gameObject.SetActive(true);
        yield return new WaitUntil(() => EventManager.okPressed == true);
        EventManager.okPressed = false;
        this.okButton.gameObject.SetActive(false);
        // Do nothing
        EventManager.handleInformationAction = false;
        yield break;
    }
    IEnumerator HandleChoiceActions(Card.Action firstAction, float firstValue, Card.Action secondAction, float secondValue){
        this.choiceOneButton.gameObject.SetActive(true);
        this.choiceTwoButton.gameObject.SetActive(true);
        yield return new WaitUntil(() => EventManager.choiceOnePressed == true || EventManager.choiceTwoPressed == true);
        bool choiceOne = EventManager.choiceOnePressed;
        EventManager.choiceOnePressed = false;
        EventManager.choiceTwoPressed = false;
        this.choiceOneButton.gameObject.SetActive(false);
        this.choiceTwoButton.gameObject.SetActive(false);
        if(choiceOne){
            EventManager.action = true;
            StartCoroutine(HandleAtomicAction(firstAction, firstValue));
            yield return new WaitUntil(() => EventManager.action == false);
        }
        else{
            EventManager.action = true;
            StartCoroutine(HandleAtomicAction(secondAction, secondValue));
            yield return new WaitUntil(() => EventManager.action == false);
        }
        EventManager.handleChoiceActions = false;
    }
    IEnumerator HandleQuestionActions(int questionId){
        // En fonction de la question
        // -> Afficher des réponses en bouton
        // -> Si correct, exécuter la récompense de la question
        // -> Si incorrect, exécuter la perte de question
        EventManager.handleQuestionActions = false;
        yield break;
    }
    IEnumerator HandlePermanentAction(Card.Permanent permanent){
        switch (permanent){
            case Card.Permanent.Jinx : {
                this.okButton.gameObject.SetActive(true);
                yield return new WaitUntil(() => EventManager.okPressed == true);
                EventManager.okPressed = false;
                this.okButton.gameObject.SetActive(false);
                StateManager.jinxed = true;
                break;
            }
            case Card.Permanent.NoMoreTestIssues : {
                this.okButton.gameObject.SetActive(true);
                yield return new WaitUntil(() => EventManager.okPressed == true);
                EventManager.okPressed = false;
                this.okButton.gameObject.SetActive(false);
                StateManager.noMoreTestIssues = true;
                break;
            }
            case Card.Permanent.OneMoreTaskPerRoll : {
                this.acceptButton.gameObject.SetActive(true);
                this.declineButton.gameObject.SetActive(true);
                yield return new WaitUntil(() => EventManager.acceptPressed == true || EventManager.declinePressed == true);
                bool accept = EventManager.acceptPressed;
                EventManager.acceptPressed = false;
                EventManager.declinePressed = false;
                this.acceptButton.gameObject.SetActive(false);
                this.declineButton.gameObject.SetActive(false);
                if(accept){
                    EventManager.action = true;
                    StartCoroutine(HandleAtomicAction(Card.Action.CurrentPlayerPassATurn, 4));
                    yield return new WaitUntil(() => EventManager.action == false);
                    StateManager.oneMoreTaskPerRoll = true;
                }
                break;
            }
            case Card.Permanent.TasksOnBeginSprint : {
                this.okButton.gameObject.SetActive(true);
                yield return new WaitUntil(() => EventManager.okPressed == true);
                EventManager.okPressed = false;
                this.okButton.gameObject.SetActive(false);
                StateManager.tasksOnBeginSprint = true;
                break;
            }
            case Card.Permanent.TwoMoreTasksPerRoll : {
                this.acceptButton.gameObject.SetActive(true);
                this.declineButton.gameObject.SetActive(true);
                yield return new WaitUntil(() => EventManager.acceptPressed == true || EventManager.declinePressed == true);
                bool accept = EventManager.acceptPressed;
                EventManager.acceptPressed = false;
                EventManager.declinePressed = false;
                this.acceptButton.gameObject.SetActive(false);
                this.declineButton.gameObject.SetActive(false);
                if(accept){
                    EventManager.action = true;
                    StartCoroutine(HandleAtomicAction(Card.Action.CurrentPlayerPassATurn, 5));
                    yield return new WaitUntil(() => EventManager.action == false);
                    StateManager.currentPlayer.twoMoreTasksPerRoll = true;
                }
                break;
            }
            case Card.Permanent.MaxUserStoriesLowered : { // TODO : handle la carte
                this.okButton.gameObject.SetActive(true);
                yield return new WaitUntil(() => EventManager.okPressed == true);
                EventManager.okPressed = false;
                this.okButton.gameObject.SetActive(false);
                StateManager.maxUserStoryLowered = true;
                break;
            }
            case Card.Permanent.DecreaseDebtPerTurn : {
                this.acceptButton.gameObject.SetActive(true);
                this.declineButton.gameObject.SetActive(true);
                yield return new WaitUntil(() => EventManager.acceptPressed == true || EventManager.declinePressed == true);
                bool accept = EventManager.acceptPressed;
                EventManager.acceptPressed = false;
                EventManager.declinePressed = false;
                this.acceptButton.gameObject.SetActive(false);
                this.declineButton.gameObject.SetActive(false);
                if(accept){
                    EventManager.action = true;
                    StartCoroutine(HandleAtomicAction(Card.Action.CurrentPlayerPassATurn, 3));
                    yield return new WaitUntil(() => EventManager.action == false);
                    StateManager.currentPlayer.decreaseDebtPerTurn = true;
                }
                break;
            }
            case Card.Permanent.OneTaskPerDay : {
                this.okButton.gameObject.SetActive(true);
                yield return new WaitUntil(() => EventManager.okPressed == true);
                EventManager.okPressed = false;
                this.okButton.gameObject.SetActive(false);
                StateManager.oneTaskPerDay = true;
                break;
            }
            default :
                break;
        }
        EventManager.handlePermanentAction = false;
        yield break;
    }
    #endregion
    
    #region - - - - - - - - - - - - - - - - - Card Actions - - - - - - - - - - - - - - - - -
    IEnumerator IncreaseTask(int n){
        yield return new WaitUntil(() => EventManager.action == true);
        EventManager.handleAddingTask = true;
        animationManager.HideCardPick();
        yield return new WaitUntil(() => EventManager.animate == false);
        StartCoroutine(gameManager.AddTasks(n));
        yield return new WaitUntil(() => EventManager.handleAddingTask == false);
        EventManager.action = false;
    }
    IEnumerator IncreaseDebt(int n){
        yield return new WaitUntil(() => EventManager.action == true);
        animationManager.HideCardPick();
        yield return new WaitUntil(() => EventManager.animate == false);
        animationManager.UpdateDebtScrollBar(this.debtSlider.value + n);
        yield return new WaitUntil(() => EventManager.animate == false);
        EventManager.action = false;
    }
    IEnumerator IncreaseTaskPerPlayer(int n){
        yield return new WaitUntil(() => EventManager.action == true);
        EventManager.action = false;
    }
    IEnumerator DecreaseTaskPerPlayer(int n){
        yield return new WaitUntil(() => EventManager.action == true);
        EventManager.action = false;
    }
    IEnumerator MultiplieDebt(float n){
        yield return new WaitUntil(() => EventManager.action == true);
        animationManager.HideCardPick();
        yield return new WaitUntil(() => EventManager.animate == false);
        animationManager.UpdateDebtScrollBar(Mathf.RoundToInt(this.debtSlider.value * n));
        yield return new WaitUntil(() => EventManager.animate == false);
        EventManager.action = false;
    }
    IEnumerator DecreaseTaskPerCurrentDebt(){
        yield return new WaitUntil(() => EventManager.action == true);
        EventManager.action = false;
        yield break;
    }
    IEnumerator CurrentPlayerPassATurn(int n){
        yield return new WaitUntil(() => EventManager.action == true);
        StateManager.players[StateManager.currentPlayer.playerNumber-1].turnToPass = n;
        EventManager.action = false;
        yield break;
    }
    IEnumerator NextPlayerPassATurn(int n){
        yield return new WaitUntil(() => EventManager.action == true);
        StateManager.players[StateManager.currentPlayer.nextPlayerNumber-1].turnToPass = n;
        EventManager.action = false;
        yield break;
    }
    IEnumerator AllPlayersPassATurn(int n){
        yield return new WaitUntil(() => EventManager.action == true);
        foreach (Player player in StateManager.players){
            player.turnToPass = n;
        }
        EventManager.action = false;
        yield break;
    }
    IEnumerator PickProblemCards(int n){
        yield return new WaitUntil(() => EventManager.action == true);
        EventManager.cardsToPick += n;
        // animate updeck
        yield return new WaitUntil(() => EventManager.animate == false);
        for (int i = 0; i < n; i++){
            if (this.problemCards.Count < 1){
                this.problemCards.AddRange(this.discardedProblemCards);
                this.discardedProblemCards = new List<Card>();
            }
            int index = random.Next(0, this.problemCards.Count);
            Card choosenCard = this.problemCards[index];
            this.cardPicker.AddCart(choosenCard);
            yield return new WaitUntil(() => EventManager.animate == false);
            this.discardedProblemCards.Add(choosenCard);
            this.problemCards.Remove(choosenCard);
        }
        // animate downdeck
        yield return new WaitUntil(() => EventManager.animate == false);
        EventManager.action = false;
    }
    IEnumerator PickDailyCards(int n){
        yield return new WaitUntil(() => EventManager.action == true);
        EventManager.cardsToPick += n;
        // animate updeck
        yield return new WaitUntil(() => EventManager.animate == false);
        for (int i = 0; i < n; i++){
            if (this.dailyCards.Count < 1){
                this.dailyCards.AddRange(this.discardedDailyCards);
                this.discardedDailyCards = new List<Card>();
            }
            int index = random.Next(0, this.dailyCards.Count);
            Card choosenCard = this.dailyCards[index];
            this.cardPicker.AddCart(choosenCard);
            yield return new WaitUntil(() => EventManager.animate == false);
            this.discardedDailyCards.Add(choosenCard);
            this.dailyCards.Remove(choosenCard);
        }
        // animate downdeck
        yield return new WaitUntil(() => EventManager.animate == false);
        EventManager.action = false;
    }
    IEnumerator PickReviewCards(int n){
        yield return new WaitUntil(() => EventManager.action == true);
        EventManager.cardsToPick += n;
        // animate updeck
        yield return new WaitUntil(() => EventManager.animate == false);
        for (int i = 0; i < n; i++){
            if (this.reviewCards.Count < 1){
                this.reviewCards.AddRange(this.discardedReviewCards);
                this.discardedReviewCards = new List<Card>();
            }
            int index = random.Next(0, this.reviewCards.Count);
            Card choosenCard = this.reviewCards[index];
            this.cardPicker.AddCart(choosenCard);
            yield return new WaitUntil(() => EventManager.animate == false);
            this.discardedReviewCards.Add(choosenCard);
            this.reviewCards.Remove(choosenCard);
        }
        // animate downdeck
        yield return new WaitUntil(() => EventManager.animate == false);
        EventManager.action = false;
    }
    #endregion

    #region - - - - - - - - - - - - - - - - - FirstCards - - - - - - - - - - - - - - - - -
    public IEnumerator FirstPickDailyCard(){
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.PICK_DAILY);
        EventManager.cardsToPick = 1;
        for (int i = 0; i < 3; i++){
            if (this.remainingDailyCards.Count < 1){
                this.remainingDailyCards.AddRange(this.discardedDailyCards);
                this.discardedDailyCards = new List<Card>();
            }
            int index = random.Next(0, this.remainingDailyCards.Count);
            Card choosenCard = this.remainingDailyCards[index];
            this.cardPicker.AddCart(choosenCard);
            this.pickedCards.Add(choosenCard);
            this.remainingDailyCards.Remove(choosenCard);
        }
        EventManager.handleCards = true;
        StartCoroutine(HandleCards());
        yield return new WaitUntil(() => EventManager.handleCards == false);
        StateManager.gameState = StateManager.GameState.PLAYER_TURN;
    }

    public IEnumerator FirstPickProblemCard(){
        yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.PROBLEM);
        EventManager.cardsToPick = 1;
        // Card customPickedCard = this.problemCards[51];
        // this.cardPicker.AddCart(customPickedCard);
        // this.pickedCards.Add(customPickedCard);
        // this.remainingProblemCards.Remove(customPickedCard);
        for (int i = 0; i < 3; i++){
            if (this.remainingProblemCards.Count <= 0){
                this.remainingProblemCards.AddRange(this.discardedProblemCards);
                this.discardedProblemCards = new List<Card>();
            }
            int index = random.Next(0, this.remainingProblemCards.Count);
            Card choosenCard = this.remainingProblemCards[index];
            this.cardPicker.AddCart(choosenCard);
            this.pickedCards.Add(choosenCard);
            this.remainingProblemCards.Remove(choosenCard);
        }
        EventManager.handleCards = true;
        StartCoroutine(HandleCards());
        yield return new WaitUntil(() => EventManager.handleCards == false);
        StateManager.turnState = StateManager.TurnState.RESULT;
    }
    public IEnumerator FirstPickReviewCard(){
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.REVIEW);
        EventManager.cardsToPick = 1;
        for (int i = 0; i < 3; i++){
            if (this.remainingReviewCards.Count < 1){
                this.remainingReviewCards.AddRange(this.discardedReviewCards);
                this.discardedReviewCards = new List<Card>();
            }
            int index = random.Next(0, this.remainingReviewCards.Count);
            Card choosenCard = this.remainingReviewCards[index];
            this.cardPicker.AddCart(choosenCard);
            this.pickedCards.Add(choosenCard);
            this.remainingReviewCards.Remove(choosenCard);
        }
        EventManager.handleCards = true;
        StartCoroutine(HandleCards());
        yield return new WaitUntil(() => EventManager.handleCards == false);
        StateManager.gameState = StateManager.GameState.RETROSPECTIVE;
    }
    #endregion
    #endregion
    #region ###### Initialisation ######
    public void CreateDailyCards(){
        string path = "";
        if (StateManager.language == LocalizationSettings.AvailableLocales.GetLocale("en")){
            path = Application.streamingAssetsPath + "/Cards/DailyCards_EN.json";
        } else {
            path = Application.streamingAssetsPath + "/Cards/DailyCards_FR.json";
        }
        string dailyCardsStr = File.ReadAllText(path);
        this.dailyCards = JsonConvert.DeserializeObject<List<Card>>(dailyCardsStr);
        this.remainingDailyCards.AddRange(this.dailyCards);
        this.discardedDailyCards = new List<Card>();
        foreach (Card card in this.dailyCards){
            Debug.Log(card.ToString());
        }
    }

    public void CreateProblemCards(){
        string path = "";
        if (StateManager.language == LocalizationSettings.AvailableLocales.GetLocale("en")){
            path = Application.streamingAssetsPath + "/Cards/ProblemCards_EN.json";
        } else {
            path = Application.streamingAssetsPath + "/Cards/ProblemCards_FR.json";
        }
        string problemCardsStr = File.ReadAllText(path);
        this.problemCards = JsonConvert.DeserializeObject<List<Card>>(problemCardsStr);
        this.remainingProblemCards.AddRange(this.problemCards);
        this.discardedProblemCards = new List<Card>();
        foreach (Card card in this.problemCards){
            Debug.Log(card.ToString());
        }
    }

    public void CreateReviewCards(){
        string path = "";
        if (StateManager.language == LocalizationSettings.AvailableLocales.GetLocale("en")){
            path = Application.streamingAssetsPath + "/Cards/ReviewCards_EN.json";
        } else {
            path = Application.streamingAssetsPath + "/Cards/ReviewCards_FR.json";
        }
        string reviewCardsStr = File.ReadAllText(path);
        this.reviewCards = JsonConvert.DeserializeObject<List<Card>>(reviewCardsStr);
        this.remainingReviewCards.AddRange(this.reviewCards);
        this.discardedReviewCards = new List<Card>();
        foreach (Card card in this.reviewCards){
            Debug.Log(card.ToString());
        }
    }
    #endregion
    #region ###### Clickables ######
    public void OnOkClick(){
        EventManager.okPressed = true;
    }
    public void OnChoiceOneClick(){
        EventManager.choiceOnePressed = true;
    }
    public void OnChoiceTwoClick(){
        EventManager.choiceTwoPressed = true;
    }
    public void OnAcceptClick(){
        EventManager.acceptPressed = true;
    }
    public void OnDeclineClick(){
        EventManager.declinePressed = true;
    }
    #endregion
}
