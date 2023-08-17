using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitManager : MonoBehaviour
{
    [SerializeField] AnimationManager animationManager;
    [SerializeField] GameObject blurBack;
    [SerializeField] GameObject elementsBack;
    [SerializeField] GameObject exitPannel;

    private bool alreadyShowed = false;

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape) && !alreadyShowed){
            alreadyShowed = true;
            ShowExitPannel();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && alreadyShowed){
            alreadyShowed = false;
            HideExitPannel();
        }
    }
    public void OnYesClick(){
        Application.Quit();
    }
    public void OnNoClick(){
        HideExitPannel();
    }
    public void ShowExitPannel(){
        animationManager.ShowExitPannel(exitPannel, elementsBack, blurBack);
    }

    public void HideExitPannel(){
        animationManager.HideExitPannel(exitPannel, elementsBack, blurBack);
    }

    
}
