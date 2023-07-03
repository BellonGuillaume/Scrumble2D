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
        ROLL,
        RESULT,
        PROBLEM,
        END_OF_TURN

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
}
