using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject userStoryUIPrefab;
    List<UserStory> userStories;
    List<Card> dailyCards;
    List<Card> problemCards;
    List<Card> reviewCards;


    // Start is called before the first frame update
    void Start()
    {
        CreateUserStories(StateManager.userStory);
        // CreateDailyCards();
        CreateProblemCards();
        // CreateReviewCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateUserStories(string userStory){
        string path = Application.dataPath;
        if (StateManager.userStory == "GIFT SHOP"){
            path += "/UserStories/GIFT SHOP.json";
        } else if (StateManager.userStory == "DIET COACH"){
            path += "/UserStories/DIET COACH.json";
        } else if (StateManager.userStory == "TRAVEL DIARY"){
            path += "/UserStories/TRAVEL DIARY.json";
        } else {
            path += "/UserStories/GIFT SHOP.json";
            // throw new System.Exception();
        }
        string userStoriesStr = File.ReadAllText(path);
        this.userStories = JsonConvert.DeserializeObject<List<UserStory>>(userStoriesStr);

        Debug.Log("Values of the user stories :\n");
        for (int i = 0; i < userStories.Count; i++){
            Debug.Log(userStories[i].ToString());
        }
    }
    void CreateDailyCards(){
        string path = Application.dataPath + "/Cards/DailyCards.json";
        string dailyCardsStr = File.ReadAllText(path);
        this.dailyCards = JsonConvert.DeserializeObject<List<Card>>(dailyCardsStr);
    }

    void CreateProblemCards(){
        string path = Application.dataPath + "/Cards/ProblemCards.json";
        string problemCardsStr = File.ReadAllText(path);
        this.problemCards = JsonConvert.DeserializeObject<List<Card>>(problemCardsStr);

        Debug.Log("Here are the problem cards");
        for (int i = 0; i < problemCards.Count; i++){
            Debug.Log(problemCards[i].ToString());
        }
    }

    void CreateReviewCards(){
        string path = Application.dataPath + "/Cards/ReviewCards.json";
        string reviewCardsStr = File.ReadAllText(path);
        this.reviewCards = JsonConvert.DeserializeObject<List<Card>>(reviewCardsStr);
    }
}
