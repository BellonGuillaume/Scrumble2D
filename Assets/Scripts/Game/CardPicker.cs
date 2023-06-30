using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPicker : MonoBehaviour
{
    [SerializeField] UICard uiCardPrefab;
    [SerializeField] Sprite dailyVerso;
    [SerializeField] Sprite problemVerso;
    [SerializeField] Sprite reviewVerso;

    Color RED = new Color32(202, 95, 95, 255);
    Color BLUE = new Color32(88, 136, 199, 255);
    Color GREEN = new Color32(113, 203, 99, 255);

    public static string typeOfCard;

    UICard cardOne;
    UICard cardTwo;
    UICard cardThree;
    // Start is called before the first frame update
    void Start()
    {
        this.cardOne = Instantiate(uiCardPrefab, new Vector3(460,540,0), transform.rotation);
        this.cardOne.transform.SetParent(this.transform);
        this.cardTwo = Instantiate(uiCardPrefab, new Vector3(960,540,0), transform.rotation);
        this.cardTwo.transform.SetParent(this.transform);
        this.cardThree = Instantiate(uiCardPrefab, new Vector3(1460,540,0), transform.rotation);
        this.cardThree.transform.SetParent(this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if(typeOfCard == "PROBLEM"){
            this.cardOne.verso.sprite = problemVerso;
            this.cardTwo.verso.sprite = problemVerso;
            this.cardThree.verso.sprite = problemVerso;
            this.cardOne.recto.color = RED;
            this.cardTwo.recto.color = RED;
            this.cardThree.recto.color = RED;
        }
        else if (typeOfCard == "DAILY"){
            this.cardOne.verso.sprite = dailyVerso;
            this.cardTwo.verso.sprite = dailyVerso;
            this.cardThree.verso.sprite = dailyVerso;
            this.cardOne.recto.color = BLUE;
            this.cardTwo.recto.color = BLUE;
            this.cardThree.recto.color = BLUE;
        }
        else {
            this.cardOne.verso.sprite = reviewVerso;
            this.cardTwo.verso.sprite = reviewVerso;
            this.cardThree.verso.sprite = reviewVerso;
            this.cardOne.recto.color = GREEN;
            this.cardTwo.recto.color = GREEN;
            this.cardThree.recto.color = GREEN;
        }
    }

    public void ChooseCard(UICard clickedCard){

    }
}
