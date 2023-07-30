using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using TMPro;
using System;

public class EndScreenManager : MonoBehaviour
{
    [SerializeField] AnimationManager animationManager;
    [SerializeField] TMP_Text header;
    [SerializeField] TMP_Text difficulty;
    [SerializeField] TMP_Text userStoryUsed;
    [SerializeField] TMP_Text userStorySize;
    [SerializeField] TMP_Text gameDuration;
    [SerializeField] TMP_Text sprintNumber;
    [SerializeField] TMP_Text finalDebt;
    [SerializeField] TMP_Text problemCards;
    [SerializeField] TMP_Text totalTasks;
    [SerializeField] TMP_Text loosedTasks;
    [SerializeField] TMP_Text player1;
    [SerializeField] TMP_Text player2;
    [SerializeField] TMP_Text player3;
    [SerializeField] TMP_Text player4;
    [SerializeField] TMP_Text player5;
    [SerializeField] TMP_Text player6;
    [SerializeField] TMP_Text player7;
    [SerializeField] TMP_Text player8;
    [SerializeField] TMP_Text player9;

    private List<TMP_Text> players;
    private TimeSpan duration;

    public IEnumerator HandleEndGame(){
        StateManager.endTime = DateTime.Now;
        duration = StateManager.endTime - StateManager.startTime;
        players = new List<TMP_Text>();
        players.Add(player1);
        players.Add(player2);
        players.Add(player3);
        players.Add(player4);
        players.Add(player5);
        players.Add(player6);
        players.Add(player7);
        players.Add(player8);
        players.Add(player9);
        for (int i = 0; i < StateManager.players.Count; i++){
            players[i].text = StateManager.players[i].userName;
        }
        header.text = ComputeHeader();
        difficulty.text = ComputeDifficulty();
        userStoryUsed.text = ComputeUserStoryUsed();
        userStorySize.text = ComputeUserStorySize();
        gameDuration.text = duration.ToString(@"hh\:mm\:ss");
        sprintNumber.text = StateManager.sprintNumber.ToString();
        finalDebt.text = StateManager.currentDebt.ToString();
        problemCards.text = StateManager.problemCards.ToString();
        totalTasks.text = StateManager.totalTasks.ToString();
        loosedTasks.text = StateManager.loosedTasks.ToString();
        ShowFinalScreen();
        yield break;
    }

    private string ComputeHeader(){
        string headerTemp = GetString("HeaderFirst") + " ";
        int hours = duration.Hours;
        int minutes = duration.Minutes;
        int seconds = duration.Seconds;
        if (hours > 1)
            headerTemp += hours.ToString() + " " + GetString("Hours");
        else if (hours == 1)
            headerTemp += hours.ToString() + " " + GetString("Hour");
        if (hours > 0 && minutes > 0)
            headerTemp += ", ";
        if (minutes > 1)
            headerTemp += minutes.ToString() + " " + GetString("Minutes");
        else if (hours == 1)
            headerTemp += minutes.ToString() + " " + GetString("Minute");
        if (hours > 0 || minutes > 0)
            headerTemp += " " + GetString("And") + " ";
        if (seconds > 1)
            headerTemp += seconds.ToString() + " " + GetString("Seconds") + " ";
        else
            headerTemp += seconds.ToString() + " " + GetString("Second") + " ";
        headerTemp += GetString("HeaderSecond") + " ";
        if(StateManager.difficulty == StateManager.Difficulty.EASY)
            headerTemp += GetString("easy") + " ";
        else if(StateManager.difficulty == StateManager.Difficulty.NORMAL)
            headerTemp += GetString("normal") + " ";
        else
            headerTemp += GetString("hard") + " ";
        headerTemp += GetString("In") + " " + StateManager.sprintNumber.ToString() + " ";
        if (StateManager.sprintNumber > 1)
            headerTemp += GetString("Sprints") +".";
        else
            headerTemp += GetString("Sprint") + ".";
        return headerTemp;
    }
    private string ComputeDifficulty(){
        if (StateManager.difficulty == StateManager.Difficulty.EASY)
            return GetString("easy");
        else if (StateManager.difficulty == StateManager.Difficulty.NORMAL)
            return GetString("normal");
        else
            return GetString("hard");
    }
    private string ComputeUserStoryUsed(){
        if (StateManager.category == StateManager.Category.GIFT_SHOP)
            return GetString("GiftShop");
        else if (StateManager.category == StateManager.Category.DIET_COACH)
            return GetString("DietCoach");
        else if (StateManager.category == StateManager.Category.TRAVEL_DIARY)
            return GetString("TravelDiary");
        else if (StateManager.category == StateManager.Category.KNOWLEDGE_MANAGEMENT)
            return GetString("KnowledgeManagement");
        else
            return GetString("Custom");
    }

    private string ComputeUserStorySize(){
        if (StateManager.pokerPlanning == true)
            return GetString("Custom");
        else
            return GetString("Default");
    }
    public void OnScreenShotClick(){
        ScreenCapture.CaptureScreenshot("Scrumble Result.png");
    }
    public void OnDownloadClick(){

    }
    private void ShowFinalScreen(){
        this.gameObject.SetActive(true);
    }
    public string GetString(string stringKey){
        return LocalizationSettings.StringDatabase.GetLocalizedString("EndGame", stringKey);
    }
}
