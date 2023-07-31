using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

public class UICard : MonoBehaviour
{
    [SerializeField] Sprite dailyRecto;
    [SerializeField] Sprite dailyVersoFR;
    [SerializeField] Sprite dailyVersoEN;
    [SerializeField] Sprite problemRecto;
    [SerializeField] Sprite problemVersoFR;
    [SerializeField] Sprite problemVersoEN;
    [SerializeField] Sprite reviewRecto;
    [SerializeField] Sprite reviewVersoFR;
    [SerializeField] Sprite reviewVersoEN;
    [SerializeField] TMP_Text description;
    [SerializeField] TMP_Text result;
    [SerializeField] Image verso;
    [SerializeField] Image recto;
    [SerializeField] Button clickHandler;
    Color BACK = new Color32(247, 252, 237, 255);   // #f7fced
    Color RED = new Color32(214, 71, 71, 255);      // #d64747
    Color BLUE = new Color32(79, 178, 228, 255);    // #4fb2e4
    Color GREEN = new Color32(122, 201, 67, 255);   // #7ac943
    Color PINK = new Color32(100, 2, 55, 255);
    public Card card;
    public CardPicker cardPicker;
    public int id;
    public static int count = 0;
    public Vector2 positionBeforeMove;

    public void Fill(Card card, CardPicker cardPicker){
        this.card = card;
        this.description.text = card.description;
        this.result.text = card.result;
        switch (card.category){
            case Card.CategoryOfCard.DAILY:
                // this.recto.color = BLUE;
                this.recto.sprite = dailyRecto;
                this.verso.sprite = GetDailyVerso();
                break;
            case Card.CategoryOfCard.PROBLEM:
                // this.recto.color = RED;
                this.recto.sprite = problemRecto;
                this.verso.sprite = GetProblemVerso();
                break;
            case Card.CategoryOfCard.REVIEW:
                // this.recto.color = GREEN;
                this.recto.sprite = reviewRecto;
                this.verso.sprite = GetReviewVerso();
                break;
        }
        this.cardPicker = cardPicker;
        id = count;
    }

    public void UnFill(){
        this.card = null;
        this.description.text = null;
        this.result.text = null;
        this.recto.sprite = null;
        this.verso.sprite = null;
        id = -1;
        this.positionBeforeMove = Vector2.zero;
    }

    public void OnClick(){
        Debug.Log("Clicked");
        this.cardPicker.ChooseCard(this.id);
    }

    public override string ToString()
    {
        return $"Recto with description {description} and result {result}";
    }
    public void RemoveVerso(){
        this.verso.gameObject.SetActive(false);
        this.card.flipped = true;
    }
    public void AddVerso(){
        this.verso.gameObject.SetActive(true);
        this.card.flipped = false;
    }

    public void Disable(){
        this.clickHandler.GetComponent<Button>().interactable = false;
        Color32 temp = this.clickHandler.GetComponent<Image>().color;
        temp.a = 136;
        this.clickHandler.GetComponent<Image>().color = temp;
    }
    public void Enable(){
        this.clickHandler.GetComponent<Button>().interactable = true;
        Color32 temp = this.clickHandler.GetComponent<Image>().color;
        temp.a = 0;
        this.clickHandler.GetComponent<Image>().color = temp;
    }

    public void SetAlpha(float alpha){
        Image[] images = GetComponentsInChildren<Image>();
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
        Color newColor;
        foreach(Image image in images){
            newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;
        }
        foreach(TMP_Text text in texts){
            newColor = text.color;
            newColor.a = alpha;
            text.color = newColor;
        }
    }

    public Sprite GetDailyVerso(){
        if (StateManager.language == LocalizationSettings.AvailableLocales.GetLocale("fr")){
            return this.dailyVersoFR;
        }
        return this.dailyVersoEN;
    }
    public Sprite GetProblemVerso(){
        if (StateManager.language == LocalizationSettings.AvailableLocales.GetLocale("fr")){
            return this.problemVersoFR;
        }
        return this.problemVersoEN;
    }
    public Sprite GetReviewVerso(){
        if (StateManager.language == LocalizationSettings.AvailableLocales.GetLocale("fr")){
            return this.reviewVersoFR;
        }
        return this.reviewVersoEN;
    }
}