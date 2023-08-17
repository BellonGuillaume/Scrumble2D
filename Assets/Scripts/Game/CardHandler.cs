using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CardHandler : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] BurndownChartManager burndownChartManager;

    public List<Card> dailyCards;
    public List<Card> problemCards;
    public List<Card> reviewCards;

    List<Card> remainingDailyCards = new List<Card>();
    List<Card> remainingProblemCards = new List<Card>();
    List<Card> remainingReviewCards = new List<Card>();

    List<Card> discardedDailyCards;
    List<Card> discardedProblemCards;
    List<Card> discardedReviewCards;

    [SerializeField] CardPicker cardPicker;
    [SerializeField] AnimationManager animationManager;
    [SerializeField] Slider debtSlider;
    [SerializeField] Button okButton;
    [SerializeField] Button choiceOneButton;
    [SerializeField] Button choiceTwoButton;
    [SerializeField] Button acceptButton;
    [SerializeField] Button declineButton;
    [SerializeField] UIDice uiDice;

    [SerializeField] QuestionHandler questionHandler;

    [SerializeField] UICard permanentCard1;
    [SerializeField] UICard permanentCard2;
    [SerializeField] UICard permanentCard3;
    [SerializeField] UICard permanentCard4;
    [SerializeField] UICard permanentCard5;
    [SerializeField] UICard permanentCard6;
    [SerializeField] UICard permanentCard7;
    [SerializeField] UICard permanentCard8;
    [SerializeField] UICard permanentCard9;

    private System.Random random;

    void Start(){
        this.random = new System.Random();
    }

    IEnumerator HandleCards(){
        animationManager.ShowCardPick();
        yield return new WaitUntil(() => EventManager.animate == false);
        while (EventManager.cardsToPick > 0 || this.cardPicker.centralCard.GetComponent<UICard>().card != null){
            yield return new WaitUntil(() => this.cardPicker.centralCard.GetComponent<UICard>().card != null);
            yield return new WaitForSeconds(2);
            EventManager.handleSingleCard = true;
            StartCoroutine(HandleSingleCard(this.cardPicker.choosenCard.GetComponent<UICard>()));
            yield return new WaitUntil(() => EventManager.handleSingleCard == false);
        }
        this.cardPicker.RemoveCards();
        ReDeckUnflippedCards();
        DiscardCards();
        yield return new WaitUntil(() => EventManager.cardToRemove <= 0);
        this.cardPicker.Reset();
        if (cardPicker.gameObject.activeSelf == true){
            animationManager.HideCardPick();
            yield return new WaitUntil(() => EventManager.animate == false);
        }
        EventManager.handleCards = false;
    }

    public void ReDeckUnflippedCards(){
        foreach (GameObject go in cardPicker.cards){
            UICard uiCard = go.GetComponent<UICard>();
            if (uiCard.flipped == false){
                Debug.Log($"Redeck card {uiCard.card.id}");
                if (uiCard.card.category == Card.CategoryOfCard.DAILY)
                    this.remainingDailyCards.Add(uiCard.card);
                else if (uiCard.card.category == Card.CategoryOfCard.PROBLEM)
                    this.remainingProblemCards.Add(uiCard.card);
                else if (uiCard.card.category == Card.CategoryOfCard.REVIEW)
                    this.remainingReviewCards.Add(uiCard.card);
            }
        }
    }

    public void DiscardCards(){
        foreach (GameObject go in cardPicker.cards){
            UICard uiCard = go.GetComponent<UICard>();
            if (uiCard.readyToDiscard == true){
                Debug.Log($"Discard card {uiCard.card.id}");
                if (uiCard.card.category == Card.CategoryOfCard.DAILY)
                    this.discardedDailyCards.Add(uiCard.card);
                else if (uiCard.card.category == Card.CategoryOfCard.PROBLEM)
                    this.discardedProblemCards.Add(uiCard.card);
                else if (uiCard.card.category == Card.CategoryOfCard.REVIEW)
                    this.discardedReviewCards.Add(uiCard.card);
            }
        }
    }
    
    IEnumerator HandleSingleCard(UICard uiCard){
        int doubleValue = 1;
        if (StateManager.skipProblemOrDoubleDaily > 0 && uiCard.card.category == Card.CategoryOfCard.DAILY && uiCard.card.positive == true){
            EventManager.permanentCardShowned = false;
            this.ShowSkipProblemOrDoubleDailyPermanent();
            yield return new WaitUntil(() => EventManager.permanentCardShowned == true);
            yield return new WaitForSeconds(2f);
            EventManager.readyToHidePermanent = true;
            yield return new WaitUntil(() => EventManager.permanentCardHidden == true);
            EventManager.permanentCardHidden = false;
            StateManager.skipProblemOrDoubleDaily--;
            if (StateManager.skipProblemOrDoubleDaily <= 0){
                DiscardPermanentCard(this.permanentCard8);
                yield return new WaitUntil(() => EventManager.animate == false);
            }
            doubleValue = 2;
        }
        if(StateManager.noMoreTestIssues == true && uiCard.card.category == Card.CategoryOfCard.PROBLEM && uiCard.card.test == true){
            EventManager.permanentCardShowned = false;
            this.ShowNoMoreTestIssuesPermanent();
            yield return new WaitUntil(() => EventManager.permanentCardShowned == true);
            this.okButton.gameObject.SetActive(true);
            yield return new WaitUntil(() => EventManager.okPressed == true);
            EventManager.okPressed = false;
            EventManager.readyToHidePermanent = true;
            yield return new WaitUntil(() => EventManager.permanentCardHidden == true);
            EventManager.permanentCardHidden = false;
            this.okButton.gameObject.SetActive(false);
            StartCoroutine(this.cardPicker.UnChooseCard());
            uiCard.readyToDiscard = true;
            yield return new WaitUntil(() => EventManager.animate == false);
        } else if (StateManager.skipProblemOrDoubleDaily > 0 && uiCard.card.category == Card.CategoryOfCard.PROBLEM){
            EventManager.permanentCardShowned = false;
            this.ShowSkipProblemOrDoubleDailyPermanent();
            yield return new WaitUntil(() => EventManager.permanentCardShowned == true);
            this.okButton.gameObject.SetActive(true);
            yield return new WaitUntil(() => EventManager.okPressed == true);
            EventManager.okPressed = false;
            this.okButton.gameObject.SetActive(false);
            EventManager.readyToHidePermanent = true;
            yield return new WaitUntil(() => EventManager.permanentCardHidden == true);
            EventManager.permanentCardHidden = false;
            StateManager.skipProblemOrDoubleDaily--;
            if (StateManager.skipProblemOrDoubleDaily <= 0){
                DiscardPermanentCard(this.permanentCard8);
                yield return new WaitUntil(() => EventManager.animate == false);
            }
            StartCoroutine(this.cardPicker.UnChooseCard());
            uiCard.readyToDiscard = true;
            yield return new WaitUntil(() => EventManager.animate == false);
        } else {
            switch (uiCard.card.typeOfCard){
                case Card.TypeOfCard.Simple :
                    EventManager.handleSimpleAction = true;
                    StartCoroutine(HandleSimpleAction(uiCard.card.firstAction, uiCard.card.firstValue * doubleValue));
                    yield return new WaitUntil(() => EventManager.handleSimpleAction == false);
                    uiCard.readyToDiscard = true;
                    break;
                case Card.TypeOfCard.Multiple :
                    EventManager.handleMultipleActions = true;
                    StartCoroutine(HandleMultipleActions(uiCard.card.firstAction, uiCard.card.firstValue * doubleValue, uiCard.card.secondAction, uiCard.card.secondValue * doubleValue));
                    yield return new WaitUntil(() => EventManager.handleMultipleActions == false);
                    uiCard.readyToDiscard = true;
                    break;
                case Card.TypeOfCard.Proposition :
                    EventManager.handlePropositionAction = true;
                    StartCoroutine(HandlePropositionAction(uiCard.card.firstAction, uiCard.card.firstValue * doubleValue, uiCard.card.secondAction, uiCard.card.secondValue * doubleValue, uiCard.card.thirdAction, uiCard.card.thirdValue * doubleValue));
                    yield return new WaitUntil(() => EventManager.handlePropositionAction == false);
                    uiCard.readyToDiscard = true;
                    break;
                case Card.TypeOfCard.Information :
                    EventManager.handleInformationAction = true;
                    StartCoroutine(HandleInformationAction());
                    yield return new WaitUntil(() => EventManager.handleInformationAction == false);
                    uiCard.readyToDiscard = true;
                    break;
                case Card.TypeOfCard.Choice :
                    EventManager.handleChoiceActions = true;
                    StartCoroutine(HandleChoiceActions(uiCard.card.firstAction, uiCard.card.firstValue * doubleValue, uiCard.card.secondAction, uiCard.card.secondValue * doubleValue));
                    yield return new WaitUntil(() => EventManager.handleChoiceActions == false);
                    uiCard.readyToDiscard = true;
                    break;
                case Card.TypeOfCard.Question :
                    EventManager.handleQuestionActions = true;
                    StartCoroutine(HandleQuestionActions(uiCard.card));
                    yield return new WaitUntil(() => EventManager.handleQuestionActions == false);
                    uiCard.readyToDiscard = true;
                    break;
                case Card.TypeOfCard.Permanent :
                    EventManager.handlePermanentAction = true;
                    StartCoroutine(HandlePermanentAction(uiCard));
                    yield return new WaitUntil(() => EventManager.handlePermanentAction == false);
                    break;
                default :
                    break;
            }
        }
        if (EventManager.cardsToPick > 0 && cardPicker.gameObject.activeSelf == false){
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
                StartCoroutine(IncreaseTask(-StateManager.debtFactor));
                break;
            case Card.Action.IncreaseTaskPerRoll : {
                this.uiDice.gameObject.SetActive(true);
                EventManager.rolled = false;
                StartCoroutine(this.uiDice.RollDice());
                yield return new WaitUntil(() => EventManager.rolled == true);
                EventManager.rolled = false;
                int diceResult = this.uiDice.currentFace;
                this.uiDice.gameObject.SetActive(false);
                StartCoroutine(IncreaseTask(diceResult));
                break;
            }
            case Card.Action.DecreaseTaskPerRoll : {
                this.uiDice.gameObject.SetActive(true);
                EventManager.rolled = false;
                StartCoroutine(this.uiDice.RollDice());
                yield return new WaitUntil(() => EventManager.rolled == true);
                EventManager.rolled = false;
                int diceResult = this.uiDice.currentFace;
                this.uiDice.gameObject.SetActive(false);
                StartCoroutine(IncreaseTask(-diceResult));
                break;
            }
            case Card.Action.IncreaseDebtPerRoll : {
                this.uiDice.gameObject.SetActive(true);
                EventManager.rolled = false;
                StartCoroutine(this.uiDice.RollDice());
                yield return new WaitUntil(() => EventManager.rolled == true);
                EventManager.rolled = false;
                int diceResult = this.uiDice.currentFace;
                this.uiDice.gameObject.SetActive(false);
                StartCoroutine(IncreaseDebt(diceResult));
                break;
            }
            case Card.Action.DecreaseDebtPerRoll : {
                this.uiDice.gameObject.SetActive(true);
                EventManager.rolled = false;
                StartCoroutine(this.uiDice.RollDice());
                yield return new WaitUntil(() => EventManager.rolled == true);
                EventManager.rolled = false;
                int diceResult = this.uiDice.currentFace;
                this.uiDice.gameObject.SetActive(false);
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
                this.uiDice.gameObject.SetActive(true);
                EventManager.rolled = false;
                StartCoroutine(this.uiDice.RollDice());
                yield return new WaitUntil(() => EventManager.rolled == true);
                EventManager.rolled = false;
                int diceResult = this.uiDice.currentFace;
                this.uiDice.gameObject.SetActive(false);
                if (diceResult <= 3){
                    StartCoroutine(PickProblemCards(diceResult));
                } else {
                    EventManager.action = false;
                }
                break;
            }
            case Card.Action.GetRidOfJinxCard :
                if (StateManager.jinxed == true){
                    DiscardPermanentCard(permanentCard7);
                    yield return new WaitUntil(() => EventManager.animate == false);
                    StateManager.jinxed = false;
                }
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
        StartCoroutine(this.cardPicker.UnChooseCard());
        yield return new WaitUntil(() => EventManager.animate == false);
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
        StartCoroutine(this.cardPicker.UnChooseCard());
        yield return new WaitUntil(() => EventManager.animate == false);
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
        StartCoroutine(this.cardPicker.UnChooseCard());
        yield return new WaitUntil(() => EventManager.animate == false);
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
        StartCoroutine(this.cardPicker.UnChooseCard());
        yield return new WaitUntil(() => EventManager.animate == false);
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
        StartCoroutine(this.cardPicker.UnChooseCard());
        yield return new WaitUntil(() => EventManager.animate == false);
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
    IEnumerator HandleQuestionActions(Card card){
        EventManager.questionHandled = false;
        StartCoroutine(questionHandler.HandleQuestion(card));
        yield return new WaitUntil(() => EventManager.questionHandled == true);
        EventManager.questionHandled = false;
        if (questionHandler.rightAnswerIsFound == true){
            EventManager.action = true;
            StartCoroutine(HandleAtomicAction(questionHandler.successAction, questionHandler.value));
            yield return new WaitUntil(() => EventManager.action == false);
        } else {
            EventManager.action = true;
            StartCoroutine(HandleAtomicAction(questionHandler.failedAction, questionHandler.value));
            yield return new WaitUntil(() => EventManager.action == false);
        }
        questionHandler.ResetQuestionHandler();
        StartCoroutine(this.cardPicker.UnChooseCard());
        yield return new WaitUntil(() => EventManager.animate == false);
        EventManager.handleQuestionActions = false;
        yield break;
    }
    IEnumerator HandlePermanentAction(UICard uiCard){
        GameObject permanentPlaceholder = null;
        switch (uiCard.card.permanent){
            case Card.Permanent.NoMoreTestIssues : {
                this.okButton.gameObject.SetActive(true);
                yield return new WaitUntil(() => EventManager.okPressed == true);
                EventManager.okPressed = false;
                this.okButton.gameObject.SetActive(false);
                permanentPlaceholder = permanentCard1.gameObject;
                StateManager.noMoreTestIssues = true;
                StartCoroutine(this.cardPicker.PlacePermanentCard(permanentPlaceholder));
                yield return new WaitUntil(() => EventManager.animate == false);
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
                    permanentPlaceholder = permanentCard2.gameObject;
                    StateManager.currentPlayer.oneMoreTaskPerRoll = true;
                    StartCoroutine(this.cardPicker.PlacePermanentCard(permanentPlaceholder));
                    yield return new WaitUntil(() => EventManager.animate == false);
                } else {
                    StartCoroutine(this.cardPicker.UnChooseCard());
                    yield return new WaitUntil(() => EventManager.animate == false);
                    uiCard.readyToDiscard = true;
                }
                break;
            }
            case Card.Permanent.TasksOnBeginSprint : {
                this.okButton.gameObject.SetActive(true);
                yield return new WaitUntil(() => EventManager.okPressed == true);
                EventManager.okPressed = false;
                this.okButton.gameObject.SetActive(false);
                permanentPlaceholder = permanentCard3.gameObject;
                StateManager.tasksOnBeginSprint = true;
                StartCoroutine(this.cardPicker.PlacePermanentCard(permanentPlaceholder));
                yield return new WaitUntil(() => EventManager.animate == false);
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
                    permanentPlaceholder = permanentCard4.gameObject;
                    StateManager.currentPlayer.twoMoreTasksPerRoll = true;
                    StartCoroutine(this.cardPicker.PlacePermanentCard(permanentPlaceholder));
                    yield return new WaitUntil(() => EventManager.animate == false);
                } else {
                    StartCoroutine(this.cardPicker.UnChooseCard());
                    yield return new WaitUntil(() => EventManager.animate == false);
                    uiCard.readyToDiscard = true;
                }
                break;
            }
            case Card.Permanent.MaxUserStoriesLowered : {
                this.okButton.gameObject.SetActive(true);
                yield return new WaitUntil(() => EventManager.okPressed == true);
                EventManager.okPressed = false;
                this.okButton.gameObject.SetActive(false);
                permanentPlaceholder = permanentCard5.gameObject;
                StartCoroutine(this.cardPicker.PlacePermanentCard(permanentPlaceholder));
                StateManager.maxUserStoryLowered = true;
                UpdateCurrentUS();
                int newCurrentRemainingTasks = 0;
                float newInitialRemainingTasks = burndownChartManager.currentSprint.currentDay.plannedRemainingTasks + burndownChartManager.currentSprint.currentDay.plannedTasks;
                foreach (GameObject arrowedUS in gameManager.doingAUS){
                    newInitialRemainingTasks -= 3;
                    newCurrentRemainingTasks += (arrowedUS.GetComponent<ArrowedUS>().userStory.maxTask - arrowedUS.GetComponent<ArrowedUS>().userStory.currentTask);
                }
                burndownChartManager.currentSprint.currentDay.Update(newInitialRemainingTasks, newCurrentRemainingTasks);
                yield return new WaitUntil(() => EventManager.animate == false);
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
                    permanentPlaceholder = permanentCard6.gameObject;
                    StateManager.currentPlayer.decreaseDebtPerTurn = true;
                    StartCoroutine(this.cardPicker.PlacePermanentCard(permanentPlaceholder));
                    yield return new WaitUntil(() => EventManager.animate == false);
                } else {
                    StartCoroutine(this.cardPicker.UnChooseCard());
                    yield return new WaitUntil(() => EventManager.animate == false);
                    uiCard.readyToDiscard = true;
                }
                break;
            }
            case Card.Permanent.Jinx : {
                this.okButton.gameObject.SetActive(true);
                yield return new WaitUntil(() => EventManager.okPressed == true);
                EventManager.okPressed = false;
                this.okButton.gameObject.SetActive(false);
                permanentPlaceholder = permanentCard7.gameObject;
                StateManager.jinxed = true;
                StartCoroutine(this.cardPicker.PlacePermanentCard(permanentPlaceholder));
                yield return new WaitUntil(() => EventManager.animate == false);
                break;
            }
            case Card.Permanent.SkipProblemOrDoubleDaily : {
                this.okButton.gameObject.SetActive(true);
                yield return new WaitUntil(() => EventManager.okPressed == true);
                EventManager.okPressed = false;
                this.okButton.gameObject.SetActive(false);
                permanentPlaceholder = permanentCard8.gameObject;
                StartCoroutine(this.cardPicker.PlacePermanentCard(permanentPlaceholder));
                StateManager.skipProblemOrDoubleDaily++;
                yield return new WaitUntil(() => EventManager.animate == false);
                break;
            }
            case Card.Permanent.OneTaskPerDay : {
                this.okButton.gameObject.SetActive(true);
                yield return new WaitUntil(() => EventManager.okPressed == true);
                EventManager.okPressed = false;
                this.okButton.gameObject.SetActive(false);
                permanentPlaceholder = permanentCard9.gameObject;
                StateManager.oneTaskPerDay = true;
                StartCoroutine(this.cardPicker.PlacePermanentCard(permanentPlaceholder));
                yield return new WaitUntil(() => EventManager.animate == false);
                break;
            }
            default :
                break;
        }
        EventManager.handlePermanentAction = false;
        yield break;
    }
    
    private void DiscardPermanentCard(UICard card){
        animationManager.DiscardCard(card.gameObject);
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
    IEnumerator MultiplieDebt(float n){
        yield return new WaitUntil(() => EventManager.action == true);
        animationManager.HideCardPick();
        yield return new WaitUntil(() => EventManager.animate == false);
        animationManager.UpdateDebtScrollBar(Mathf.RoundToInt(this.debtSlider.value * n));
        yield return new WaitUntil(() => EventManager.animate == false);
        EventManager.action = false;
    }
    IEnumerator CurrentPlayerPassATurn(int n){
        yield return new WaitUntil(() => EventManager.action == true);
        StateManager.currentPlayer.turnToPass = n;
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
        if (EventManager.firstCard == true){
            EventManager.firstCard = false;
            ReDeckUnflippedCards();
            DiscardCards();
            this.cardPicker.RemoveCards();
            yield return new WaitUntil(() => EventManager.cardToRemove <= 0);
            this.cardPicker.Reset();
        }
        EventManager.cardsToPick += n;
        StateManager.problemCards += n;
        StateManager.sprintProblemCards += n;
        // Card customPickedCard = this.problemCards[58];
        // this.cardPicker.AddCart(customPickedCard);
        // this.pickedCards.Add(customPickedCard);
        // this.remainingProblemCards.Remove(customPickedCard);
        // this.readyToDiscard.Add(customPickedCard.id + (((int) customPickedCard.category -1) * 60) -1, false);
        // animate updeck
        yield return new WaitUntil(() => EventManager.animate == false);
        for (int i = 0; i < n; i++){
            if (this.remainingProblemCards.Count < 1){
                this.remainingProblemCards.AddRange(this.discardedProblemCards);
                this.discardedProblemCards = new List<Card>();
            }
            int index = random.Next(0, this.remainingProblemCards.Count);
            Card choosenCard = this.remainingProblemCards[index];
            this.cardPicker.AddCart(choosenCard);
            yield return new WaitUntil(() => EventManager.animate == false);
            this.remainingProblemCards.Remove(choosenCard);
        }
        // animate downdeck
        yield return new WaitUntil(() => EventManager.animate == false);
        EventManager.action = false;
    }
    IEnumerator PickDailyCards(int n){
        yield return new WaitUntil(() => EventManager.action == true);
        if (EventManager.firstCard == true){
            EventManager.firstCard = false;
            ReDeckUnflippedCards();
            DiscardCards();
            this.cardPicker.RemoveCards();
            yield return new WaitUntil(() => EventManager.cardToRemove <= 0);
            this.cardPicker.Reset();
        }
        EventManager.cardsToPick += n;
        // animate updeck
        yield return new WaitUntil(() => EventManager.animate == false);
        // Card customPickedCard = this.dailyCards[21];
        // this.cardPicker.AddCart(customPickedCard);
        // this.remainingDailyCards.Remove(customPickedCard);
        for (int i = 0; i < n; i++){
            if (this.remainingDailyCards.Count < 1){
                this.remainingDailyCards.AddRange(this.discardedDailyCards);
                this.discardedDailyCards = new List<Card>();
            }
            int index = random.Next(0, this.remainingDailyCards.Count);
            Card choosenCard = this.remainingDailyCards[index];
            this.cardPicker.AddCart(choosenCard);
            yield return new WaitUntil(() => EventManager.animate == false);
            this.remainingDailyCards.Remove(choosenCard);
        }
        // animate downdeck
        yield return new WaitUntil(() => EventManager.animate == false);
        EventManager.action = false;
    }
    IEnumerator PickReviewCards(int n){
        yield return new WaitUntil(() => EventManager.action == true);
        if (EventManager.firstCard == true){
            EventManager.firstCard = false;
            ReDeckUnflippedCards();
            DiscardCards();
            this.cardPicker.RemoveCards();
            yield return new WaitUntil(() => EventManager.cardToRemove <= 0);
            this.cardPicker.Reset();
        }
        EventManager.cardsToPick += n;
        // animate updeck
        yield return new WaitUntil(() => EventManager.animate == false);
        for (int i = 0; i < n; i++){
            if (this.remainingReviewCards.Count < 1){
                this.remainingReviewCards.AddRange(this.discardedReviewCards);
                this.discardedReviewCards = new List<Card>();
            }
            int index = random.Next(0, this.remainingReviewCards.Count);
            Card choosenCard = this.remainingReviewCards[index];
            this.cardPicker.AddCart(choosenCard);
            yield return new WaitUntil(() => EventManager.animate == false);
            this.remainingReviewCards.Remove(choosenCard);
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
        // Card customPickedCard = this.dailyCards[32];
        // this.cardPicker.AddCart(customPickedCard);
        // this.remainingDailyCards.Remove(customPickedCard);
        for (int i = 0; i < 3; i++){
            if (this.remainingDailyCards.Count < 1){
                this.remainingDailyCards.AddRange(this.discardedDailyCards);
                this.discardedDailyCards = new List<Card>();
            }
            int index = random.Next(0, this.remainingDailyCards.Count);
            Card choosenCard = this.remainingDailyCards[index];
            this.cardPicker.AddCart(choosenCard);
            this.remainingDailyCards.Remove(choosenCard);
        }
        EventManager.handleCards = true;
        EventManager.firstCard = true;
        StartCoroutine(HandleCards());
        yield return new WaitUntil(() => EventManager.handleCards == false);
        StateManager.gameState = StateManager.GameState.PLAYER_TURN;
    }

    public IEnumerator FirstPickProblemCard(){
        yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.PROBLEM);
        EventManager.cardsToPick = 1;
        StateManager.problemCards++;
        StateManager.sprintProblemCards++;
        // Card customPickedCard = this.problemCards[51];
        // this.cardPicker.AddCart(customPickedCard);
        // this.remainingProblemCards.Remove(customPickedCard);
        // Card customPickedCard2 = this.problemCards[36];
        // this.cardPicker.AddCart(customPickedCard2);
        // this.remainingProblemCards.Remove(customPickedCard2);
        for (int i = 0; i < 3; i++){
            if (this.remainingProblemCards.Count <= 0){
                this.remainingProblemCards.AddRange(this.discardedProblemCards);
                this.discardedProblemCards = new List<Card>();
            }
            int index = random.Next(0, this.remainingProblemCards.Count);
            Card choosenCard = this.remainingProblemCards[index];
            this.cardPicker.AddCart(choosenCard);
            this.remainingProblemCards.Remove(choosenCard);
        }
        EventManager.handleCards = true;
        EventManager.firstCard = true;
        StartCoroutine(HandleCards());
        yield return new WaitUntil(() => EventManager.handleCards == false);
        StateManager.turnState = StateManager.TurnState.RESULT;
    }
    public IEnumerator FirstPickReviewCard(int n){
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.REVIEW_CARDS);
        if (n <= 0){
            StateManager.gameState = StateManager.GameState.SUMMARY;
            yield break;
        }
        // Card customPickedCard = this.reviewCards[10];
        // this.cardPicker.AddCart(customPickedCard);
        // this.remainingDailyCards.Remove(customPickedCard);
        EventManager.cardsToPick = n;
        for (int i = 0; i < n; i++){
            if (this.remainingReviewCards.Count < 1){
                this.remainingReviewCards.AddRange(this.discardedReviewCards);
                this.discardedReviewCards = new List<Card>();
            }
            int index = random.Next(0, this.remainingReviewCards.Count);
            Card choosenCard = this.remainingReviewCards[index];
            this.cardPicker.AddCart(choosenCard);
            this.remainingReviewCards.Remove(choosenCard);
        }
        EventManager.handleCards = true;
        EventManager.firstCard = false;
        StartCoroutine(HandleCards());
        yield return new WaitUntil(() => EventManager.handleCards == false);
        StateManager.gameState = StateManager.GameState.SUMMARY;
    }

    public void UpdateCurrentUS(){
        foreach (GameObject aus in gameManager.doingAUS){
            int newMax = aus.GetComponent<ArrowedUS>().userStory.maxTask - 3;
            int newCurrent = Mathf.Min(aus.GetComponent<ArrowedUS>().userStory.currentTask, newMax);
            Debug.Log($"Nouveau max et nouveau courant : {newMax}, {newCurrent}");
            aus.GetComponent<ArrowedUS>().userStory.currentTask = newCurrent;
            aus.GetComponent<ArrowedUS>().userStory.maxTask = newMax;
            aus.GetComponent<ArrowedUS>().UpdateColor(aus.GetComponent<ArrowedUS>().userStory.currentTask);
            if (newMax == newCurrent)
                aus.GetComponent<ArrowedUS>().userStory.state = UserStory.State.DONE;
        }
    }
    #endregion
    #endregion
    #region ###### Initialisation ######
    public void CreateDailyCards(){
        string path = "";
        if (StateManager.language == LocalizationSettings.AvailableLocales.GetLocale("fr")){
            path = Application.streamingAssetsPath + "/Cards/DailyCards_FR.json";
        } else {
            path = Application.streamingAssetsPath + "/Cards/DailyCards_EN.json";
        }
        string dailyCardsStr = File.ReadAllText(path);
        this.dailyCards = JsonConvert.DeserializeObject<List<Card>>(dailyCardsStr);
        this.remainingDailyCards.AddRange(this.dailyCards);
        this.discardedDailyCards = new List<Card>();
    }

    public void CreateProblemCards(){
        string path = "";
        if (StateManager.language == LocalizationSettings.AvailableLocales.GetLocale("fr")){
            path = Application.streamingAssetsPath + "/Cards/ProblemCards_FR.json";
        } else {
            path = Application.streamingAssetsPath + "/Cards/ProblemCards_EN.json";
        }
        string problemCardsStr = File.ReadAllText(path);
        this.problemCards = JsonConvert.DeserializeObject<List<Card>>(problemCardsStr);
        this.remainingProblemCards.AddRange(this.problemCards);
        this.discardedProblemCards = new List<Card>();
    }

    public void CreateReviewCards(){
        string path = "";
        if (StateManager.language == LocalizationSettings.AvailableLocales.GetLocale("fr")){
            path = Application.streamingAssetsPath + "/Cards/ReviewCards_FR.json";
        } else {
            path = Application.streamingAssetsPath + "/Cards/ReviewCards_EN.json";
        }
        string reviewCardsStr = File.ReadAllText(path);
        this.reviewCards = JsonConvert.DeserializeObject<List<Card>>(reviewCardsStr);
        this.remainingReviewCards.AddRange(this.reviewCards);
        this.discardedReviewCards = new List<Card>();
    }

    public void InitPermanentCard(){
        permanentCard1.Fill(dailyCards[1], cardPicker);
        permanentCard2.Fill(dailyCards[14], cardPicker);
        permanentCard3.Fill(dailyCards[21], cardPicker);
        permanentCard4.Fill(dailyCards[30], cardPicker);
        permanentCard5.Fill(dailyCards[31], cardPicker);
        permanentCard6.Fill(dailyCards[49], cardPicker);
        permanentCard7.Fill(problemCards[51], cardPicker);
        permanentCard8.Fill(dailyCards[19], cardPicker);
        permanentCard9.Fill(reviewCards[44], cardPicker);
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

    #region Permanent Animations
    public void ShowNoMoreTestIssuesPermanent(){
        StartCoroutine(ShowPermanentCardPopUp(permanentCard1));
    }
    public void ShowOneMoreTaskPerRollPermanent(){
        StartCoroutine(ShowPermanentCardWithoutPopUp(permanentCard2));
    }
    public void ShowTasksOnBeginSprintPermanent(){
        StartCoroutine(ShowPermanentCardWithoutPopUp(permanentCard3));
    }
    public void ShowTwoMoreTasksPerRollPermanent(){
        StartCoroutine(ShowPermanentCardWithoutPopUp(permanentCard4));
    }
    public void ShowMaxUserStoriesLoweredPermanent(){
        StartCoroutine(ShowPermanentCardWithoutPopUp(permanentCard5));
    }
    public void ShowDecreaseDebtPerTurnPermanent(){
        StartCoroutine(ShowPermanentCardWithoutPopUp(permanentCard6));
    }
    public void ShowJinxPermanent(){
        StartCoroutine(ShowPermanentCardPopUp(permanentCard7));
    }
    public void ShowSkipProblemOrDoubleDailyPermanent(){
        StartCoroutine(ShowPermanentCardPopUp(permanentCard8));
    }
    public void ShowOneTaskPerDayPermanent(){
        StartCoroutine(ShowPermanentCardWithoutPopUp(permanentCard9));
    }
    public IEnumerator ShowPermanentCardPopUp(UICard permanentCard){
        Transform initParent = permanentCard.transform.parent;
        int initIndex = permanentCard.transform.GetSiblingIndex();
        Vector3 initPos = permanentCard.transform.position;
        Vector3 initScale = permanentCard.transform.localScale;
        animationManager.ShowPermanentCardPopUp(permanentCard.gameObject);
        yield return new WaitUntil(() => EventManager.animate == false);
        EventManager.permanentCardShowned = true;
        yield return new WaitUntil(() => EventManager.readyToHidePermanent == true);
        EventManager.permanentCardShowned = false;
        EventManager.readyToHidePermanent = false;
        animationManager.HidePermanentCardPopUp(permanentCard.gameObject, initParent, initIndex, initPos, initScale);
        yield return new WaitUntil(() => EventManager.animate == false);
        EventManager.permanentCardHidden = true;
    }

    public IEnumerator ShowPermanentCardWithoutPopUp(UICard permanentCard){
        Transform initParent = permanentCard.transform.parent;
        int initIndex = permanentCard.transform.GetSiblingIndex();
        Vector3 initPos = permanentCard.transform.position;
        Vector3 initScale = permanentCard.transform.localScale;
        animationManager.ShowPermanentCardWithoutPopUp(permanentCard.gameObject);
        yield return new WaitUntil(() => EventManager.animate == false);
        EventManager.permanentCardShowned = true;
        yield return new WaitUntil(() => EventManager.readyToHidePermanent == true);
        EventManager.permanentCardShowned = false;
        EventManager.readyToHidePermanent = false;
        animationManager.HidePermanentCardWithoutPopUp(permanentCard.gameObject, initParent, initIndex, initPos, initScale);
        yield return new WaitUntil(() => EventManager.animate == false);
        EventManager.permanentCardHidden = true;
    }
    #endregion
}
