using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LocalNextButton : MonoBehaviour
{

    [SerializeField] Button nextButton;
    [SerializeField] TMP_InputField serverNameIn;
    [SerializeField] Slider pokerPlanningIn;
    [SerializeField] TMP_Dropdown difficultyIn;
    [SerializeField] TMP_Dropdown userStoryIn;

    static string serverName;
    static bool pokerPlanning;
    static int difficulty;
    static int userStory;

    public bool IsReady(){
        if (string.IsNullOrWhiteSpace(serverNameIn.text)) {
            return false;
        }
        if (userStoryIn.value < 0 | userStoryIn.value > 3){
            return false;
        }
        serverName = serverNameIn.text;
        pokerPlanning = (Math.Round(pokerPlanningIn.value) == 1) ? true : false;
        difficulty = difficultyIn.value;
        userStory = userStoryIn.value;
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsReady()){
            nextButton.interactable  = true;
        } else {
            nextButton.interactable  = false;
        }
    }

    public string GetServerName(){
        return serverName;
    }
    public bool GetPokerPlanning(){
        return pokerPlanning;
    }
    public int GetDifficulty(){
        return difficulty;
    }
    public int GetUserStory(){
        return userStory;
    }
}
