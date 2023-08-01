using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class FilledChoiceHandler : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] AnimationManager animationManager;
    [SerializeField] TMP_Text title;
    [SerializeField] Button debt;
    [SerializeField] Button moreUS;
    [SerializeField] Button endSprint;
    [SerializeField] GameObject popUp;
    [SerializeField] GameObject filledChoiceUI;
    public bool moreUSClicked = false;
    public bool debtClicked = false;
    public bool endSprintClicked = false;
    public IEnumerator HandleFilledChoice(){
        moreUSClicked = false;
        debtClicked = false;
        endSprintClicked = false;
        debt.interactable = true;
        moreUS.interactable = true;
        endSprint.interactable = true;
        bool noMoreTasks = true;
        foreach (UserStory userStory in StateManager.userStories){
            if (userStory.state == UserStory.State.PRODUCT_BACKLOG)
                noMoreTasks = false;
        }
        if (noMoreTasks){
            title.text = GetString("EndSprintOrDebt");
            moreUS.gameObject.SetActive(false);
            endSprint.gameObject.SetActive(true);
        } else {
            title.text = GetString("AddUSOrDebt");
            moreUS.gameObject.SetActive(true);
            endSprint.gameObject.SetActive(false);
        }
        animationManager.ShowFilledChoice(filledChoiceUI);
        yield return new WaitUntil(() => EventManager.animate == false);
        yield return new WaitUntil(() => moreUSClicked || debtClicked || endSprintClicked);
        debt.interactable = false;
        moreUS.interactable = false;
        endSprint.interactable = false;
        if(moreUSClicked){
            animationManager.HideFilledChoice(filledChoiceUI);
            yield return new WaitUntil(() => EventManager.animate == false);
            StateManager.gameState = StateManager.GameState.TDTD;
            StartCoroutine(gameManager.ChooseToDoToDoing());
            yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.BEGIN_DAY);
        }
        else if (endSprintClicked){
            animationManager.HideFilledChoice(filledChoiceUI);
            yield return new WaitUntil(() => EventManager.animate == false);
        }
        else {
            animationManager.HideFilledChoice(filledChoiceUI);
            yield return new WaitUntil(() => EventManager.animate == false);
            EventManager.onlyDebt = true;
        }
        EventManager.allFilledChoiceMade = true;
    }

    public string GetString(string stringKey){
        return LocalizationSettings.StringDatabase.GetLocalizedString("Game", stringKey);
    }

    public void OnMoreUSClick(){
        moreUSClicked = true;
    }
    public void OnEndSprintClick(){
        endSprintClicked = true;
    }
    public void OnDebtClick(){
        debtClicked = true;
    }
}
