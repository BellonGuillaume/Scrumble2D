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
    public GameObject choosenCard;

    public void AddCart(Card card){
        Debug.Log($"Try to add the card {card.id.ToString()}");
        GameObject go = Instantiate(cardUIPrefab);
        go.transform.SetParent(content.transform);
        go.GetComponent<UICard>().Fill(card, this);
        this.cards.Add(go);
        go.transform.localScale = Vector3.one;
    }

    public void ChooseCard(int id){
        if (EventManager.cardsToPick <= 0 || choosenCard != null){
            return;
        }
        // animationManager.FlipCard(cards[id].GetComponent<UICard>());
        Debug.Log($"Card ${id} picked : {this.cards[id].ToString()}");
        this.cards[id].GetComponent<UICard>().RemoveVerso();
        this.choosenCard = this.cards[id];
        animationManager.CenterCard(this.choosenCard);

        EventManager.cardsToPick--;
        if (EventManager.cardsToPick <= 0){
            foreach(GameObject go in this.cards){
                if (go.GetComponent<UICard>().card.flipped == false){
                    go.GetComponent<UICard>().Disable();
                    // animationManager.RemoveCardUI(go);
                }
            }
            RemoveUnflippedCards();
        }
    }

    public void RemoveUnflippedCards(){
        foreach(GameObject card in cards){
            if(card.GetComponent<UICard>().card.flipped == false){
                EventManager.cardToRemove++;
                animationManager.RemoveCard(card);
            }
        }
    }

    public void Reset(){
        foreach (Transform child in content.transform){
            Destroy(child.gameObject);
        }
        cards = new List<GameObject>();
        EventManager.cardsToPick = 0;
        UICard.count = 0;
        choosenCard = null;
    }
}
