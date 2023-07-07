using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CustomPokerPlanningManager : MonoBehaviour
{
    [SerializeField] GameObject userStoryPrefab;
    [SerializeField] GameObject scrollPannel;
    [SerializeField] CustomUserStoryUI leftUS;
    [SerializeField] CustomUserStoryUI centralUS;
    [SerializeField] CustomUserStoryUI rightUS;
    [SerializeField] GameObject preciseUI;
    [SerializeField] TMP_Text numberOfRemainingStars;
    [SerializeField] Button play;
    public List<GameObject> userStoriesUI;
    public List<UserStory> userStories;
    public List<bool> ready;

    UserStory leftCurrent, centralCurrent, rightCurrent;
    int remainingStars;

    void Start(){
        this.play.interactable = false;
        StateManager.gameState = StateManager.GameState.CUSTOM_POKER_PLANNING;
        StateManager.customPokerPlanningState = StateManager.CustomPokerPlanningState.GLOBAL;
        userStories = new List<UserStory>();
        userStoriesUI = new List<GameObject>();
        ready = new List<bool>();
        for (int i = 0; i < 15; i++){
            AddUserStoryUI();
            ready.Add(false);
        }
        remainingStars = 45;
    }

    void Update(){
        if(StateManager.customPokerPlanningState == StateManager.CustomPokerPlanningState.PRECISE){
            if (this.remainingStars + this.centralUS.userStory.stars <= 0){
                this.centralUS.BlockStars(5);
            } else if (this.remainingStars + this.centralUS.userStory.stars == 1){
                this.centralUS.BlockStars(4);
            } else if (this.remainingStars + this.centralUS.userStory.stars == 2){
                this.centralUS.BlockStars(3);
            } else if (this.remainingStars + this.centralUS.userStory.stars == 3){
                this.centralUS.BlockStars(2);
            } else if (this.remainingStars + this.centralUS.userStory.stars == 4){
                this.centralUS.BlockStars(1);
            } else {
                this.centralUS.FreeStars();
            }
        }
    }

    public override string ToString()
    {
        string temp = "Valeur des UserStories : ";
        for (int i = 0; i < this.userStoriesUI.Count; i++){
            temp += "--- User Story n°" + i.ToString() + "---\n" + this.userStoriesUI[i].GetComponent<UserStoryUI>().GetUserStory().ToString()+ "\n";
            // temp += "--- User Story n°" + i.ToString() + "---\n" + this.userStories[i].ToString()+ "\n";

        }
        return temp;
    }

    public void OnUserStoryClick(int id){
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

        ColorCustomUserStoryUI(this.leftUS);
        ColorCustomUserStoryUI(this.centralUS);
        ColorCustomUserStoryUI(this.rightUS);

        this.preciseUI.SetActive(true);
        StateManager.customPokerPlanningState = StateManager.CustomPokerPlanningState.PRECISE;
    }
    public void OnRightClick(){
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

        ColorCustomUserStoryUI(this.leftUS);
        ColorCustomUserStoryUI(this.centralUS);
        ColorCustomUserStoryUI(this.rightUS);
    }
    public void OnLeftClick(){
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

        ColorCustomUserStoryUI(this.leftUS);
        ColorCustomUserStoryUI(this.centralUS);
        ColorCustomUserStoryUI(this.rightUS);
    }
    public void OnBackClick(){
        if (AllReady() && this.remainingStars == 0) {
            this.play.interactable = true;
        } else {
            this.play.interactable = false;
        }
        StateManager.customPokerPlanningState = StateManager.CustomPokerPlanningState.GLOBAL;
        this.preciseUI.SetActive(false);
    }
    public void AddUserStoryUI(){
        UserStory userStory = new UserStory(this.userStories.Count+1, "CUSTOM", "En tant que\t,\nje veux ", 0, UserStory.Size.NOT_DEFINED, 0);
        this.userStories.Add(userStory);
        GameObject go = Instantiate(userStoryPrefab);
        go.transform.SetParent(this.scrollPannel.transform);
        go.GetComponent<UserStoryUI>().Connect(this);
        go.GetComponent<UserStoryUI>().Fill(userStory);
        go.GetComponent<UserStoryUI>().ChangeOutlineColor(UserStory.OutlineColor.RED);
        this.userStoriesUI.Add(go);
    }
    public void UpdateUserStoryUI(int id){
        if (id == this.centralCurrent.id){
            this.userStoriesUI[id-1].GetComponent<UserStoryUI>().Fill(this.userStories[id-1]);
            this.centralUS.Fill(this.centralCurrent);
            ColorUserStoryUI(this.userStoriesUI[id-1].GetComponent<UserStoryUI>());
            ColorCustomUserStoryUI(this.centralUS);
        }
    }
    public void UpdateStarsCount(){
        int tempStars = 0;
        for (int i = 0; i < this.userStories.Count; i++){
            tempStars += this.userStories[i].stars;
        }
        this.remainingStars = 45 - tempStars;
        this.numberOfRemainingStars.text = this.remainingStars.ToString();
    }
    public bool IsEmpty(UserStory userStory){
        if (userStory.asA == "" && userStory.iWant == "" && userStory.stars == 0 && userStory.size == UserStory.Size.NOT_DEFINED && userStory.restriction == 0){
            return true;
        }
        return false;
    }
    public bool IsNotFinished(UserStory userStory){
        int asa = Convert.ToInt32(userStory.asA == "");
        int iwant = Convert.ToInt32(userStory.iWant == "");
        int stars = Convert.ToInt32(userStory.stars == 0);
        int size = Convert.ToInt32(userStory.size == UserStory.Size.NOT_DEFINED);
        int sum = asa + iwant + stars + size;
        if (sum > 0 && sum < 4){
            return true;
        }
        if (sum == 4 && userStory.restriction != 0){
            return true;
        }
        return false;
    }
    public bool IsComplete(UserStory userStory){
        if (userStory.asA != "" && userStory.iWant != "" && userStory.stars != 0 && userStory.size != UserStory.Size.NOT_DEFINED){
            return true;
        }
        return false;
    }

    public void ColorUserStoryUI(UserStoryUI userStoryUI){
        if (IsEmpty(userStoryUI.userStory)){
            userStoryUI.GetComponent<UserStoryUI>().ChangeOutlineColor(UserStory.OutlineColor.RED);
            this.ready[userStoryUI.userStory.id-1] = false;
        }
        else if (IsNotFinished(userStoryUI.userStory)){
            userStoryUI.GetComponent<UserStoryUI>().ChangeOutlineColor(UserStory.OutlineColor.ORANGE);
            this.ready[userStoryUI.userStory.id-1] = false;
        }
        else if (IsComplete(userStoryUI.userStory)){
            userStoryUI.GetComponent<UserStoryUI>().ChangeOutlineColor(UserStory.OutlineColor.GREEN);
            this.ready[userStoryUI.userStory.id-1] = true;
        }
        else {
            userStoryUI.GetComponent<UserStoryUI>().ChangeOutlineColor(UserStory.OutlineColor.YELLOW);
            this.ready[userStoryUI.userStory.id-1] = false;
        }
    }
    public void ColorCustomUserStoryUI(CustomUserStoryUI customUserStoryUI){
        if (IsEmpty(customUserStoryUI.userStory)){
            customUserStoryUI.GetComponent<CustomUserStoryUI>().ChangeOutlineColor(UserStory.OutlineColor.RED);
        }
        else if (IsNotFinished(customUserStoryUI.userStory)){
            customUserStoryUI.GetComponent<CustomUserStoryUI>().ChangeOutlineColor(UserStory.OutlineColor.ORANGE);
        }
        else if (IsComplete(customUserStoryUI.userStory)){
            customUserStoryUI.GetComponent<CustomUserStoryUI>().ChangeOutlineColor(UserStory.OutlineColor.GREEN);
        }
        else {
            customUserStoryUI.GetComponent<CustomUserStoryUI>().ChangeOutlineColor(UserStory.OutlineColor.YELLOW);
        }
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
        StateManager.userStories = this.userStories;
        StateManager.gameState = StateManager.GameState.INITIALISATION;
        SceneManager.LoadSceneAsync("Game");
    }
}
