using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowedUS : MonoBehaviour
{
    [SerializeField] Image outline;
    [SerializeField] Button arrowUp;
    [SerializeField] Button arrowDown;
    [SerializeField] HighlightScore highlightScore;


    [HideInInspector] public UserStory userStory;

    public Color baseColor = UserStory.yellow; // Couleur de base par défaut
    public Color color = UserStory.red; // Couleur spécifique par défaut
    public float startAngle = 0f; // Angle de départ par défaut
    public float openAngle = 90f; // Ouverture d'angle par défaut
    public int delta;

    private Material modifiedMaterial; // Matériau modifié pour cet objet

    void Start(){
        this.delta = 0;

        modifiedMaterial = new Material(outline.material);
        modifiedMaterial.SetColor("_BaseColor", baseColor);
        modifiedMaterial.SetColor("_Color", color);
        modifiedMaterial.SetFloat("_StartAngle", startAngle);
        modifiedMaterial.SetFloat("_OpenAngle", openAngle);
        outline.material = modifiedMaterial;
    }

    public void ClickUp(){
        this.delta++;
        IncreaseColor(1);
        EventManager.taskToAdd--;
        if (userStory.currentTask == 0)
            userStory.state = UserStory.State.SPRINT_BACKLOG;
        else if (userStory.currentTask == userStory.maxTask)
            userStory.state = UserStory.State.DONE;
        else
            userStory.state = UserStory.State.IN_PROGRESS;
    }
    public void ClickDown(){
        this.delta--;
        IncreaseColor(-1);
        EventManager.taskToAdd++;
        if (userStory.currentTask == 0)
            userStory.state = UserStory.State.SPRINT_BACKLOG;
        else if (userStory.currentTask == userStory.maxTask)
            userStory.state = UserStory.State.DONE;
        else
            userStory.state = UserStory.State.IN_PROGRESS;
    }

    public void SetUserStory(UserStory userStory){
        this.userStory = userStory;
        this.highlightScore.userStory = userStory;
    }
    public void IncreaseColor(int i){
        if (this.userStory.currentTask > this.userStory.maxTask){
            return;
        }
        if (this.userStory.currentTask < 0){
            return;
        }
        if (this.userStory.currentTask == this.userStory.maxTask && i > 0){
            return;
        }
        if (this.userStory.currentTask == 0 && i < 0){
            return;
        }
        this.userStory.currentTask += i;
        UpdateColor(this.userStory.currentTask);
    }
    public void UpdateColor(int currentTask){
        float approximation = ((float) currentTask) / ((float) this.userStory.maxTask);

        float angle = Mathf.RoundToInt(approximation * 360);
        int colorNumber = Mathf.RoundToInt(approximation * (UserStory.colors.Count - 1));

        SetColor(UserStory.colors[colorNumber]);
        SetAngle(startAngle, angle);
    }

    public void SetColor(Color newColor)
    {
        color = newColor;
        modifiedMaterial.SetColor("_Color", color);
    }

    public void SetAngle(float newStartAngle, float newOpenAngle)
    {
        startAngle = newStartAngle;
        openAngle = newOpenAngle;
        modifiedMaterial.SetFloat("_StartAngle", startAngle);
        modifiedMaterial.SetFloat("_OpenAngle", openAngle);
    }

    public void ShowUpArrow(){
        this.arrowUp.gameObject.SetActive(true);
        this.arrowDown.gameObject.SetActive(false);
    }

    public void HideUpArrow(){
        this.arrowUp.gameObject.SetActive(false);
    }
    public void ShowDownArrow(){
        this.arrowDown.gameObject.SetActive(true);
        this.arrowUp.gameObject.SetActive(false);
    }
    public void HideDownArrow(){
        this.arrowDown.gameObject.SetActive(false);
    }
    public void ShowArrows(){
        this.arrowUp.gameObject.SetActive(true);
        this.arrowDown.gameObject.SetActive(true);
    }
    public void HideArrows(){
        this.arrowUp.gameObject.SetActive(false);
        this.arrowDown.gameObject.SetActive(false);
    }
}
