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
    public int choosenIndex;

    public void AddCart(Card card){
        Debug.Log($"Adding the card {card.id.ToString()} : \n {card.ToString()}");
        GameObject go = Instantiate(cardUIPrefab);
        go.transform.SetParent(content.transform);
        go.GetComponent<UICard>().Fill(card, this);
        this.cards.Add(go);
        go.transform.localScale = Vector3.one;
        UICard.count++;
    }

    public void ChooseCard(int id){
        if (EventManager.cardsToPick <= 0 || choosenCard.GetComponent<UICard>().card != null){
            return;
        }
        Debug.Log($"Card ${id} picked : {this.cards[id].ToString()}");
        this.choosenCard.GetComponent<UICard>().Fill(this.cards[id].GetComponent<UICard>().card, this);
        this.choosenIndex = id;
        this.choosenCard.transform.position = this.cards[id].transform.position;
        this.choosenCard.transform.localScale = this.cards[id].transform.localScale;
        this.cards[id].GetComponent<UICard>().RemoveVerso();
        this.cards[id].GetComponent<UICard>().SetAlpha(0);
        this.choosenCard.SetActive(true);
        animationManager.CenterCard(this.choosenCard);
        StartCoroutine(animationManager.FlipCard(this.choosenCard));

        EventManager.cardsToPick--;
        foreach (GameObject card in cards){
            card.GetComponent<UICard>().Disable();
        }
    }

    public IEnumerator UnChooseCard(){
        animationManager.UncenterCard(this.choosenCard);
        if (EventManager.cardsToPick > 0){
            foreach (GameObject card in cards){
                if(card.GetComponent<UICard>().card.flipped == false){
                    card.GetComponent<UICard>().Enable();
                }   
            }
        }
        yield return new WaitUntil(() => EventManager.animate == false);
        this.cards[choosenIndex].GetComponent<UICard>().SetAlpha(255);
        this.cards[choosenIndex].GetComponent<UICard>().Disable();
        this.choosenCard.SetActive(false);
        this.choosenCard.GetComponent<UICard>().AddVerso();
        this.choosenCard.GetComponent<UICard>().UnFill();
        this.choosenIndex = -1;
    }

    public IEnumerator PlacePermanentCard(GameObject permanentCardLocation){
        animationManager.AddPermanentCard(this.choosenCard, permanentCardLocation);
        yield return new WaitUntil(() => EventManager.animate == false);
        this.choosenCard.SetActive(false);
        this.choosenCard.GetComponent<UICard>().AddVerso();
        this.choosenCard.GetComponent<UICard>().UnFill();
        this.choosenIndex = -1;
    }

    public void RemoveCards(){
        foreach(GameObject card in cards){
            EventManager.cardToRemove++;
            animationManager.RemoveCard(card);
        }
    }

    public void Reset(){
        foreach (Transform child in content.transform){
            Destroy(child.gameObject);
        }
        cards = new List<GameObject>();
        EventManager.cardsToPick = 0;
        UICard.count = 0;
    }
}
