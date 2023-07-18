using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static bool animate = false;
    public static bool handleCards = false;
    public static bool handleSingleCard = false;
    public static bool handleSimpleAction = false;
    public static bool handleMultipleActions = false;
    public static bool handleQuestionActions = false;
    public static bool handleRollTheDiceAction = false;
    public static bool handleRollTheHalfDiceAction = false;
    public static bool handlePermanentAction = false;
    public static bool handleChoiceActions = false;
    public static bool handleInformationAction = false;
    public static bool handlePropositionAction = false;
    public static bool action = false;
    public static int cardsToPick = 0;
    public static bool handleAddingTask = false;
    public static bool taskAdded = true;
    public static int taskToAdd = 0;
    public static int cardToRemove = 0;
}
