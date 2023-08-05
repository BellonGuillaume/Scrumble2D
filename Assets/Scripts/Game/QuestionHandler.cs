using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

public class QuestionHandler : MonoBehaviour
{
    [SerializeField] CardHandler cardHandler;
    [SerializeField] Button answer1;
    [SerializeField] Button answer2;
    [SerializeField] Button answer3;
    [SerializeField] Button answer4;

    Dictionary<int, string> buttonsDictionary = new Dictionary<int, string>(){
        {0, ""},
        {1, ""},
        {2, ""},
        {3, ""}
    };
    private bool questionHandled;
    private bool button1Pressed;
    private bool button2Pressed;
    private bool button3Pressed;
    private bool button4Pressed;
    public bool rightAnswerIsFound;
    public bool wrongAnswerIsFound;
    private int remainingRightAnswers = 3;

    public Card.Action successAction = Card.Action.None;
    public Card.Action failedAction = Card.Action.None;
    public int value;
    public IEnumerator HandleQuestion(Card card){
        ApplyAnswersToButtons(card);
        ShowQuestionHandler();
        rightAnswerIsFound = false;
        while (!rightAnswerIsFound && !wrongAnswerIsFound){
            yield return new WaitUntil(() => button1Pressed || button2Pressed || button3Pressed || button4Pressed);
            HandleAnswer(card);
            button1Pressed = false;
            button2Pressed = false;
            button3Pressed = false;
            button4Pressed = false;
        }
        yield return new WaitForSeconds(1);
        HideQuestionHandler();
        EventManager.questionHandled = true;
        yield break;
    }

    private void ApplyAnswersToButtons(Card card){
        List<string> answers = new List<string>();
        List<Button> buttons = new List<Button>();
        answers.Add(card.answer1);
        answers.Add(card.answer2);
        answers.Add(card.answer3);
        answers.Add(card.answer4);
        buttons.Add(answer1);
        buttons.Add(answer2);
        buttons.Add(answer3);
        buttons.Add(answer4);
        Debug.Log("Here are the answers :");
        foreach(string answer in answers){
            Debug.Log(answer);
        }
        Debug.Log("Here are the buttons :");
        foreach(Button button in buttons){
            Debug.Log(button.ToString());
        }
        for (int i = 0; i < 4; i++){
            int index = Random.Range(0, answers.Count);
            string temp = answers[index];
            buttons[i].GetComponentInChildren<TMP_Text>().text = temp;
            answers.RemoveAt(index);
            buttonsDictionary[i] = temp;
        }
    }

    public void OnClick(int n){
        if (n == 0)
            button1Pressed = true;
        else if (n == 1)
            button2Pressed = true;
        else if (n == 2)
            button3Pressed = true;
        else
            button4Pressed = true;
    }

    public void ShowQuestionHandler(){
        this.gameObject.SetActive(true);
    }
    public void HideQuestionHandler(){
        this.gameObject.SetActive(false);
    }

    public void HandleAnswer(Card card){
        string choosenAnswer;
        Button buttonPressed;
        
        if (button1Pressed){
            choosenAnswer = buttonsDictionary[0];
            buttonPressed = answer1;
        }
        else if (button2Pressed){
            choosenAnswer = buttonsDictionary[1];
            buttonPressed = answer2;
        }
        else if (button3Pressed){
            choosenAnswer = buttonsDictionary[2];
            buttonPressed = answer3;
        }
        else {
            choosenAnswer = buttonsDictionary[3];
            buttonPressed = answer4;
        }
        switch (card.questionId){
            case 1 :
                HandleQuestion1(card, choosenAnswer, buttonPressed);
                break;
            case 2 :
                HandleQuestion2(card, choosenAnswer, buttonPressed);
                break;
            case 3 :
                HandleQuestion3(card, choosenAnswer, buttonPressed);
                break;
            case 4 :
                HandleQuestion4(card, choosenAnswer, buttonPressed);
                break;
            case 5 :
                HandleQuestion5(card, choosenAnswer, buttonPressed);
                break;
            case 6 :
                HandleQuestion6(card, choosenAnswer, buttonPressed);
                break;
            case 7 :
                HandleQuestion7(card, choosenAnswer, buttonPressed);
                break;
            default :
                break;
        }
    }

    public void HandleQuestion1(Card card, string answer, Button buttonPressed){
        if (answer == "3"){
            buttonPressed.image.color = Color.green;
            rightAnswerIsFound = true;
            DeactivateButtons();
            successAction = Card.Action.IncreaseTask;
            value = 3;
        } else {
            buttonPressed.image.color = Color.red;
            if (answer1.GetComponentInChildren<TMP_Text>().text == "3")
                answer1.image.color = Color.green;
            else if (answer2.GetComponentInChildren<TMP_Text>().text == "3")
                answer2.image.color = Color.green;
            else if (answer3.GetComponentInChildren<TMP_Text>().text == "3")
                answer3.image.color = Color.green;
            else if (answer4.GetComponentInChildren<TMP_Text>().text == "3")
                answer4.image.color = Color.green;
            buttonPressed.enabled = false;
        }
    }

    public void HandleQuestion2(Card card, string answer, Button buttonPressed){
        if (answer == GetString("DevelopmentTeam")){
            buttonPressed.image.color = Color.green;
            successAction = Card.Action.DecreaseDebt;
            value = 3;
            rightAnswerIsFound = true;
        } else {
            buttonPressed.image.color = Color.red;
            if (answer1.GetComponentInChildren<TMP_Text>().text == "DevelopmentTeam")
                answer1.image.color = Color.green;
            else if (answer2.GetComponentInChildren<TMP_Text>().text == "DevelopmentTeam")
                answer2.image.color = Color.green;
            else if (answer3.GetComponentInChildren<TMP_Text>().text == "DevelopmentTeam")
                answer3.image.color = Color.green;
            else if (answer4.GetComponentInChildren<TMP_Text>().text == "DevelopmentTeam")
                answer4.image.color = Color.green;
            wrongAnswerIsFound = true;
        }
        DeactivateButtons();
    }
    public void HandleQuestion3(Card card, string answer, Button buttonPressed){
        if (answer == GetString("DidYesterday") || answer == GetString("DoToday") || answer == GetString("Obstacles")){
            buttonPressed.image.color = Color.green;
            buttonPressed.enabled = false;
            remainingRightAnswers--;
        } else {
            buttonPressed.image.color = Color.red;
            if (answer1.GetComponentInChildren<TMP_Text>().text == "DidYesterday" || answer1.GetComponentInChildren<TMP_Text>().text == "DoToday" || answer1.GetComponentInChildren<TMP_Text>().text == "Obstacles")
                answer1.image.color = Color.green;
            else if (answer2.GetComponentInChildren<TMP_Text>().text == "DidYesterday" || answer2.GetComponentInChildren<TMP_Text>().text == "DoToday" || answer2.GetComponentInChildren<TMP_Text>().text == "Obstacles")
                answer2.image.color = Color.green;
            else if (answer3.GetComponentInChildren<TMP_Text>().text == "DidYesterday" || answer3.GetComponentInChildren<TMP_Text>().text == "DoToday" || answer3.GetComponentInChildren<TMP_Text>().text == "Obstacles")
                answer3.image.color = Color.green;
            else if (answer4.GetComponentInChildren<TMP_Text>().text == "DidYesterday" || answer4.GetComponentInChildren<TMP_Text>().text == "DoToday" || answer4.GetComponentInChildren<TMP_Text>().text == "Obstacles")
                answer4.image.color = Color.green;
            failedAction = Card.Action.IncreaseDebt;
            value = 5;
            wrongAnswerIsFound = true;
            DeactivateButtons();
        }
        if (remainingRightAnswers <= 0){
            rightAnswerIsFound = true;
            DeactivateButtons();
        }
    }
    public void HandleQuestion4(Card card, string answer, Button buttonPressed){
        if (answer == "4"){
            buttonPressed.image.color = Color.green;
            successAction = Card.Action.DecreaseDebt;
            value = 4;
            rightAnswerIsFound = true;
        } else {
            buttonPressed.image.color = Color.red;
            if (answer1.GetComponentInChildren<TMP_Text>().text == "4")
                answer1.image.color = Color.green;
            else if (answer2.GetComponentInChildren<TMP_Text>().text == "4")
                answer2.image.color = Color.green;
            else if (answer3.GetComponentInChildren<TMP_Text>().text == "4")
                answer3.image.color = Color.green;
            else if (answer4.GetComponentInChildren<TMP_Text>().text == "4")
                answer4.image.color = Color.green;
            wrongAnswerIsFound = true;
        }
        DeactivateButtons();
    }
    public void HandleQuestion5(Card card, string answer, Button buttonPressed){
        if (answer == "2"){
            buttonPressed.image.color = Color.green;
            successAction = Card.Action.DecreaseDebt;
            value = 2;
            rightAnswerIsFound = true;
        } else {
            buttonPressed.image.color = Color.red;
            if (answer1.GetComponentInChildren<TMP_Text>().text == "2")
                answer1.image.color = Color.green;
            else if (answer2.GetComponentInChildren<TMP_Text>().text == "2")
                answer2.image.color = Color.green;
            else if (answer3.GetComponentInChildren<TMP_Text>().text == "2")
                answer3.image.color = Color.green;
            else if (answer4.GetComponentInChildren<TMP_Text>().text == "2")
                answer4.image.color = Color.green;
            wrongAnswerIsFound = true;
        }
        DeactivateButtons();
    }
    public void HandleQuestion6(Card card, string answer, Button buttonPressed){
        if (answer == "3"){
            buttonPressed.image.color = Color.green;
            successAction = Card.Action.DecreaseDebt;
            value = 3;
            rightAnswerIsFound = true;
        } else {
            buttonPressed.image.color = Color.red;
            if (answer1.GetComponentInChildren<TMP_Text>().text == "3")
                answer1.image.color = Color.green;
            else if (answer2.GetComponentInChildren<TMP_Text>().text == "3")
                answer2.image.color = Color.green;
            else if (answer3.GetComponentInChildren<TMP_Text>().text == "3")
                answer3.image.color = Color.green;
            else if (answer4.GetComponentInChildren<TMP_Text>().text == "3")
                answer4.image.color = Color.green;
            wrongAnswerIsFound = true;
        }
        DeactivateButtons();
    }
    public void HandleQuestion7(Card card, string answer, Button buttonPressed){
        if (answer == "8"){
            buttonPressed.image.color = Color.green;
            successAction = Card.Action.IncreaseTask;
            value = 8;
            rightAnswerIsFound = true;
        } else {
            buttonPressed.image.color = Color.red;
            if (answer1.GetComponentInChildren<TMP_Text>().text == "8")
                answer1.image.color = Color.green;
            else if (answer2.GetComponentInChildren<TMP_Text>().text == "8")
                answer2.image.color = Color.green;
            else if (answer3.GetComponentInChildren<TMP_Text>().text == "8")
                answer3.image.color = Color.green;
            else if (answer4.GetComponentInChildren<TMP_Text>().text == "8")
                answer4.image.color = Color.green;
            wrongAnswerIsFound = true;
        }
        DeactivateButtons();
    }

    public void DeactivateButtons(){
        answer1.enabled = false;
        answer2.enabled = false;
        answer3.enabled = false;
        answer4.enabled = false;
    }

    public void ActivateButtons(){
        answer1.enabled = true;
        answer2.enabled = true;
        answer3.enabled = true;
        answer4.enabled = true;
    }
    public void ResetQuestionHandler(){
        buttonsDictionary[0] = "";
        buttonsDictionary[1] = "";
        buttonsDictionary[2] = "";
        buttonsDictionary[3] = "";
        button1Pressed = false;
        button2Pressed = false;
        button3Pressed = false;
        button4Pressed = false;
        answer1.gameObject.SetActive(true);
        answer2.gameObject.SetActive(true);
        answer3.gameObject.SetActive(true);
        answer4.gameObject.SetActive(true);
        answer1.image.color = Color.white;
        answer2.image.color = Color.white;
        answer3.image.color = Color.white;
        answer4.image.color = Color.white;
        answer1.GetComponentInChildren<TMP_Text>().text = "";
        answer2.GetComponentInChildren<TMP_Text>().text = "";
        answer3.GetComponentInChildren<TMP_Text>().text = "";
        answer4.GetComponentInChildren<TMP_Text>().text = "";
        ActivateButtons();
        rightAnswerIsFound = false;
        wrongAnswerIsFound = false;
        successAction = Card.Action.None;
        failedAction = Card.Action.None;
        value = 0;
    }
    public string GetString(string stringKey){
        return LocalizationSettings.StringDatabase.GetLocalizedString("Question", stringKey);
    }
}
