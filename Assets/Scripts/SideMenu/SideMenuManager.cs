using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class SideMenuManager : MonoBehaviour
{
    public bool scrumboardShowed = false;

    [SerializeField] AnimationManager animationManager;
    [SerializeField] TMP_Text timeOut;
    [SerializeField] TMP_Text starsOut;
    [SerializeField] TMP_Text sprintOut;
    [SerializeField] Button outsideButton;
    [SerializeField] GameObject sideMenuUI;
    [SerializeField] GameObject backUI;
    [SerializeField] GameObject outclick;
    [SerializeField] GameObject scrumBoard;
    [SerializeField] ScrumboardManager scrumboardManager;

    void Update(){
        TimeSpan interval = DateTime.Now - StateManager.startTime;
        this.timeOut.text = interval.ToString(@"hh\:mm\:ss");
        this.starsOut.text = StateManager.starsNumber.ToString();
        this.sprintOut.text = StateManager.sprintNumber.ToString();
    }

    public void ScrumboardClick(){
        Debug.Log("Clicked on scrumboard");
        scrumboardShowed = true;
        outsideButton.gameObject.SetActive(true);
        animationManager.ShowScrumboard(scrumBoard);
        Debug.Log("Show scrumboard");
    }
    public void OnOutClick(){
        if (!scrumboardShowed)
            return;
        else {
            Debug.Log("Hide scrumboard");
            animationManager.HideScrumboard(scrumBoard);
            scrumboardShowed = false;
        }
        outsideButton.gameObject.SetActive(false);
    }

    public void ShowSideMenu(){
        animationManager.ShowSideMenu(this.sideMenuUI, this.backUI, this.outclick);
        scrumboardManager.Refresh();
    }
    public void HideSideMenu(){
        animationManager.HideSideMenu(this.sideMenuUI, this.backUI, this.outclick);
    }
}
