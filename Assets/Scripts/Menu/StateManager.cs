using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class StateManager : MonoBehaviour
{
    #region Menu
        public static Locale language;
        public enum Difficulty{
            EASY, NORMAL, HARD
        }
        public static Difficulty difficulty;
        public enum Category{
            GIFT_SHOP, DIET_COACH, TRAVEL_DIARY, CUSTOM
        }
        public static Category category;
    #endregion
    #region Game State
    public static List<UserStory> userStories;
    public static string gameName;
    public static List<Player> players;
    public static bool pokerPlanning;
    public static GameState gameState;

    public enum GameState{
        MENU, POKER_PLANNING, CUSTOM_POKER_PLANNING, INITIALISATION, BEGIN_GAME, TDTD, BEGIN_DAY, PICK_DAILY, PLAYER_TURN, END_OF_DAY, REVIEW, RETROSPECTIVE, END_OF_SPRINT,
    }
    #endregion
    #region Turn State
    public enum TurnState{
        BEGIN_TURN, CHOICE, ROLL, RESULT, PROBLEM, END_OF_TURN
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
        if (StateManager.language == LocalizationSettings.AvailableLocales.GetLocale("en")){
            if (userStory == StateManager.Category.GIFT_SHOP){
                path += "/UserStories/GiftShop_EN.json";
            } else if (userStory == StateManager.Category.DIET_COACH){
                path += "/UserStories/DietCoach_EN.json";
            } else if (userStory == StateManager.Category.TRAVEL_DIARY){
                path += "/UserStories/TravelDiary_EN.json";
            } else {
                path += "/UserStories/GiftShop_EN.json";
                // throw new System.Exception();
            }
        } else {
            if (userStory == StateManager.Category.GIFT_SHOP){
                path += "/UserStories/GiftShop_FR.json";
            } else if (userStory == StateManager.Category.DIET_COACH){
                path += "/UserStories/DietCoach_FR.json";
            } else if (userStory == StateManager.Category.TRAVEL_DIARY){
                path += "/UserStories/TravelDiary_FR.json";
            } else {
                path += "/UserStories/GiftShop_FR.json";
                // throw new System.Exception();
            }
        }
        string userStoriesStr = File.ReadAllText(path);
        StateManager.userStories = JsonConvert.DeserializeObject<List<UserStory>>(userStoriesStr);
    }
}
