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

    public List<GameObject> cards = new List<GameObject>();
    public GameObject centralCard;
    public GameObject choosenCard;

    public void AddCart(Card card){
        Debug.Log($"Adding card {card.id} : \n {card.ToString()}");
        GameObject go = Instantiate(cardUIPrefab);
        go.transform.SetParent(content.transform);
        go.GetComponent<UICard>().Fill(card, this);
        this.cards.Add(go);
        go.transform.localScale = Vector3.one;
        UICard.count++;
    }

    public void ChooseCard(int id){
        if (EventManager.cardsToPick <= 0 || centralCard.GetComponent<UICard>().card != null){
            return;
        }
        this.choosenCard = this.cards[id];
        Debug.Log($"Choosing card {choosenCard.GetComponent<UICard>().card.id}");
        this.centralCard.GetComponent<UICard>().Fill(choosenCard.GetComponent<UICard>().card, this);
        this.centralCard.transform.position = choosenCard.transform.position;
        this.centralCard.transform.localScale = choosenCard.transform.localScale;
        this.centralCard.SetActive(true);
        this.choosenCard.GetComponent<UICard>().RemoveVerso();
        this.choosenCard.GetComponent<UICard>().SetAlpha(0);
        animationManager.CenterCard(this.centralCard);
        StartCoroutine(animationManager.FlipCard(this.centralCard));

        EventManager.cardsToPick--;
        foreach (GameObject card in cards){
            if (card != choosenCard)
                card.GetComponent<UICard>().Disable();
        }
    }

    public IEnumerator UnChooseCard(){
        Debug.Log($"Unchoosing card {choosenCard.GetComponent<UICard>().card.id}");
        animationManager.UncenterCard(this.centralCard);
        if (EventManager.cardsToPick > 0){
            foreach (GameObject card in cards){
                if(card.GetComponent<UICard>().flipped == false){
                    card.GetComponent<UICard>().Enable();
                }   
            }
        }
        yield return new WaitUntil(() => EventManager.animate == false);
        choosenCard.GetComponent<UICard>().SetAlpha(255);
        choosenCard.GetComponent<UICard>().Disable();
        this.centralCard.SetActive(false);
        this.centralCard.GetComponent<UICard>().AddVerso();
        this.centralCard.GetComponent<UICard>().UnFill();
    }

    public IEnumerator PlacePermanentCard(GameObject permanentCardLocation){
        animationManager.AddPermanentCard(this.centralCard, permanentCardLocation);
        yield return new WaitUntil(() => EventManager.animate == false);
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
