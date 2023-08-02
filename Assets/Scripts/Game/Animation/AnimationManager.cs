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
    [SerializeField] GameObject scrumboard;
    [SerializeField] GameObject burndownChart;

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
        Color32 startColor = new Color32(0, 0, 0, 0);
        Color32 endColor = new Color32(0, 0, 0, 255);
        float startAlpha = 0;
        float endAlpha = 1;
        Vector2 startPosition = this.daySlider.transform.localPosition;
        Vector2 endPosition = Vector2.zero;
        Vector2 startScale = this.daySlider.transform.localScale;
        Vector2 endScale = new Vector2(4f, 4f);
        float startDayValue = this.daySlider.value;
        float endDayValue = (float) n;
        this.coverImage.color = startColor;
        Transform oldDayParent = this.daySlider.transform.parent;
        int oldSiblingIndex = this.daySlider.transform.GetSiblingIndex();
        this.coverImage.gameObject.SetActive(true);
        animationCoroutine = this.CreateAnimationRoutine(
            dayAnimationDuration,
            delegate(float progress){
                float easedProgress = Easing.clerp(0, 1, progress);
                Color32 color = Color32.Lerp(startColor, endColor, easedProgress);
                this.coverImage.color = color;
            },
            delegate {
                this.daySlider.GetComponent<DaySlider>().SetAlpha(startAlpha);
                this.daySlider.transform.localPosition = endPosition;
                this.daySlider.transform.localScale = endScale;
                this.daySlider.transform.SetParent(this.daySlider.transform.root);
                this.daySlider.transform.SetAsLastSibling();
                this.CreateAnimationRoutine(
                    dayAnimationDuration,
                    delegate(float progress){
                        float easedProgress = Easing.clerp(0, 1, progress);
                        float alpha = Mathf.Lerp(startAlpha, endAlpha, easedProgress);
                        this.daySlider.GetComponent<DaySlider>().SetAlpha(alpha);
                    },
                    delegate{
                        this.CreateAnimationRoutine(
                            0.5f,
                            delegate(float progress){
                                float easedProgress = Easing.clerp(0, 1, progress);
                                float value = Mathf.Lerp(startDayValue, endDayValue, easedProgress);
                                this.daySlider.value = value;
                            },
                            delegate{
                                this.CreateAnimationRoutine(
                                    dayAnimationDuration,
                                    delegate(float progress){
                                        float easedProgress = Easing.clerp(0, 1, progress);
                                        Vector2 scale = Vector2.Lerp(endScale, startScale, easedProgress);
                                        Vector2 position = Vector2.Lerp(endPosition, startPosition, easedProgress);
                                        Color32 color = Color32.Lerp(endColor, startColor, easedProgress);
                                        this.daySlider.transform.localPosition = position;
                                        this.daySlider.transform.localScale = scale;
                                        this.coverImage.color = color;
                                    },
                                    delegate{
                                        this.coverImage.gameObject.SetActive(false);
                                        this.daySlider.transform.SetParent(oldDayParent);
                                        this.daySlider.transform.SetSiblingIndex(oldSiblingIndex);
                                        EventManager.animate = false;
                                    }
                                );
                            }
                        );
                    }
                );
            }
        );
    }

    public void StartTurnAnimation(Player player){
                EventManager.animate = true;
        string text = GetString("Game", "TurnOf");
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

    public void ShowTDDD(bool withPopUp = true){
        EventManager.animate = true;
        byte startBlur = 0;
        byte endBlur = this.blurValue;
        Vector2 startScale = new Vector2(0f, 0f);
        Vector2 endScale = this.popUpScale;
        if(withPopUp)
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
                if(withPopUp)
                    backgroundPopUp.GetComponent<Image>().color = temp;
                if(withPopUp)
                    windowPopUp.transform.localScale = scale;
                tdddPopUp.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );

    }

    public void HideTDDD(bool withPopUp = true){
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
                if(withPopUp)
                    backgroundPopUp.GetComponent<Image>().color = temp;
                if(withPopUp)
                    windowPopUp.transform.localScale = scale;
                tdddPopUp.transform.localScale = scale;
            },
            delegate{
                if(withPopUp)
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
        Vector2 endScale = new Vector2(3f, 3f);
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
                                StateManager.UpdateDebt(newValue);
                            }
                        );
                    }
                );
            }
        );
    }

    public void RemoveCard(GameObject card){
        card.GetComponent<UICard>().positionBeforeMove = card.transform.position;
        Vector2 startPos = card.GetComponent<UICard>().positionBeforeMove;
        Vector2 endPos = new Vector2(startPos.x, 2000f);
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 position = Vector2.Lerp(startPos, endPos, easedProgress);
                card.transform.position = position;
            },
            delegate{
                Destroy(card);
                EventManager.cardToRemove--;
            }
        );
        
    }

    public void CenterCard(GameObject card){
        EventManager.animate = true;
        card.GetComponent<UICard>().positionBeforeMove = card.transform.position;
        Vector2 startPos = card.GetComponent<UICard>().positionBeforeMove;
        Vector2 endPos = new Vector2(960f, 540f);
        Vector2 startScale = card.transform.localScale;
        Vector2 endScale = new Vector2(1.2f, 1.2f);
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 position = Vector2.Lerp(startPos, endPos, easedProgress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                card.transform.position = position;
                card.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }

    public void UncenterCard(GameObject card){
        EventManager.animate = true;
        Vector2 startPos = new Vector2(960f, 540f);
        Vector2 endPos = card.GetComponent<UICard>().positionBeforeMove;
        Vector2 startScale = card.transform.localScale;
        Vector2 endScale = Vector2.one;
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 position = Vector2.Lerp(startPos, endPos, easedProgress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                card.transform.position = position;
                card.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }
    
    public void RollToFace(GameObject dice, int value){
        EventManager.animate = true;
        float startRotation = 0;
        float leftRotation = -Random.Range(10, 45);
        float rightRotation = Random.Range(10, 45);
        float endRotation = 0;
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.05f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                float rotation = Mathf.Lerp(startRotation, leftRotation, easedProgress);
                dice.transform.GetChild(0).localRotation = Quaternion.Euler(0f, 0f, rotation);
            },
            delegate{
                this.CreateAnimationRoutine(
                    0.1f,
                    delegate(float progress){
                        float easedProgress = Easing.easeInCubic(0, 1, progress);
                        float rotation = Mathf.Lerp(leftRotation, rightRotation, easedProgress);
                        dice.transform.GetChild(0).localRotation = Quaternion.Euler(0f, 0f, rotation);
                    },
                    delegate{
                        dice.GetComponent<UIDice>().currentFace = value;
                        this.CreateAnimationRoutine(
                            0.05f,
                            delegate(float progress){
                                float easedProgress = Easing.easeInCubic(0, 1, progress);
                                float rotation = Mathf.Lerp(rightRotation, endRotation, easedProgress);
                                dice.transform.GetChild(0).localRotation = Quaternion.Euler(0f, 0f, rotation);
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

    public void ShowScrumboard(GameObject scrumBoard){
        EventManager.animate = true;
        Vector2 startScale = scrumBoard.transform.localScale;
        Vector2 endScale = Vector2.one;
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.2f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                scrumBoard.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }
    public void HideScrumboard(GameObject scrumBoard){
        EventManager.animate = true;
        Vector2 startScale = Vector2.one;
        Vector2 endScale = new Vector2(0.33f, 0.33f);
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.2f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                scrumBoard.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }
    public void ShowBurndownChart(){
        EventManager.animate = true;
        Vector2 startScale = this.burndownChart.transform.localScale;
        Vector2 endScale = Vector2.one;
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.2f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                this.burndownChart.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }
    public void HideBurndownChart(){
        EventManager.animate = true;
        Vector2 startScale = Vector2.one;
        Vector2 endScale = new Vector2(0.33f, 0.33f);
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.2f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                this.burndownChart.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }
    public void ShowDailyPlayer(GameObject dailyPlayer){
        EventManager.animate = true;
        byte startBlur = 0;
        byte endBlur = this.blurValue;
        Vector2 startScale = new Vector2(0f, 0f);
        Vector2 endScale = this.popUpScale;
        this.popUp.SetActive(true);
        dailyPlayer.SetActive(true);
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
                dailyPlayer.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }

    public void HideDailyPlayer(GameObject dailyPlayer){
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
                dailyPlayer.transform.localScale = scale;
            },
            delegate{
                this.popUp.SetActive(false);
                dailyPlayer.SetActive(false);
                EventManager.animate = false;
            }
        );
    }
    public IEnumerator FlipCard(GameObject card){
        yield return new WaitUntil(() => EventManager.animate == false);
        EventManager.animate = true;
        Vector2 startScale = card.transform.localScale;
        Vector2 intermediateScale = new Vector2(0f, startScale.y);
        Vector2 endScale = startScale;
        animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate(float progress){
                float easedProgress = Easing.easeInCirc(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, intermediateScale, easedProgress);
                card.transform.localScale = scale;
            },
            delegate{
                card.GetComponent<UICard>().RemoveVerso();
                this.CreateAnimationRoutine(
                    0.5f,
                    delegate(float progress){
                        float easedProgress = Easing.easeOutCirc(0, 1, progress);
                        Vector2 scale = Vector2.Lerp(intermediateScale, endScale, easedProgress);
                        card.transform.localScale = scale;
                    },
                    delegate{
                        EventManager.animate = false;
                    }
                );
            }
        );
    }

    public void ShowReviewScreen(GameObject reviewScreen, GameObject background, GameObject container){
        EventManager.animate = true;
        byte startBlur = 0;
        byte endBlur = this.blurValue;
        Vector2 startScale = Vector2.zero;
        Vector2 endScale = Vector2.one;
        reviewScreen.SetActive(true);
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.25f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 temp = background.GetComponent<Image>().color;
                temp.a = blur;
                background.GetComponent<Image>().color = temp;
                container.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }

    public void HideReviewScreen(GameObject reviewScreen, GameObject background, GameObject container){
        EventManager.animate = true;
        byte startBlur = this.blurValue;
        byte endBlur = 0;
        Vector2 startScale = Vector2.one;
        Vector2 endScale = Vector2.zero;
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.25f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 temp = background.GetComponent<Image>().color;
                temp.a = blur;
                background.GetComponent<Image>().color = temp;
                container.transform.localScale = scale;
            },
            delegate{
                reviewScreen.SetActive(false);
                container.transform.localScale = startScale;
                EventManager.animate = false;
            }
        );
    }

    public void MoveOutOfScreen(GameObject arrowedUS)
    {
        EventManager.animate = true;
        Vector2 startPos = arrowedUS.transform.position;
        Vector2 endPos = new Vector2(2220f, 540f);
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate(float progress){
                float easedProgress = Easing.easeInExpo(0, 1, progress);
                Vector2 pos = Vector2.Lerp(startPos, endPos, easedProgress);
                arrowedUS.transform.position = pos;
            },
            delegate{
                arrowedUS.SetActive(false);
                EventManager.animate = false;
            }
        );
    }

    public void ShiftUsToRight(GameObject arrowedUS){
        Vector2 startPos = arrowedUS.transform.position;
        Vector2 endPos = new Vector2(arrowedUS.transform.position.x + 960f, arrowedUS.transform.position.y);
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 pos = Vector2.Lerp(startPos, endPos, easedProgress);
                arrowedUS.transform.position = pos;
            },
            delegate{
                EventManager.usToShift--;
            }
        );
    }

    public void MoveUSToCase(GameObject arrowedUS, Vector2 position){
        EventManager.animate = true;
        Vector2 startPos = arrowedUS.transform.position;
        Vector2 endPos = position;
        Vector2 startScale = Vector2.one;
        Vector2 endScale = new Vector2(0.291666f, 0.291666f);
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.2f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 pos = Vector2.Lerp(startPos, endPos, easedProgress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                arrowedUS.transform.position = pos;
                arrowedUS.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }

    public void ApproveUS(GameObject arrowedUS){
        EventManager.animate = true;
        Vector2 startScale = new Vector2(3f, 3f);
        Vector2 endScale = new Vector2(1f, 1f);
        arrowedUS.GetComponent<ArrowedUS>().approvedSeal.transform.localScale = startScale;
        arrowedUS.GetComponent<ArrowedUS>().approvedSeal.gameObject.SetActive(true);
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.2f,
            delegate(float progress){
                float easedProgress = Easing.easeOutExpo(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                arrowedUS.GetComponent<ArrowedUS>().approvedSeal.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }

    public void ShowNewValue(GameObject arrow, GameObject textBox){
        EventManager.animate = true;
        byte startBlur = 0;
        byte endBlur = this.blurValue;
        Vector2 startArrowPos = new Vector2(arrow.transform.localPosition.x -10, arrow.transform.localPosition.y);
        Vector2 endArrowPos = arrow.transform.localPosition;
        Vector2 startTextBoxPos = new Vector2(textBox.transform.localPosition.x -10, textBox.transform.localPosition.y);
        Vector2 endTextBoxPos = textBox.transform.localPosition;
        arrow.SetActive(true);
        textBox.SetActive(true);
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 arrowPos = Vector2.Lerp(startArrowPos, endArrowPos, easedProgress);
                Vector2 textBoxPos = Vector2.Lerp(startTextBoxPos, endTextBoxPos, easedProgress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 arrowTemp = arrow.GetComponent<TMP_Text>().color;
                Color32 textBoxTemp = textBox.GetComponent<TMP_Text>().color;
                arrowTemp.a = blur;
                textBoxTemp.a = blur;
                arrow.GetComponent<TMP_Text>().color = arrowTemp;
                textBox.GetComponent<TMP_Text>().color = textBoxTemp;
                arrow.transform.localPosition = arrowPos;
                textBox.transform.localPosition = textBoxPos;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }

    public void DiscardCard(GameObject card){
        EventManager.animate = true;
        Vector2 startPos = card.transform.position;
        Vector2 endPos = new Vector2(startPos.x, 1300);
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 pos = Vector2.Lerp(startPos, endPos, easedProgress);
                card.transform.position = pos;
            },
            delegate{
                card.SetActive(false);
                card.transform.position = startPos;
                EventManager.animate = false;
            }
        );
    }

    public void AddPermanentCard(GameObject card, GameObject permanentCard){
        EventManager.animate = true;
        Vector2 startPos = card.transform.position;
        Vector2 endPos = permanentCard.transform.position;
        Vector3 startScale = card.transform.localScale;
        Vector3 endScale = permanentCard.transform.localScale;
        this.animationCoroutine = this.CreateAnimationRoutine(
            1f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 pos = Vector2.Lerp(startPos, endPos, easedProgress);
                Vector3 scale = Vector3.Lerp(startScale, endScale, easedProgress);
                card.transform.position = pos;
                card.transform.localScale = scale;
            },
            delegate{
                permanentCard.SetActive(true);
                card.GetComponent<UICard>().SetAlpha(0);
                card.transform.position = startPos;
                EventManager.animate = false;
            }
        );
    }

    public void ShowFilledChoice(GameObject filledChoice){
        EventManager.animate = true;
        byte startBlur = 0;
        byte endBlur = this.blurValue;
        Vector2 startScale = new Vector2(0f, 0f);
        Vector2 endScale = this.popUpScale;
        this.popUp.SetActive(true);
        filledChoice.SetActive(true);
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
                filledChoice.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }

    public void HideFilledChoice(GameObject filledChoice){
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
                filledChoice.transform.localScale = scale;
            },
            delegate{
                this.popUp.SetActive(false);
                filledChoice.SetActive(false);
                EventManager.animate = false;
            }
        );
    }

    public void ShowSideMenu(GameObject sideMenu, GameObject elements, GameObject background){
        EventManager.animate = true;
        Vector2 startPos = elements.transform.position;
        Vector2 endPos = new Vector2(0, startPos.y);
        byte startBlur = 0;
        byte endBlur = this.blurValue;
        sideMenu.SetActive(true);
        animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate(float progress){
                float easedProgress = Easing.easeOutCubic(0, 1, progress);
                Vector2 pos = Vector2.Lerp(startPos, endPos, easedProgress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 temp = background.GetComponent<Image>().color;
                temp.a = blur;
                background.GetComponent<Image>().color = temp;
                elements.transform.position = pos;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }

    public void HideSideMenu(GameObject sideMenu, GameObject elements, GameObject background){
        EventManager.animate = true;
        Vector2 startPos = elements.transform.position;
        Vector2 endPos = new Vector2(-1280, startPos.y);
        byte startBlur = this.blurValue;
        byte endBlur = 0;
        animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 pos = Vector2.Lerp(startPos, endPos, easedProgress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 temp = background.GetComponent<Image>().color;
                temp.a = blur;
                background.GetComponent<Image>().color = temp;
                elements.transform.position = pos;
            },
            delegate{
                sideMenu.SetActive(false);
                EventManager.animate = false;
            }
        );
    }

    public void ShowSummary(GameObject summary, GameObject elements, GameObject background){
        EventManager.animate = true;
        Vector2 startPos = elements.transform.position;
        Vector2 endPos = new Vector2(1920, startPos.y);
        byte startBlur = 0;
        byte endBlur = this.blurValue;
        summary.SetActive(true);
        animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate(float progress){
                float easedProgress = Easing.easeOutCubic(0, 1, progress);
                Vector2 pos = Vector2.Lerp(startPos, endPos, easedProgress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 temp = background.GetComponent<Image>().color;
                temp.a = blur;
                background.GetComponent<Image>().color = temp;
                elements.transform.position = pos;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }

    public void HideSummary(GameObject summary, GameObject elements, GameObject background){
        EventManager.animate = true;
        Vector2 startPos = elements.transform.position;
        Vector2 endPos = new Vector2(3200, startPos.y);
        byte startBlur = this.blurValue;
        byte endBlur = 0;
        animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 pos = Vector2.Lerp(startPos, endPos, easedProgress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 temp = background.GetComponent<Image>().color;
                temp.a = blur;
                background.GetComponent<Image>().color = temp;
                elements.transform.position = pos;
            },
            delegate{
                summary.SetActive(false);
                EventManager.animate = false;
            }
        );
    }

    public void ShowRetrospective(GameObject retrospective, GameObject elements, GameObject background){
        EventManager.animate = true;
        Vector2 startScale = Vector2.zero;
        Vector2 endScale = Vector2.one;
        byte startBlur = 0;
        byte endBlur = this.blurValue;
        retrospective.SetActive(true);
        animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate(float progress){
                float easedProgress = Easing.easeOutCubic(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 temp = background.GetComponent<Image>().color;
                temp.a = blur;
                background.GetComponent<Image>().color = temp;
                elements.transform.localScale = scale;
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }

    public void HideRetrospective(GameObject retrospective, GameObject elements, GameObject background){
        EventManager.animate = true;
        Vector2 startScale = Vector2.one;
        Vector2 endScale = Vector2.zero;
        byte startBlur = this.blurValue;
        byte endBlur = 0;
        animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate(float progress){
                float easedProgress = Easing.easeInCubic(0, 1, progress);
                Vector2 scale = Vector2.Lerp(startScale, endScale, easedProgress);
                byte blur = (byte) Mathf.Lerp(startBlur, endBlur, easedProgress);
                Color32 temp = background.GetComponent<Image>().color;
                temp.a = blur;
                background.GetComponent<Image>().color = temp;
                elements.transform.localScale = scale;
            },
            delegate{
                retrospective.SetActive(false);
                retrospective.transform.localScale = Vector2.one;
                EventManager.animate = false;
            }
        );
    }

    public void ShowPermanentCard(GameObject permanentCard){
        EventManager.animate = true;
        Vector3 newPos = new Vector3(1440, 540, 0);
        Vector3 startScale = Vector3.one;
        Vector3 endScale = new Vector3(1.2f, 1.2f, 1f);
        float startAlpha = 0;
        float endAlpha = 255;
        permanentCard.GetComponent<UICard>().SetAlpha(0);
        permanentCard.transform.position = newPos;
        permanentCard.transform.localScale = startScale;
        permanentCard.transform.SetParent(popUp.transform);
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate (float progress){
                float easedProgress = Easing.easeInQuint(0, 1, progress);
                Vector3 scale = Vector3.Lerp(startScale, endScale, easedProgress);
                float alpha = Mathf.Lerp(startAlpha, endAlpha, easedProgress);
                permanentCard.transform.localScale = scale;
                permanentCard.GetComponent<UICard>().SetAlpha(alpha);
            },
            delegate{
                EventManager.animate = false;
            }
        );
    }

    public void HidePermanentCard(GameObject permanentCard, Transform initParent, int initIndex, Vector3 initPos, Vector3 initScale){
        EventManager.animate = true;
        float startAlpha = 255;
        float endAlpha = 0;
        this.animationCoroutine = this.CreateAnimationRoutine(
            0.5f,
            delegate (float progress){
                float easedProgress = Easing.easeOutQuint(0, 1, progress);
                float alpha = Mathf.Lerp(startAlpha, endAlpha, easedProgress);
                permanentCard.GetComponent<UICard>().SetAlpha(alpha);
                Debug.Log($"Putting the alpha to {alpha}");
                Debug.Log($"Here is the alpha value of the card : {permanentCard.GetComponent<UICard>().GetComponentsInChildren<Image>()[0].color.a}");
            },
            delegate{
                permanentCard.transform.SetParent(initParent);
                permanentCard.transform.SetSiblingIndex(initIndex);
                permanentCard.transform.position = initPos;
                permanentCard.transform.localScale = initScale;
                permanentCard.GetComponent<UICard>().SetAlpha(255);
                EventManager.animate = false;
            }
        );
    }

    public void ShowEndScreen(GameObject endScreen, Image blur){
        EventManager.animate = true;
        Color32 startColor = new Color32(0, 0, 0, 0);
        Color32 endColor = new Color32(0, 0, 0, 255);
        byte startAlpha = 255;
        byte endAlpha = 0;
        coverImage.color = startColor;
        coverImage.gameObject.SetActive(true);
        this.animationCoroutine = this.CreateAnimationRoutine(
            2f,
            delegate (float progress){
                float easedProgress = Easing.easeInCirc(0, 1, progress);
                Color32 color = Color32.Lerp(startColor, endColor, easedProgress);
                coverImage.color = color;
            },
            delegate{
                endScreen.SetActive(true);
                coverImage.gameObject.SetActive(false);
                this.CreateAnimationRoutine(
                    2f,
                    delegate (float progress){
                        float easedProgress = Easing.easeOutCirc(0, 1, progress);
                        byte alpha = (byte) Mathf.Lerp(startAlpha, endAlpha, easedProgress);
                        Color32 temp = blur.color;
                        temp.a = alpha;
                        blur.color = temp;
                    },
                    delegate{
                        blur.gameObject.SetActive(false);
                        EventManager.animate = false;
                    }
                );
            }
        );
        
    }
    public string GetString(string tableName, string stringKey){
        return LocalizationSettings.StringDatabase.GetLocalizedString(tableName, stringKey);
    }
}