using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomPokerPlanningManager : MonoBehaviour
{
    [SerializeField] GameObject userStoryPrefab;
    [SerializeField] GameObject scrollPannel;
    [SerializeField] CustomUserStoryUI leftUS;
    [SerializeField] CustomUserStoryUI centralUS;
    [SerializeField] CustomUserStoryUI rightUS;
    [SerializeField] GameObject preciseUI;
    public List<GameObject> userStoriesUI;
    public List<UserStory> userStories;

    UserStory leftCurrent, centralCurrent, rightCurrent;

    void Start(){
        StateManager.gameState = StateManager.GameState.CUSTOM_POKER_PLANNING;
        StateManager.customPokerPlanningState = StateManager.CustomPokerPlanningState.GLOBAL;
        userStories = new List<UserStory>();
        userStoriesUI = new List<GameObject>();
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

    public void OnUserStoryClick(int id){
        Debug.Log($"CLicked on id : {id}");
        if (id == 1) {
            this.leftCurrent =  this.userStories[this.userStories.Count-1];
        } else {
            this.leftCurrent =  this.userStories[id-2];
        }
        if (id == this.userStories.Count){
            this.rightCurrent =  this.userStories[0];
        } else {
            this.rightCurrent =  this.userStories[id];
        }
        this.centralCurrent = this.userStories[id-1];
        this.leftUS.Fill(this.leftCurrent);
        this.centralUS.Fill(this.centralCurrent);
        this.rightUS.Fill(this.rightCurrent);

        StateManager.customPokerPlanningState = StateManager.CustomPokerPlanningState.PRECISE;
        this.preciseUI.SetActive(true);        
    }
    public void OnRightClick(){
        Debug.Log("Clicked right");
        this.leftCurrent = this.centralCurrent;
        this.centralCurrent = this.rightCurrent;
        if(this.rightCurrent.id == this.userStories.Count){
            this.rightCurrent = this.userStories[0];
        } else {
            this.rightCurrent = this.userStories[this.rightCurrent.id];
        }
        this.leftUS.Fill(this.leftCurrent);
        this.centralUS.Fill(this.centralCurrent);
        this.rightUS.Fill(this.rightCurrent);
    }
    public void OnLeftClick(){
        Debug.Log("Clicked left");
        this.rightCurrent = this.centralCurrent;
        this.centralCurrent = this.leftCurrent;
        if(this.leftCurrent.id == 1){
            this.leftCurrent = this.userStories[this.userStories.Count - 1];
        } else {
            this.leftCurrent = this.userStories[this.leftCurrent.id - 2];
        }
        this.leftUS.Fill(this.leftCurrent);
        this.centralUS.Fill(this.centralCurrent);
        this.rightUS.Fill(this.rightCurrent);
    }
    public void OnBackClick(){
        StateManager.customPokerPlanningState = StateManager.CustomPokerPlanningState.GLOBAL;
        this.preciseUI.SetActive(false);
    }
    public void AddUserStoryUI(){
        UserStory userStory = new UserStory(this.userStories.Count+1, "CUSTOM", "", 0, UserStory.Size.NOT_DEFINED, 0);
        this.userStories.Add(userStory);
        GameObject go = Instantiate(userStoryPrefab);
        go.transform.SetParent(this.scrollPannel.transform);
        go.GetComponent<UserStoryUI>().Connect(this);
        go.GetComponent<UserStoryUI>().Fill(userStory);
        this.userStoriesUI.Add(go);
        for (int i = 0; i < this.userStories.Count; i++){
            Debug.Log(this.userStories[i].ToString());
        }
    }
    public void UpdateUserStoryUI(int id){
        this.userStoriesUI[id-1].GetComponent<UserStoryUI>().Fill(this.userStories[id-1]);
    }
}
