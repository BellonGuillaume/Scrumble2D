using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RetrospectiveManager : MonoBehaviour
{
    [SerializeField] AnimationManager animationManager;
    [SerializeField] GameObject retrospectiveUI;
    [SerializeField] GameObject blurBackUI;
    [SerializeField] GameObject backgroundUI;
    [SerializeField] Button passButton;
    [SerializeField] TMP_Text countDown;

    DateTime startTime;
    TimeSpan initTimer = new TimeSpan(0,2,0);
    bool endOfTimer = false;

    public IEnumerator HandleRetrospective(){
        animationManager.ShowRetrospective(retrospectiveUI, backgroundUI, blurBackUI);
        yield return new WaitUntil(() => EventManager.animate == false);
        endOfTimer = false;
        startTime = DateTime.Now;
        while(!endOfTimer){
            TimeSpan elapsedTime = DateTime.Now - startTime;
            TimeSpan remainingTime = initTimer.Subtract(elapsedTime);
            countDown.text = remainingTime.ToString(@"mm\:ss");
            if (remainingTime.Minutes <= 0 && remainingTime.Seconds <= 0 && remainingTime.Milliseconds <= 0)
                endOfTimer = true;
            yield return null;
        }
        countDown.text = "00:00";
        passButton.enabled = false;
        yield return new WaitForSeconds(0.5f);
        animationManager.HideRetrospective(retrospectiveUI, backgroundUI, blurBackUI);
        yield return new WaitUntil(() => EventManager.animate == false);
        countDown.text = "02:00";
        passButton.enabled = true;
        StateManager.gameState = StateManager.GameState.END_OF_SPRINT;
    }

    public void OnPassClick(){
        this.endOfTimer = true;
    }
}
