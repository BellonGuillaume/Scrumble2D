using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class StateManager : MonoBehaviour
{
    #region Game State
    public static string difficulty;
    public static string category;
    public static List<UserStory> userStories;
    public static string gameName;
    public static List<Player> players;
    public static bool pokerPlanning;
    public static GameState gameState;

    public enum GameState{
        MENU, INITIALISATION, POKER_PLANNING, PLAYER_TURN,
    }
    #endregion

    #region Turn State
    public enum TurnState{
        CHOICE, ROLL, RESULT, PROBLEM, END_OF_TURN
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
        turnState = TurnState.CHOICE;
    }
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
    public static void CreateUserStories(string userStory){
        string path = Application.dataPath;
        if (StateManager.category == "GIFT SHOP"){
            path += "/UserStories/GIFT SHOP.json";
        } else if (StateManager.category == "DIET COACH"){
            path += "/UserStories/DIET COACH.json";
        } else if (StateManager.category == "TRAVEL DIARY"){
            path += "/UserStories/TRAVEL DIARY.json";
        } else {
            path += "/UserStories/GIFT SHOP.json";
            // throw new System.Exception();
        }
        string userStoriesStr = File.ReadAllText(path);
        StateManager.userStories = JsonConvert.DeserializeObject<List<UserStory>>(userStoriesStr);
    }
}
