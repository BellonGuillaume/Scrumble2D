using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class AnimationManager : MonoBehaviour
{
    public Animator animator;
    
    [SerializeField] Image coverImage;

    [SerializeField] GameObject popUp;
    [SerializeField] Vector2 popUpScale;
    [SerializeField] GameObject backgroundPopUp;
    [SerializeField] GameObject windowPopUp;
    [SerializeField] GameObject choicePopUp;
    [SerializeField] GameObject rollPopUp;
    [SerializeField] GameObject resultPopUp;
    [SerializeField] GameObject tdddPopUp;
    [SerializeField] GameObject cardPick;

    [SerializeField] GameObject sideMenu;
    [SerializeField] GameObject backgroundSideMenu;
    [SerializeField] GameObject windowSideMenu;

    [SerializeField] GameObject dayPopUp;
    [SerializeField] GameObject textDayPopUp;

    [SerializeField] Slider debtSlider;
    [SerializeField] Slider daySlider;

    [SerializeField] Vector2 sideMenuShowPos;
    [SerializeField] Vector2 sideMenuHidePos;
    [SerializeField] byte blurValue;

    [SerializeField] float popUpInDuration;

    [SerializeField] float dayAnimationDuration;
    [SerializeField] Vector2 dayAnimationStartPosition;
    [SerializeField] Vector2 dayAnimationEndPosition;

    [SerializeField] float startGameScreenDuration;

    private Coroutine animationCoroutine;

    public void StartDayAnimation(int n){
        EventManager.animate = true;
        string text = LocalizationSettings.StringDatabase.GetTable("Game").GetEntry("Day").GetLocalizedString();
        this.textDayPopUp.GetComponent<TMP_Text>().text = text + " " + n.ToString();
        this.dayPopUp.SetActive(true);
        Vector2 startPosition = this.dayAnimationStartPosition;
        Vector2 endPosition = this.dayAnimationEndPosition;
        animationCoroutine = this.CreateAnimationRoutine(
            dayAnimationDuration,
            delegate(float progress){
                float easedProgress = Easing.clerp(0, 1, progress);
                Vector2 pos = Vector2.Lerp(startPosition, endPosition, easedProgress);
                textDayPopUp.transform.localPosition = pos;
            },
            delegate {
                this.dayPopUp.SetActive(false);
                EventManager.animate = false;
            }
        );
    }

    public void StartTurnAnimation(Player player){
                EventManager.animate = true;
        string text = LocalizationSettings.StringDatabase.GetTable("Game").GetEntry("TurnOf").GetLocalizedString();
        this.textDayPopUp.GetComponent<TMP_Text>().text = text + " " + player.userName;
        this.dayPopUp.SetActive(true);
        Vector2 startPosition = this.dayAnimationStartPosition;
        Vector2 endPosition = this.dayAnimationEndPosition;
        animationCoroutine = this.CreateAnimationRoutine(
            dayAnimationDuration,
            delegate(float progress){
                float easedProgress = Easing.clerp(0, 1, progress);
                Vector2 pos = Vector2.Lerp(startPosition, endPosition, easedProgress);
                textDayPopUp.transform.localPosition = pos;
            },
            delegate {
                this.dayPopUp.SetActive(false);
                EventManager.animate = false;
            }
        );
    }

    public void ShowPopUp(){
        EventManager.animate = true;
        byte startBlur = 0;
        byte endBlur = this.blurValue;
        Vector2 startScale = new Vector2(0f, 0f);
        Vector2 endScale = this.popUpScale;
        this.popUp.SetActive(true);
        animationCoroutine = this.CreateAnimationRoutine(
            popUpInDuration,
            delegate(float progress){
                float easedProgress = Easing.easeInExpo(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 temp = backgroundPopUp.GetComponent<Image>().color;
                temp.a = blur;
                backgroundPopUp.GetComponent<Image>().color = temp;
                windowPopUp.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }

    public void HidePopUp(){
        EventManager.animate = true;
        byte startBlur = 0;
        byte endBlur = this.blurValue;
        Vector2 startScale = new Vector2(0f, 0f);
        Vector2 endScale = this.popUpScale;
        animationCoroutine = this.CreateAnimationRoutine(
            popUpInDuration,
            delegate(float progress){
                float easedProgress = Easing.easeInExpo(0, 1, progress);
                Vector2 scale = Vector2.Lerp(endScale, startScale, easedProgress);
                byte blur = (byte) Mathf.Lerp(endBlur, startBlur, easedProgress);
                Color32 temp = backgroundPopUp.GetComponent<Image>().color;
                temp.a = blur;
                backgroundPopUp.GetComponent<Image>().color = temp;
                windowPopUp.transform.localScale = scale;
            },
            delegate{
                this.popUp.SetActive(false);
                EventManager.animate = false;
            }
        );
    }
    public void ShowCardPick(){
        EventManager.animate = true;
        byte startBlur = 0;
        byte endBlur = this.blurValue;
        Vector2 startScale = new Vector2(0f, 0f);
        Vector2 endScale = this.popUpScale;
        this.popUp.SetActive(true);
        this.cardPick.SetActive(true);
        animationCoroutine = this.CreateAnimationRoutine(
            popUpInDuration,
            delegate(float progress){
                float easedProgress = Easing.easeInExpo(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 temp = backgroundPopUp.GetComponent<Image>().color;
                temp.a = blur;
                backgroundPopUp.GetComponent<Image>().color = temp;
                windowPopUp.transform.localScale = scale;
                cardPick.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }

    public void HideCardPick(){
        EventManager.animate = true;
        byte startBlur = 0;
        byte endBlur = this.blurValue;
        Vector2 startScale = new Vector2(0f, 0f);
        Vector2 endScale = this.popUpScale;
        animationCoroutine = this.CreateAnimationRoutine(
            popUpInDuration,
            delegate(float progress){
                float easedProgress = Easing.easeInExpo(0, 1, progress);
                Vector2 scale = Vector2.Lerp(endScale, startScale, easedProgress);
                byte blur = (byte) Mathf.Lerp(endBlur, startBlur, easedProgress);
                Color32 temp = backgroundPopUp.GetComponent<Image>().color;
                temp.a = blur;
                backgroundPopUp.GetComponent<Image>().color = temp;
                windowPopUp.transform.localScale = scale;
                cardPick.transform.localScale = scale;
            },
            delegate{
                this.popUp.SetActive(false);
                this.cardPick.SetActive(false);
                EventManager.animate = false;
            }
        );
    }

    public void ShowChoice(){
        EventManager.animate = true;
        byte startBlur = 0;
        byte endBlur = this.blurValue;
        Vector2 startScale = new Vector2(0f, 0f);
        Vector2 endScale = this.popUpScale;
        this.popUp.SetActive(true);
        this.choicePopUp.SetActive(true);
        animationCoroutine = this.CreateAnimationRoutine(
            popUpInDuration,
            delegate(float progress){
                float easedProgress = Easing.easeInExpo(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 temp = backgroundPopUp.GetComponent<Image>().color;
                temp.a = blur;
                backgroundPopUp.GetComponent<Image>().color = temp;
                windowPopUp.transform.localScale = scale;
                choicePopUp.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }

    public void ShowResults(){
        EventManager.animate = true;
        byte startBlur = 0;
        byte endBlur = this.blurValue;
        Vector2 startScale = new Vector2(0f, 0f);
        Vector2 endScale = this.popUpScale;
        this.popUp.SetActive(true);
        this.resultPopUp.SetActive(true);
        animationCoroutine = this.CreateAnimationRoutine(
            popUpInDuration,
            delegate(float progress){
                float easedProgress = Easing.easeInExpo(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 temp = backgroundPopUp.GetComponent<Image>().color;
                temp.a = blur;
                backgroundPopUp.GetComponent<Image>().color = temp;
                windowPopUp.transform.localScale = scale;
                resultPopUp.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }
    
    public void HideResults(){
        EventManager.animate = true;
        byte startBlur = this.blurValue;
        byte endBlur = 0;
        Vector2 startScale = this.popUpScale;
        Vector2 endScale = new Vector2(0f, 0f);
        animationCoroutine = this.CreateAnimationRoutine(
            popUpInDuration,
            delegate(float progress){
                float easedProgress = Easing.easeInExpo(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 temp = backgroundPopUp.GetComponent<Image>().color;
                temp.a = blur;
                backgroundPopUp.GetComponent<Image>().color = temp;
                windowPopUp.transform.localScale = scale;
                resultPopUp.transform.localScale = scale;
            },
            delegate{
                this.popUp.SetActive(false);
                this.resultPopUp.SetActive(false);
                EventManager.animate = false;
            }
        );
    }

    public void StartGame(){
        EventManager.animate = true;
        this.coverImage.color = Color.white;
        this.coverImage.gameObject.SetActive(true);
        byte startBlur = 255;
        byte endBlur = 0;
        animationCoroutine = this.CreateAnimationRoutine(
            startGameScreenDuration,
            delegate(float progress){
                float easedProgress = Easing.easeInBack(0, 1, progress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 temp = coverImage.color;
                temp.a = blur;
                coverImage.color = temp;
            },
            delegate {
                this.coverImage.gameObject.SetActive(false);
                EventManager.animate = false;
            }
        );
    }

    public void ShowInfo(string info){
        EventManager.animate = true;
        this.textDayPopUp.GetComponent<TMP_Text>().text = info;
        this.dayPopUp.SetActive(true);
        Vector2 startPosition = this.dayAnimationStartPosition;
        Vector2 endPosition = this.dayAnimationEndPosition;
        animationCoroutine = this.CreateAnimationRoutine(
            dayAnimationDuration,
            delegate(float progress){
                float easedProgress = Easing.clerp(0, 1, progress);
                Vector2 pos = Vector2.Lerp(startPosition, endPosition, easedProgress);
                textDayPopUp.transform.localPosition = pos;
            },
            delegate {
                this.dayPopUp.SetActive(false);
                EventManager.animate = false;
            }
        );
    }

    public void ShowTDDD(){
        EventManager.animate = true;
        byte startBlur = 0;
        byte endBlur = this.blurValue;
        Vector2 startScale = new Vector2(0f, 0f);
        Vector2 endScale = this.popUpScale;
        this.popUp.SetActive(true);
        this.tdddPopUp.SetActive(true);
        animationCoroutine = this.CreateAnimationRoutine(
            popUpInDuration,
            delegate(float progress){
                float easedProgress = Easing.easeInExpo(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 temp = backgroundPopUp.GetComponent<Image>().color;
                temp.a = blur;
                backgroundPopUp.GetComponent<Image>().color = temp;
                windowPopUp.transform.localScale = scale;
                tdddPopUp.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );

    }

    public void HideTDDD(){
        EventManager.animate = true;
        byte startBlur = this.blurValue;
        byte endBlur = 0;
        Vector2 startScale = this.popUpScale;
        Vector2 endScale = new Vector2(0f, 0f);
        animationCoroutine = this.CreateAnimationRoutine(
            popUpInDuration,
            delegate(float progress){
                float easedProgress = Easing.easeInExpo(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 temp = backgroundPopUp.GetComponent<Image>().color;
                temp.a = blur;
                backgroundPopUp.GetComponent<Image>().color = temp;
                windowPopUp.transform.localScale = scale;
                tdddPopUp.transform.localScale = scale;
            },
            delegate{
                this.popUp.SetActive(false);
                this.tdddPopUp.SetActive(false);
                EventManager.animate = false;
            }
        );

    }

    public void ZoomInPopUp(GameObject gameObject){
        EventManager.animate = true;
        Vector2 startScale = new Vector2(0f, 0f);
        Vector2 endScale = this.popUpScale;
        gameObject.SetActive(true);
        animationCoroutine = this.CreateAnimationRoutine(
            popUpInDuration,
            delegate(float progress){
                float easedProgress = Easing.easeInExpo(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                gameObject.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }

    public void ZoomOutPopUp(GameObject gameObject){
        EventManager.animate = true;
        Vector2 startScale = this.popUpScale;
        Vector2 endScale = new Vector2(0f, 0f);
        animationCoroutine = this.CreateAnimationRoutine(
            popUpInDuration,
            delegate(float progress){
                float easedProgress = Easing.easeInExpo(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                gameObject.transform.localScale = scale;
            },
            delegate{
                gameObject.SetActive(false);
                EventManager.animate = false;
            }
        );

    }

    public void UpdateDebtScrollBar(float newValue){
        EventManager.animate = true;
        Vector2 startPos = this.debtSlider.transform.localPosition;
        Vector2 startScale = this.debtSlider.transform.localScale;
        float startValue = this.debtSlider.value;
        Vector2 endPos = Vector2.zero;
        Vector2 endScale = new Vector2(4f, 4f);
        float endValue = newValue;
        animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 pos = Vector2.Lerp(startPos, endPos, easedProgress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                this.debtSlider.transform.localPosition = pos;
                this.debtSlider.transform.localScale = scale;
            },
            delegate{
                this.CreateAnimationRoutine(
                    1.5f,
                    delegate(float progress){
                        float easedProgress = Easing.easeInOutQuad(0, 1, progress);
                        float value = Mathf.Lerp(startValue, endValue, easedProgress);
                        this.debtSlider.value = value;
                    },
                    delegate{
                        this.debtSlider.value = endValue;
                        this.CreateAnimationRoutine(
                            0.5f,
                            delegate(float progress){
                                float easedProgress = Easing.easeOutCubic(0, 1, progress);
                                Vector2 pos = Vector2.Lerp(endPos, startPos, easedProgress);
                                Vector2 scale = Vector2.Lerp(endScale, startScale, easedProgress);
                                this.debtSlider.transform.localPosition = pos;
                                this.debtSlider.transform.localScale = scale;
                            },
                            delegate{
                                EventManager.animate = false;
                            }
                        );
                    }
                );
            }
        );
    }

}
