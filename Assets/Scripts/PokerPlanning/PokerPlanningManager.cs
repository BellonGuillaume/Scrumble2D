using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;

public class PokerPlanningManager : MonoBehaviour
{
    [SerializeField] GameObject userStoryPrefab;
    [SerializeField] GameObject scrollPannel;
    [SerializeField] UserStoryUI leftUS;
    [SerializeField] UserStoryUI centralUS;
    [SerializeField] UserStoryUI rightUS;
    [SerializeField] GameObject preciseUI;
    [SerializeField] TMP_Text userStoryTitle;
    [SerializeField] LocalizedString[] userStoryOptions;
    [SerializeField] Button play;
    List<GameObject> userStoriesUI;
    List<bool> ready;

    UserStory leftCurrent, centralCurrent, rightCurrent;

    void Start(){
        this.play.interactable = false;
        if (StateManager.userStories is null){
            InitState();
        }
        this.userStoryTitle.text = CatToString(StateManager.category);
        StateManager.pokerPlanningState = StateManager.PokerPlanningState.GLOBAL;
        FillUserStoriesUI();
    }
    void Update(){
        if(StateManager.pokerPlanningState == StateManager.PokerPlanningState.PRECISE){
            if (Input.GetKeyDown (KeyCode.LeftArrow)) {
                OnLeftClick();
            }
            if (Input.GetKeyDown (KeyCode.RightArrow)) {
                OnRightClick();
            }
        }
    }
    public void FillUserStoriesUI(){
        userStoriesUI = new List<GameObject>();
        ready = new List<bool>();
        for (int i = 0; i < StateManager.userStories.Count; i++){
            GameObject go = Instantiate(userStoryPrefab);
            go.GetComponent<UserStoryUI>().Fill(StateManager.userStories[i]);
            go.transform.SetParent(scrollPannel.transform);
            go.GetComponent<UserStoryUI>().Connect(this);
            go.GetComponent<UserStoryUI>().ChangeOutlineColor(UserStory.OutlineColor.RED);
            userStoriesUI.Add(go);
            ready.Add(false);
        }
    }

    public override string ToString()
    {
        string temp = "Valeur des UserStories : ";
        for (int i = 0; i < this.userStoriesUI.Count; i++){
            temp += this.userStoriesUI[i].GetComponent<UserStoryUI>().GetUserStory().ToString();
        }
        return temp;
    }

    void InitState(){
        // StateManager.difficulty = StateManager.difficulty.EASY; TODO
        StateManager.difficulty = StateManager.Difficulty.EASY;
        StateManager.category = StateManager.Category.GIFT_SHOP;
        StateManager.gameName = "";
        StateManager.pokerPlanning = false;
        StateManager.CreateUserStories(StateManager.category);
        StateManager.CreatePlayers(new List<string>{"Alice", "Bob", "Charles"});

        StateManager.gameState = StateManager.GameState.POKER_PLANNING;
        StateManager.pokerPlanningState = StateManager.PokerPlanningState.GLOBAL;
        Debug.Log("-STATE_MANAGER INITIALIZED");
    }

    public void OnUserStoryClick(int id){
        if (id == 1) {
            this.leftCurrent =  StateManager.userStories[StateManager.userStories.Count-1];
        } else {
            this.leftCurrent =  StateManager.userStories[id-2];
        }
        if (id == StateManager.userStories.Count){
            this.rightCurrent =  StateManager.userStories[0];
        } else {
            this.rightCurrent =  StateManager.userStories[id];
        }
        this.centralCurrent = StateManager.userStories[id-1];
        this.leftUS.Fill(this.leftCurrent);
        this.centralUS.Fill(this.centralCurrent);
        this.rightUS.Fill(this.rightCurrent);
        ColorUserStoryUI(this.leftUS);
        ColorUserStoryUI(this.centralUS);
        ColorUserStoryUI(this.rightUS);

        StateManager.pokerPlanningState = StateManager.PokerPlanningState.PRECISE;
        this.preciseUI.SetActive(true);        
    }

    public void OnRightClick(){
        this.leftCurrent = this.centralCurrent;
        this.centralCurrent = this.rightCurrent;
        if(this.rightCurrent.id == StateManager.userStories.Count){
            this.rightCurrent = StateManager.userStories[0];
        } else {
            this.rightCurrent = StateManager.userStories[this.rightCurrent.id];
        }
        this.leftUS.Fill(this.leftCurrent);
        this.centralUS.Fill(this.centralCurrent);
        this.rightUS.Fill(this.rightCurrent);
        ColorUserStoryUI(this.leftUS);
        ColorUserStoryUI(this.centralUS);
        ColorUserStoryUI(this.rightUS);
    }
    public void OnLeftClick(){
        this.rightCurrent = this.centralCurrent;
        this.centralCurrent = this.leftCurrent;
        if(this.leftCurrent.id == 1){
            this.leftCurrent = StateManager.userStories[StateManager.userStories.Count - 1];
        } else {
            this.leftCurrent = StateManager.userStories[this.leftCurrent.id - 2];
        }
        this.leftUS.Fill(this.leftCurrent);
        this.centralUS.Fill(this.centralCurrent);
        this.rightUS.Fill(this.rightCurrent);
        ColorUserStoryUI(this.leftUS);
        ColorUserStoryUI(this.centralUS);
        ColorUserStoryUI(this.rightUS);
    }

    public void OnSizeClick(string size){
        UserStory.Size sizeUS = UserStory.Size.XL;
        switch (size){
            case "XS":
                sizeUS = UserStory.Size.XS;
                break;
            case "S":
                sizeUS = UserStory.Size.S;
                break;
            case "M":
                sizeUS = UserStory.Size.M;
                break;
            case "L":
                sizeUS = UserStory.Size.L;
                break;
            case "XL":
                sizeUS = UserStory.Size.XL;
                break;
        }
            StateManager.userStories[this.centralCurrent.id-1].size = sizeUS;
            this.centralUS.SetSize(sizeUS);
            this.userStoriesUI[this.centralCurrent.id-1].GetComponent<UserStoryUI>().SetSize(sizeUS);
            ColorUserStoryUI(this.centralUS);
            ColorUserStoryUI(this.userStoriesUI[this.centralCurrent.id-1].GetComponent<UserStoryUI>());
            this.ready[this.centralCurrent.id-1] = true;
    }

    public void OnBackClick(){
        if (AllReady()){
            this.play.interactable = true;
        } else {
            this.play.interactable = false;
        }
        StateManager.pokerPlanningState = StateManager.PokerPlanningState.GLOBAL;
        this.preciseUI.SetActive(false);
    }

    public void ColorUserStoryUI(UserStoryUI userStoryUI){
        if (IsComplete(userStoryUI.userStory)){
            userStoryUI.GetComponent<UserStoryUI>().ChangeOutlineColor(UserStory.OutlineColor.GREEN);
        } else {
            userStoryUI.GetComponent<UserStoryUI>().ChangeOutlineColor(UserStory.OutlineColor.RED);
        }
    }
    public bool IsComplete(UserStory userStory){
        if (userStory.size != UserStory.Size.NOT_DEFINED){
            return true;
        }
        return false;
    }
    public bool AllReady(){
        for (int i = 0; i < this.ready.Count; i++){
            if (!this.ready[i]){
                return false;
            }
        }
        return true;
    }
    public void Play(){
        StateManager.pokerPlanningState = StateManager.PokerPlanningState.FINISHED;
        StateManager.gameState = StateManager.GameState.INITIALISATION;
        SceneManager.LoadSceneAsync("Game");
    }
    public string CatToString(StateManager.Category category){
        switch(category){
            case StateManager.Category.GIFT_SHOP:
                return userStoryOptions[0].GetLocalizedString();
            case StateManager.Category.DIET_COACH:
                return userStoryOptions[1].GetLocalizedString();
            case StateManager.Category.TRAVEL_DIARY:
                return userStoryOptions[2].GetLocalizedString();
            case StateManager.Category.CUSTOM:
                return userStoryOptions[3].GetLocalizedString();
            default:
                return userStoryOptions[0].GetLocalizedString();
        }
    }

}
