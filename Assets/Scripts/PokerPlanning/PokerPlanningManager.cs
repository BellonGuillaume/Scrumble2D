using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerPlanningManager : MonoBehaviour
{
    [SerializeField] GameObject userStoryPrefab;
    [SerializeField] GameObject scrollPannel;
    List<GameObject> userStoriesUI;

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

    public static void OnUserStoryClick(int id){
        Debug.Log($"Clicked on UserStory {id}");
    }
}
