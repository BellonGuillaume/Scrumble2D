using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class AnimationManager : MonoBehaviour
{
    public static bool animated;
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
        this.animator.SetBool("ANIMATE", true);
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
                this.animator.SetBool("ANIMATE", false);
            }
        );
    }

    public void StartTurnAnimation(Player player){
        this.animator.SetBool("ANIMATE", true);
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
                this.animator.SetBool("ANIMATE", false);
            }
        );
    }

    public void ShowChoice(){
        this.animator.SetBool("ANIMATE", true);
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
                this.animator.SetBool("ANIMATE", false);
            }
        );
    }

    public void HideResults(){
        this.animator.SetBool("ANIMATE", true);
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
                this.animator.SetBool("ANIMATE", false);
            }
        );
    }

    public void StartGame(){
        this.animator.SetBool("ANIMATE", true);
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
                this.animator.SetBool("ANIMATE", false);
            }
        );
    }

    public void ShowInfo(string info){
        this.animator.SetBool("ANIMATE", true);
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
                this.animator.SetBool("ANIMATE", false);
            }
        );
    }

    public void ShowTDDD(){
        this.animator.SetBool("ANIMATE", true);
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
                this.animator.SetBool("ANIMATE", false);
            }
        );

    }

    public void HideTDDD(){
        this.animator.SetBool("ANIMATE", true);
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
                this.animator.SetBool("ANIMATE", false);
            }
        );

    }

    public void ZoomInPopUp(GameObject gameObject){
        this.animator.SetBool("ANIMATE", true);
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
                this.animator.SetBool("ANIMATE", false);
            }
        );
    }

    public void ZoomOutPopUp(GameObject gameObject){
        this.animator.SetBool("ANIMATE", true);
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
                this.animator.SetBool("ANIMATE", false);
            }
        );

    }

    public void UpdateDebtScrollBar(float newValue){
        this.animator.SetBool("ANIMATE", true);
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
                                this.animator.SetBool("ANIMATE", false);
                            }
                        );
                    }
                );
            }
        );
    }

}
