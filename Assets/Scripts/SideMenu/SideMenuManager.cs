using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class SideMenuManager : MonoBehaviour
{
    public bool scrumboardShowed = false;
    public bool burndownChartShowed = false;

    [SerializeField] AnimationManager animationManager;
    [SerializeField] TMP_Text timeOut;
    [SerializeField] TMP_Text starsOut;
    [SerializeField] TMP_Text sprintOut;
    [SerializeField] Button outsideButton;
    void Update(){
        TimeSpan interval = DateTime.Now - StateManager.startTime;
        this.timeOut.text = interval.ToString(@"hh\:mm\:ss");
        this.starsOut.text = StateManager.starsNumber.ToString();
        this.sprintOut.text = StateManager.sprintNumber.ToString();
    }

    public void ScrumboardClick(){
        Debug.Log("Clicked on scrumboard");
        if (scrumboardShowed == true || burndownChartShowed == true)
            return;
        scrumboardShowed = true;
        outsideButton.gameObject.SetActive(true);
        animationManager.ShowScrumboard();
        Debug.Log("Show scrumboard");
    }
    public void BurndownChartClick(){
        if (scrumboardShowed == true || burndownChartShowed == true)
            return;
        burndownChartShowed = true;
        outsideButton.gameObject.SetActive(true);
        animationManager.ShowBurndownChart();
        Debug.Log("Show burndownchart");
    }
    public void OnOutClick(){
        if (scrumboardShowed == false && burndownChartShowed == false)
            return;
        if (scrumboardShowed == true){
            Debug.Log("Hide scrumboard");
            animationManager.HideScrumboard();
            scrumboardShowed = false;
        } else if (burndownChartShowed == true){
            Debug.Log("Hide burndownchart");
            animationManager.HideBurndownChart();
            burndownChartShowed = false;
        }
        outsideButton.gameObject.SetActive(false);
    }
}
