using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    #region Game State
    public static string difficulty;
    public static string userStory;
    public static string gameName;
    public static List<string> playerNames;
    public static bool pokerPlanning;
    public static GameState gameState;

    public enum GameState{
        MENU,
        INITIALISATION,
        POKER_PLANNING,
        PLAYER_TURN,

    }
    #endregion

    #region Turn State
    public enum TurnState{
        CHOICE,
        ROLL

    }
    public static Player currentPlayer;
    public static string taskOrDebt;
    public static int diceResult;
    public static TurnState turnState;

    public static void ClearTurnState(){
        currentPlayer = null;
        taskOrDebt = null;
        diceResult = 0;
        turnState = TurnState.CHOICE;
    }
    #endregion
}
