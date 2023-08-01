using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System;

public class StateManager : MonoBehaviour
{
    #region Menu
    public static Locale language;
    public enum Difficulty{
        EASY, NORMAL, HARD
    }
    public static Difficulty difficulty;
    public enum Category{
        GIFT_SHOP, DIET_COACH, TRAVEL_DIARY, KNOWLEDGE_MANAGEMENT, CUSTOM
    }
    public static Category category;
    public static bool pokerPlanning;
    public static string gameName;
    #endregion
    #region Game State
    public static List<UserStory> userStories;
    public static List<Player> players;
    public static GameState gameState;
    public static DateTime startTime;
    public static DateTime endTime;
    public static int sprintNumber = 0;
    public static int starsNumber = 0;
    public static int finishedUS = 0;
    public static int problemCards = 0;
    public static int totalTasks = 0;
    public static int loosedTasks = 0;
    public enum GameState{
        MENU, POKER_PLANNING, CUSTOM_POKER_PLANNING, INITIALISATION, BEGIN_GAME, TDTD, BEGIN_DAY, PICK_DAILY, PLAYER_TURN, WANT_TO_PASS, END_OF_DAY, REVIEW, REVIEW_CARDS, SUMMARY, RETROSPECTIVE, END_OF_SPRINT, END_OF_GAME
    }
    
    public static int currentDebt;
    public static int debtFactor;
    public static bool jinxed = false;
    public static bool noMoreTestIssues = false;
    public static bool maxUserStoryLowered = false;
    public static bool oneMoreTaskPerRoll = false;
    public static bool tasksOnBeginSprint = false;
    public static bool oneTaskPerDay = false;
    public static int skipProblemOrDoubleDaily = 0;
    #endregion
    #region Sprint State
    public static DateTime sprintBeginTime;
    public static int sprintStars = 0;
    public static int sprintDebt = 0;
    public static int sprintFinishedUS = 0;
    public static int sprintProblemCards = 0;
    public static int sprintLoosedTasks = 0;

    #endregion
    #region Turn State
    public enum TurnState{
        BEGIN_TURN, CHOICE, ROLL, RESULT, PROBLEM, NEW_US_ADDED, WANT_TO_PASS, END_OF_TURN,
    }
    public static Player currentPlayer;
    public static string firstTaskOrDebtChoice;
    public static int firstDiceResult;
    public static bool alreadyReRoll;
    public static string secondTaskOrDebtChoice;
    public static int secondDiceResult;
    public static string optionalResult;
    public static TurnState turnState;

    public static void ClearTurnState(){
        currentPlayer = null;
        firstTaskOrDebtChoice = null;
        firstDiceResult = 0;
        alreadyReRoll = false;
        secondDiceResult = 0;
        secondTaskOrDebtChoice = null;
        optionalResult = null;
    }
    #endregion
    #region Planning Poker State
    
    public enum PokerPlanningState{
        GLOBAL, PRECISE, FINISHED
    }
    public static PokerPlanningState pokerPlanningState;
    #endregion
    #region Custom Planning Poker State
    public enum CustomPokerPlanningState
    {
        GLOBAL, PRECISE, FINISHED
    }
    public static CustomPokerPlanningState customPokerPlanningState;
    #endregion
    #region Day State
    public static int currentDay = 0;
    #endregion
    public static void CreatePlayers(List<string> usernames){
        players = new List<Player>();
        for (int i = 0; i < usernames.Count; i++){
            if(i + 1 == usernames.Count){
                players.Add(new Player(usernames[i], i+1, 1));
            } else {
                players.Add(new Player(usernames[i], i+1, i+2));
            }
        }
    }
    public static void CreateUserStories(StateManager.Category userStory){
        string path = Application.streamingAssetsPath;
        if (StateManager.language == LocalizationSettings.AvailableLocales.GetLocale("fr")){
            if (userStory == StateManager.Category.GIFT_SHOP){
                path += "/UserStories/GiftShop_FR.json";
            } else if (userStory == StateManager.Category.DIET_COACH){
                path += "/UserStories/DietCoach_FR.json";
            } else if (userStory == StateManager.Category.TRAVEL_DIARY){
                path += "/UserStories/TravelDiary_FR.json";
            } else if (userStory == StateManager.Category.KNOWLEDGE_MANAGEMENT){
                path += "/UserStories/KnowledgeManagement_FR.json";
            } else {
                path += "/UserStories/GiftShop_FR.json";
                // throw new System.Exception();
            }
        } else {
            if (userStory == StateManager.Category.GIFT_SHOP){
                path += "/UserStories/GiftShop_EN.json";
            } else if (userStory == StateManager.Category.DIET_COACH){
                path += "/UserStories/DietCoach_EN.json";
            } else if (userStory == StateManager.Category.TRAVEL_DIARY){
                path += "/UserStories/TravelDiary_EN.json";
            } else if (userStory == StateManager.Category.KNOWLEDGE_MANAGEMENT){
                path += "/UserStories/KnowledgeManagement_EN.json";
            } else {
                path += "/UserStories/GiftShop_EN.json";
                // throw new System.Exception();
            }
        }
        string userStoriesStr = File.ReadAllText(path);
        StateManager.userStories = JsonConvert.DeserializeObject<List<UserStory>>(userStoriesStr);
    }
    public static void UpdateDebt(float value){
        if (value >= 40)
            debtFactor = 12;
        else if(value >= 30)
            debtFactor = 9;
        else if(value >= 20)
            debtFactor = 6;
        else if(value >= 10)
            debtFactor = 4;
        else
            debtFactor = 3;
        currentDebt = Mathf.Max(0, Mathf.FloorToInt(value));
    }
}
