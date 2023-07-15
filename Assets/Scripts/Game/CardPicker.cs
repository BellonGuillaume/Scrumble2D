using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPicker : MonoBehaviour
{
    [SerializeField] GameObject cardUIPrefab;

    [SerializeField] AnimationManager animationManager;
    [SerializeField] GameObject content;

    public Card.CategoryOfCard typeOfCard;
    public List<GameObject> cards = new List<GameObject>();
    public bool initialized;

    public void AddCart(Card card){
        Debug.Log($"Try to add the card {card.id.ToString()}");
        if (cards.Count == 0){
            this.typeOfCard = card.category;
        } else {
            if (card.category != this.typeOfCard){
                return;
            }
        }
        GameObject go = Instantiate(cardUIPrefab);
        go.transform.SetParent(content.transform);
        go.GetComponent<UICard>().Fill(card, this);
        this.cards.Add(go);
    }

    public void ChooseCard(int id){
        int cardToPick;
        if (this.typeOfCard == Card.CategoryOfCard.DAILY)
            cardToPick = EventManager.dailyCardsToPick;
        else if (this.typeOfCard == Card.CategoryOfCard.PROBLEM)
            cardToPick = EventManager.problemCardsToPick;
        else
            cardToPick = EventManager.reviewCardsToPick;

        if (cardToPick <= 0){
            return;
        }
        // animationManager.FlipCard(cards[id].GetComponent<UICard>());
        Debug.Log(id.ToString());
        Debug.Log(this.cards.Count.ToString());
        this.cards[id].GetComponent<UICard>().RemoveVerso();
        this.cards[id].GetComponent<UICard>().card.flipped = true;

        cardToPick--;
        if (this.typeOfCard == Card.CategoryOfCard.DAILY)
            EventManager.dailyCardsToPick = cardToPick;
        else if (this.typeOfCard == Card.CategoryOfCard.PROBLEM)
            EventManager.problemCardsToPick = cardToPick;
        else
            EventManager.reviewCardsToPick = cardToPick;

        if (cardToPick <= 0){
            foreach(GameObject go in this.cards){
                if (go.GetComponent<UICard>().card.flipped == false){
                    // animationManager.DeselectCard(go);
                    go.GetComponent<UICard>().Disable();
                }
            }
        }
    }

    public void Reset(){
        foreach (GameObject go in cards){
            Destroy(go);
        }
        cards = new List<GameObject>();
        EventManager.dailyCardsToPick = 0;
        EventManager.problemCardsToPick = 0;
        EventManager.reviewCardsToPick = 0;
        UICard.count = 0;
    }
}
