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

    List<Card> remainingDailyCards;
    List<Card> remainingProblemCards;
    List<Card> remainingReviewCards;

    List<Card> discardedDailyCards;
    List<Card> discardedProblemCards;
    List<Card> discardedReviewCards;

    [SerializeField] CardPicker cardPicker;
    [SerializeField] AnimationManager animationManager;
    [SerializeField] Slider debtSlider;

    private System.Random random;

    void Start(){
        this.random = new System.Random();
    }

    IEnumerator HandleCards(){
        EventManager.handleCards = true;
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
        switch (card.typeOfCard){
            case Card.TypeOfCard.Simple :
                EventManager.handleSimpleAction = true;
                StartCoroutine(HandleSimpleAction(card.firstAction, card.firstValue));
                yield return new WaitUntil(() => EventManager.handleSimpleAction == false);
                break;
            case Card.TypeOfCard.Multiple :
                EventManager.handleMultipleActions = true;
                StartCoroutine(HandleMultipleActions(card.firstAction, card.firstValue, card.secondAction, card.secondValue));
                yield return new WaitUntil(() => EventManager.handleMultipleActions == false);
                break;
            default :
                break;
        }
        if (EventManager.cardsToPick != 0){
            animationManager.ShowCardPick();
            yield return new WaitUntil(() => EventManager.animate == false);
        }
        EventManager.handleSingleCard = false;
        yield break;
    }

    #region ###### FirstCards #######
    public IEnumerator FirstPickDailyCard(){
        EventManager.cardsToPick = 1;
        for (int i = 0; i < 3; i++){
            if (this.dailyCards.Count < 1){
                this.dailyCards.AddRange(this.discardedDailyCards);
                this.discardedDailyCards = new List<Card>();
            }
            int index = random.Next(0, this.dailyCards.Count);
            Card choosenCard = this.dailyCards[index];
            this.cardPicker.AddCart(choosenCard);
            this.discardedDailyCards.Add(choosenCard);
            this.dailyCards.Remove(choosenCard);
        }
        StartCoroutine(HandleCards());
        yield return new WaitUntil(() => EventManager.handleCards == false);
        StateManager.gameState = StateManager.GameState.PLAYER_TURN;
    }

    public IEnumerator FirstPickProblemCard(){
        yield return new WaitUntil(() => StateManager.turnState == StateManager.TurnState.PROBLEM);
        EventManager.cardsToPick = 1;
        for (int i = 0; i < 3; i++){
            if (this.problemCards.Count <= 0){
                this.problemCards.AddRange(this.discardedProblemCards);
                this.discardedProblemCards = new List<Card>();
            }
            int index = random.Next(0, this.problemCards.Count);
            Card choosenCard = this.problemCards[index];
            this.cardPicker.AddCart(choosenCard);
            this.discardedProblemCards.Add(choosenCard);
            this.problemCards.Remove(choosenCard);
        }
        StartCoroutine(HandleCards());
        yield return new WaitUntil(() => EventManager.handleCards == false);
        StateManager.turnState = StateManager.TurnState.RESULT;
    }

    public IEnumerator FirstPickReviewCard(){
        EventManager.cardsToPick = 1;
        for (int i = 0; i < 3; i++){
            if (this.reviewCards.Count < 1){
                this.reviewCards.AddRange(this.discardedReviewCards);
                this.discardedReviewCards = new List<Card>();
            }
            int index = random.Next(0, this.reviewCards.Count);
            Card choosenCard = this.reviewCards[index];
            this.cardPicker.AddCart(choosenCard);
            this.discardedReviewCards.Add(choosenCard);
            this.reviewCards.Remove(choosenCard);
        }
        StartCoroutine(HandleCards());
        yield return new WaitUntil(() => EventManager.handleCards == false);
        StateManager.gameState = StateManager.GameState.RETROSPECTIVE;
    }
    #endregion
    #region ###### Cards functions ######
    #region - - - - - - - - - - - - - - - - - Handle CardType - - - - - - - - - - - - - - - - -
    IEnumerator HandleSimpleAction(Card.Action action, float value){
        EventManager.action = true;
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
            case Card.Action.CurrentPlayerPassATurn :
                StartCoroutine(CurrentPlayerPassATurn((int) value));
                break;
            case Card.Action.NextPlayerPassATurn :
                StartCoroutine(NextPlayerPassATurn((int) value));
                break;
            case Card.Action.AllPlayersPassATurn :
                StartCoroutine(AllPlayersPassATurn((int) value));
                break;
            case Card.Action.PickDailycards :
                StartCoroutine(PickDailycards((int) value));
                break;
            case Card.Action.PickProblemCards :
                StartCoroutine(PickProblemCards((int)value));
                break;
            case Card.Action.PickReviewCards :
                StartCoroutine(PickReviewCards((int) value));
                break;
            default :
                EventManager.action = false;
                break;
        }
        yield return new WaitUntil(() => EventManager.action == false);
        EventManager.handleSimpleAction = false;
    }
    IEnumerator HandleMultipleActions(Card.Action firstAction, float firstValue, Card.Action secondAction, float secondValue){
        EventManager.handleSimpleAction = true;
        StartCoroutine(HandleSimpleAction(firstAction, firstValue));
        yield return new WaitUntil(() => EventManager.handleSimpleAction == false);
        EventManager.handleSimpleAction = true;
        StartCoroutine(HandleSimpleAction(secondAction, secondValue));
        yield return new WaitUntil(() => EventManager.handleSimpleAction == false);
        EventManager.handleMultipleActions = false;
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
    IEnumerator PickDailycards(int n){
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
        this.discardedReviewCards = new List<Card>();
        foreach (Card card in this.reviewCards){
            Debug.Log(card.ToString());
        }
    }
    #endregion
}
