using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICard : MonoBehaviour
{
    [SerializeField] Sprite dailyRecto;
    [SerializeField] Sprite dailyVerso;
    [SerializeField] Sprite problemRecto;
    [SerializeField] Sprite problemVerso;
    [SerializeField] Sprite reviewRecto;
    [SerializeField] Sprite reviewVerso;
    [SerializeField] TMP_Text description;
    [SerializeField] TMP_Text result;
    [SerializeField] Image verso;
    [SerializeField] Image recto;
    [SerializeField] Button clickHandler;
    Color RED = new Color32(202, 95, 95, 255);
    Color BLUE = new Color32(88, 136, 199, 255);
    Color GREEN = new Color32(113, 203, 99, 255);
    public Card card;
    public CardPicker cardPicker;
    public int id;
    public static int count = 0;
    public bool moved = false;
    public Vector2 positionBeforeMove;
    public Vector2 positionAfterMove;

    void Update(){
        if(moved){
            this.transform.position = this.positionAfterMove;
        }
    }

    public void Fill(Card card, CardPicker cardPicker){
        this.card = card;
        this.description.text = card.description;
        this.result.text = card.result;
        switch (card.category){
            case Card.CategoryOfCard.DAILY:
                this.recto.color = BLUE;
                // this.recto.sprite = dailyRecto;
                this.verso.sprite = dailyVerso;
                break;
            case Card.CategoryOfCard.PROBLEM:
                this.recto.color = RED;
                // this.recto.sprite = problemRecto;
                this.verso.sprite = problemVerso;
                break;
            case Card.CategoryOfCard.REVIEW:
                this.recto.color = GREEN;
                // this.recto.sprite = reviewRecto;
                this.verso.sprite = reviewVerso;
                break;
        }
        this.cardPicker = cardPicker;
        id = count;
        count++;
    }

    public void OnClick(){
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

    public void Disable(){
        this.clickHandler.GetComponent<Button>().interactable = false;
        Color32 temp = this.clickHandler.GetComponent<Image>().color;
        temp.a = 136;
        this.clickHandler.GetComponent<Image>().color = temp;
    }

}