using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowedUS : MonoBehaviour
{
    [SerializeField] Image outline;
    // [SerializeField] Button arrowUp;
    // [SerializeField] Button arrowDown;


    [HideInInspector] public UserStory userStory;
    public Color baseColor = UserStory.yellow; // Couleur de base par défaut
    public Color color = UserStory.red; // Couleur spécifique par défaut
    public float startAngle = 0f; // Angle de départ par défaut
    public float openAngle = 90f; // Ouverture d'angle par défaut

    private Material modifiedMaterial; // Matériau modifié pour cet objet

    void Start(){
        this.userStory = new UserStory(1, "GIFT SHOP", "Une description", 5, UserStory.Size.XS, 0);
        this.userStory.maxTask = 24;

        modifiedMaterial = new Material(outline.material);
        modifiedMaterial.SetColor("_BaseColor", baseColor);
        modifiedMaterial.SetColor("_Color", color);
        modifiedMaterial.SetFloat("_StartAngle", startAngle);
        modifiedMaterial.SetFloat("_OpenAngle", openAngle);
        outline.material = modifiedMaterial;

        UpdateColor(this.userStory.currentTask);
    }

    public void ClickUp(){
        IncreaseColor(1);
    }
    public void ClickDown(){
        IncreaseColor(-1);
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
}
