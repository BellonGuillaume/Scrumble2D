using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PokerPlanningManager : MonoBehaviour
{
    [SerializeField] GameObject userStoryPrefab;
    [SerializeField] GameObject scrollPannel;
    [SerializeField] UserStoryUI leftUS;
    [SerializeField] UserStoryUI centralUS;
    [SerializeField] UserStoryUI rightUS;
    [SerializeField] GameObject preciseUI;
    // [SerializeField] TMP_Text xs, s, m, l, xl;
    List<GameObject> userStoriesUI;

    UserStory leftCurrent, centralCurrent, rightCurrent;

    void Start(){
        if (StateManager.userStories is null){
            InitState();
        }
        FillUserStoriesUI();
    }
    public void FillUserStoriesUI(){
        userStoriesUI = new List<GameObject>();
        for (int i = 0; i < StateManager.userStories.Count; i++){
            GameObject go = Instantiate(userStoryPrefab);
            go.GetComponent<UserStoryUI>().Fill(StateManager.userStories[i]);
            go.transform.SetParent(scrollPannel.transform);
            go.GetComponent<UserStoryUI>().Connect(this);
            userStoriesUI.Add(go);
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
        StateManager.difficulty = "EASY";
        StateManager.category = "GIFT SHOP";
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
    }

    public void OnSizeClick(string size){
        switch (size){
            case "XS":
                StateManager.userStories[this.centralCurrent.id-1].size = UserStory.Size.XS;
                this.centralUS.SetSize(UserStory.Size.XS);
                this.userStoriesUI[this.centralCurrent.id-1].GetComponent<UserStoryUI>().SetSize(UserStory.Size.XS);
                break;
            case "S":
                StateManager.userStories[this.centralCurrent.id-1].size = UserStory.Size.S;
                this.centralUS.SetSize(UserStory.Size.S);
                this.userStoriesUI[this.centralCurrent.id-1].GetComponent<UserStoryUI>().SetSize(UserStory.Size.S);
                break;
            case "M":
                StateManager.userStories[this.centralCurrent.id-1].size = UserStory.Size.M;
                this.centralUS.SetSize(UserStory.Size.M);
                this.userStoriesUI[this.centralCurrent.id-1].GetComponent<UserStoryUI>().SetSize(UserStory.Size.M);
                break;
            case "L":
                StateManager.userStories[this.centralCurrent.id-1].size = UserStory.Size.L;
                this.centralUS.SetSize(UserStory.Size.L);
                this.userStoriesUI[this.centralCurrent.id-1].GetComponent<UserStoryUI>().SetSize(UserStory.Size.L);
                break;
            case "XL":
                StateManager.userStories[this.centralCurrent.id-1].size = UserStory.Size.XL;
                this.centralUS.SetSize(UserStory.Size.XL);
                this.userStoriesUI[this.centralCurrent.id-1].GetComponent<UserStoryUI>().SetSize(UserStory.Size.XL);
                break;
        }
    }


}
