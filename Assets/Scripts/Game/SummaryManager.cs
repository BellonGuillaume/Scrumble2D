using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;
using System;
using UnityEngine.UI;
public class SummaryManager : MonoBehaviour
{
    [SerializeField] AnimationManager animationManager;

    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text durationSprint;
    [SerializeField] TMP_Text starsOld;
    [SerializeField] TMP_Text starsArrow;
    [SerializeField] TMP_Text starsNew;
    [SerializeField] TMP_Text debtOld;
    [SerializeField] TMP_Text debtArrow;
    [SerializeField] TMP_Text debtNew;
    [SerializeField] TMP_Text finishedUSOld;
    [SerializeField] TMP_Text finishedUSArrow;
    [SerializeField] TMP_Text finishedUSNew;
    [SerializeField] TMP_Text problemCardsSprint;
    [SerializeField] TMP_Text loosedTasksSprint;
    [SerializeField] Button nextButton;


    Color RED = new Color32(214, 71, 71, 255);      // #d64747
    Color GREEN = new Color32(122, 201, 67, 255);   // #7ac943

    public IEnumerator HandleSummary(){
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.SUMMARY);
        title.text = GetString("Title") + StateManager.sprintNumber.ToString();
        TimeSpan interval = DateTime.Now - StateManager.sprintBeginTime;
        this.durationSprint.text = interval.ToString(@"hh\:mm\:ss");
        this.starsOld.text = StateManager.sprintStars.ToString();
        this.debtOld.text = StateManager.sprintDebt.ToString();
        this.finishedUSOld.text = StateManager.sprintFinishedUS.ToString();
        this.problemCardsSprint.text = StateManager.sprintProblemCards.ToString();
        this.loosedTasksSprint.text = StateManager.sprintLoosedTasks.ToString();

        ShowSummary();
        yield return new WaitUntil(() => EventManager.animate == false);
        yield return new WaitForSeconds(1f);

        if (StateManager.sprintStars < StateManager.starsNumber){
            starsNew.text = StateManager.starsNumber.ToString();
            animationManager.ShowNewValue(starsArrow.gameObject, starsNew.gameObject);
        }

        if (StateManager.sprintDebt < StateManager.currentDebt){
            debtNew.text = StateManager.currentDebt.ToString();
            debtNew.color = RED;
            animationManager.ShowNewValue(debtArrow.gameObject, debtNew.gameObject);
        }
        else if (StateManager.sprintDebt > StateManager.currentDebt){
            debtNew.text = StateManager.currentDebt.ToString();
            debtNew.color = GREEN;
            animationManager.ShowNewValue(debtArrow.gameObject, debtNew.gameObject);
        }

        if (StateManager.sprintFinishedUS < StateManager.finishedUS){
            finishedUSNew.text = StateManager.finishedUS.ToString();
            animationManager.ShowNewValue(finishedUSArrow.gameObject, finishedUSNew.gameObject);
        }
        yield return new WaitForSeconds(1f);
        this.nextButton.gameObject.SetActive(true);
        yield break;
    }

    public void ShowSummary(){
        this.gameObject.SetActive(true);
    }

    public void OnNextClick(){
        this.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        starsArrow.gameObject.SetActive(false);
        debtArrow.gameObject.SetActive(false);
        finishedUSArrow.gameObject.SetActive(false);
        starsNew.gameObject.SetActive(false);
        debtNew.gameObject.SetActive(false);
        finishedUSNew.gameObject.SetActive(false);
        StateManager.gameState = StateManager.GameState.RETROSPECTIVE;
    }

    public string GetString(string stringKey){
        return LocalizationSettings.StringDatabase.GetLocalizedString("Summary", stringKey);
    }
}
