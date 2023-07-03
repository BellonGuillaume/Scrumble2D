using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPicker : MonoBehaviour
{
    [SerializeField] UIRecto uiCardPrefab;
    [SerializeField] Sprite dailyVerso;
    [SerializeField] Sprite problemVerso;
    [SerializeField] Sprite reviewVerso;
    [SerializeField] GameObject versoOne;
    Image versoOneImage;
    [SerializeField] GameObject versoTwo;
    Image versoTwoImage;
    [SerializeField] GameObject versoThree;
    Image versoThreeImage;
    List<GameObject> versos;
    List<Image> versosImages;
    [SerializeField] GameObject recto;

    Color RED = new Color32(202, 95, 95, 255);
    Color BLUE = new Color32(88, 136, 199, 255);
    Color GREEN = new Color32(113, 203, 99, 255);

    public static string typeOfCard;
    public static string cardDescription;
    public static string cardResult;
    public static bool initialized;


    // Start is called before the first frame update
    void Start()
    {
        versos = new List<GameObject>{versoOne, versoTwo, versoThree};
        this.versoOneImage = this.versoOne.transform.GetChild(0).GetComponent<Image>();
        this.versoTwoImage = this.versoTwo.transform.GetChild(0).GetComponent<Image>();
        this.versoThreeImage = this.versoThree.transform.GetChild(0).GetComponent<Image>();
        versosImages = new List<Image>{versoOneImage, versoTwoImage, versoThreeImage};
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(typeOfCard == "PROBLEM"){
            this.versoOneImage.sprite = problemVerso;
            this.versoTwoImage.sprite = problemVerso;
            this.versoThreeImage.sprite = problemVerso;
            this.recto.GetComponent<Image>().color = RED;
        }
        else if (typeOfCard == "DAILY"){
            this.versoOneImage.sprite = dailyVerso;
            this.versoTwoImage.sprite = dailyVerso;
            this.versoThreeImage.sprite = dailyVerso;
            this.recto.GetComponent<Image>().color = BLUE;
        }
        else {
            this.versoOneImage.sprite = reviewVerso;
            this.versoTwoImage.sprite = reviewVerso;
            this.versoThreeImage.sprite = reviewVerso;
            this.recto.GetComponent<Image>().color = GREEN;
        }
        this.recto.transform.GetChild(0).GetComponent<TMP_Text>().text = cardDescription;
        this.recto.transform.GetChild(1).GetComponent<TMP_Text>().text = cardResult;
    }

    public void ChooseCard(int versoNum){
        if (versoNum == 1){
            StartCoroutine(RemoveCard(0));
            StartCoroutine(DisableCard(1));
            StartCoroutine(DisableCard(2));
            StartCoroutine(ShowRecto());
        }
        else if (versoNum == 2){
            StartCoroutine(DisableCard(0));
            StartCoroutine(RemoveCard(1));
            StartCoroutine(DisableCard(2));
            StartCoroutine(ShowRecto());
        }
        else {
            StartCoroutine(DisableCard(0));
            StartCoroutine(DisableCard(1));
            StartCoroutine(RemoveCard(2));
            StartCoroutine(ShowRecto());
        }
    }

    public void Reset(){
        for (int i = 0; i < this.versos.Count; i++){
            this.versos[i].SetActive(true);
            this.versosImages[i].sprite = null;
            this.versos[i].transform.GetChild(1).GetComponent<Button>().interactable = true;
            Color tempColor = this.versos[i].transform.GetChild(1).GetComponent<Button>().image.color;
            tempColor.a = 0f;
            this.versos[i].transform.GetChild(1).GetComponent<Button>().image.color = tempColor;
        }
        this.recto.SetActive(false);
    }

    #region Animations
    public IEnumerator DisableCard(int i){
        this.versos[i].transform.GetChild(1).GetComponent<Button>().interactable = false;
        Color tempColor = this.versos[i].transform.GetChild(1).GetComponent<Button>().image.color;
        tempColor.a = 0.8f;
        this.versos[i].transform.GetChild(1).GetComponent<Button>().image.color = tempColor;
        yield break;
    }

    public IEnumerator RemoveCard(int i){
        this.versos[i].SetActive(false);
        yield break;
    }

    public IEnumerator ShowRecto(){
        this.recto.SetActive(true);
        yield return new WaitForSeconds(4);
        StateManager.turnState = StateManager.TurnState.RESULT;
    }
    #endregion
}
