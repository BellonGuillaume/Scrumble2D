using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using TMPro;

public class ReviewManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject backGround;
    [SerializeField] GameObject workStation;
    [SerializeField] GameObject arrowedUSPrefab;
    [SerializeField] GameObject littleArrowUSPrefab;
    [SerializeField] TMP_Text indication;
    [SerializeField] AnimationManager animationManager;
    [SerializeField] Transform container;
    [SerializeField] Slider debtSlider;
    List<GameObject> arrowedUSes = new List<GameObject>();
    int debtToAdd = 0;
    int caseToReplace = 0;
    bool usHandled;

    public IEnumerator handleReview(){
        yield return new WaitUntil(() => StateManager.gameState == StateManager.GameState.REVIEW);
        PopulateUS();
        ShowReviewScreen();
        yield return new WaitUntil(() => EventManager.animate == false);
        for (int i = 0; i < arrowedUSes.Count; i++){
            this.usHandled = false;
            StartCoroutine(HandleSingleUS(arrowedUSes[i]));
            yield return new WaitUntil(() => this.usHandled == true);
            EventManager.usToShift = 0;
            MoveUS(i);
            yield return new WaitUntil(() => EventManager.usToShift <= 0);
        }
        HideReviewScreen();
        yield return new WaitUntil(() => EventManager.animate == false);
        UpdateDebt();
        yield return new WaitUntil(() => EventManager.animate == false);
        ResetReview();
        StateManager.gameState = StateManager.GameState.RETROSPECTIVE;
    }

    private void PopulateUS(){
        foreach(GameObject arrowedUS in gameManager.doingAUS){
            GameObject go = Instantiate(arrowedUSPrefab);
            go.GetComponent<UserStoryUI>().Fill(arrowedUS.GetComponent<UserStoryUI>().userStory);
            go.GetComponent<ArrowedUS>().SetUserStory(arrowedUS.GetComponent<UserStoryUI>().userStory);
            go.transform.SetParent(container);
            go.transform.position = new Vector3((arrowedUSes.Count * (-960f)) + 960f, 540f, 0f);
            go.transform.localScale = Vector3.one;
            go.SetActive(true);
            arrowedUSes.Add(go);
            Destroy(arrowedUS);
        }
        gameManager.doingAUS = new List<GameObject>();
        foreach (GameObject userStoryUS in this.arrowedUSes){
            UserStory userStory = userStoryUS.GetComponent<UserStoryUI>().userStory;
            Debug.Log($"Informations de la US n°{userStory.id} : pos({userStoryUS.transform.position.x}, {userStoryUS.transform.position.y}, {userStoryUS.transform.position.z}), scale({userStoryUS.transform.localScale.x}, {userStoryUS.transform.localScale.y}, {userStoryUS.transform.localScale.z}), active({userStoryUS.activeSelf})");
        }
    }

    private void ShowReviewScreen(){
        animationManager.ShowReviewScreen(this.gameObject, backGround, container.gameObject);
    }

    private void HideReviewScreen(){
        animationManager.HideReviewScreen(this.gameObject, backGround, container.gameObject);
    }

    IEnumerator HandleSingleUS(GameObject arrowedUS){
        // animate the score
        yield return new WaitForSeconds(1);
        if (arrowedUS.GetComponent<UserStoryUI>().userStory.currentTask >= arrowedUS.GetComponent<UserStoryUI>().userStory.maxTask){
            Debug.Log($"Ready to deploy UserStory n°{arrowedUS.GetComponent<UserStoryUI>().userStory.id.ToString()}");
            arrowedUS.GetComponent<UserStoryUI>().userStory.state = UserStory.State.DEPLOYED;
            animationManager.ApproveUS(arrowedUS);
            yield return new WaitUntil(() => EventManager.animate == false);
            MoveOutOfScreen(arrowedUS);
            yield return new WaitUntil(() => EventManager.animate == false);
            this.usHandled = true;
        } else {
            int remainingTasks = arrowedUS.GetComponent<UserStoryUI>().userStory.maxTask - arrowedUS.GetComponent<UserStoryUI>().userStory.currentTask;
            this.debtToAdd += remainingTasks / 5;
            Debug.Log($"Remaining tasks and debt increase : {remainingTasks.ToString()}, {(remainingTasks / 5).ToString()}");
            indication.text = GetString("RemainingTasks") + " " + remainingTasks.ToString() + $"\n" + GetString("DebtRaised") + " " + debtToAdd.ToString();
            indication.gameObject.transform.parent.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            EventManager.usResized = false;
            StartCoroutine(MoveToFirstWorkingCase(arrowedUS));
            yield return new WaitUntil(() => EventManager.usResized == true);
            GameObject go = Instantiate(littleArrowUSPrefab);
            go.GetComponent<UserStoryUI>().Fill(arrowedUS.GetComponent<UserStoryUI>().userStory);
            go.GetComponent<ArrowedUS>().SetUserStory(arrowedUS.GetComponent<UserStoryUI>().userStory);
            go.GetComponent<ArrowedUS>().HideArrows();
            go.transform.SetParent(workStation.transform.GetChild(caseToReplace));
            go.transform.localPosition = Vector3.zero;
            gameManager.doingAUS.Add(go);
            caseToReplace++;
            indication.gameObject.transform.parent.gameObject.SetActive(false);
            this.usHandled = true;
        }
    }

    public void UpdateDebt(){
        if (debtToAdd > 0){
            animationManager.UpdateDebtScrollBar(debtSlider.value + debtToAdd);
        }
    }

    private void MoveOutOfScreen(GameObject arrowedUS){
        animationManager.MoveOutOfScreen(arrowedUS);
    }

    private void MoveUS(int n){
        for (int i = n; i < arrowedUSes.Count; i++){
            EventManager.usToShift++;
            animationManager.ShiftUsToRight(arrowedUSes[i]);
        }
    }

    IEnumerator MoveToFirstWorkingCase(GameObject arrowedUS){
        Transform placeholder = workStation.transform.GetChild(caseToReplace);
        Vector2 position = placeholder.position;
        animationManager.MoveUSToCase(arrowedUS, position);
        yield return new WaitUntil(() => EventManager.animate == false);
        // add to workstation
        arrowedUS.SetActive(false);
        EventManager.usResized = true;
    }
    private void ResetReview(){
        debtToAdd = 0;
        caseToReplace = 0;
        foreach (GameObject arrowedUS in arrowedUSes){
            Destroy(arrowedUS);
        }
        arrowedUSes = new List<GameObject>();
    }

    public string GetString(string stringKey){
        return LocalizationSettings.StringDatabase.GetLocalizedString("Review", stringKey);
    }
}
