using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class Local : MonoBehaviour
{

    [SerializeField] Button nextButton;
    [SerializeField] TMP_InputField serverNameIn;
    [SerializeField] Slider pokerPlanningIn;
    [SerializeField] TMP_Dropdown difficultyIn;
    [SerializeField] LocalizedString[] difficultyOptions;
    [SerializeField] TMP_Dropdown userStoryIn;
    [SerializeField] LocalizedString[] userStoryOptions;


    static string serverName;
    static bool pokerPlanning;
    static StateManager.Difficulty difficulty;
    static StateManager.Category userStory;

    public bool IsReady(){
        if (string.IsNullOrWhiteSpace(serverNameIn.text)) {
            return false;
        }
        if (userStoryIn.value < 0 | userStoryIn.value > 3){
            return false;
        }
        return true;
    }

    void Update()
    {
        if (IsReady()){
            serverName = serverNameIn.text;
            pokerPlanning = (Math.Round(pokerPlanningIn.value) == 1) ? true : false;
            difficulty = (StateManager.Difficulty)difficultyIn.value;
            userStory = (StateManager.Category)userStoryIn.value;
            nextButton.interactable  = true;
        } else {
            nextButton.interactable  = false;
        }
        if (StateManager.language != LocalizationSettings.SelectedLocale){
            RefreshDropDown(difficultyIn, difficultyOptions);
            RefreshDropDown(userStoryIn, userStoryOptions);
            StateManager.language = LocalizationSettings.SelectedLocale;
        }
    }

    public string GetServerName(){
        return serverName;
    }
    public bool GetPokerPlanning(){
        return pokerPlanning;
    }
    public StateManager.Difficulty GetDifficulty(){
        return difficulty;
    }
    public StateManager.Category GetUserStory(){
        return userStory;
    }
    public void RefreshDropDown(TMP_Dropdown dropdown, LocalizedString[] options){
        dropdown.ClearOptions();
        var localizedOptions = new List<string>();
        foreach (var option in options){
            string localizedText = option.GetLocalizedString();
            localizedOptions.Add(localizedText);
        }
        dropdown.AddOptions(localizedOptions);
    }
}
