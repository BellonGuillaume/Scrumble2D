using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserStoryUI : MonoBehaviour
{
    [SerializeField] TMP_Text idTXT;
    [SerializeField] TMP_Text preRestrictionTXT;
    [SerializeField] TMP_Text restrictionTXT;
    [SerializeField] TMP_Text descriptionTXT;
    [SerializeField] TMP_Text starsTXT;
    [SerializeField] TMP_Text sizeTXT;
    [SerializeField] Image outline;
    [SerializeField] Button clickHandler;
    public UserStory userStory;
    PokerPlanningManager pokerPlanningManager;
    CustomPokerPlanningManager customPokerPlanningManager;
    [HideInInspector] public bool canBeDrag = true;

    public void Fill(UserStory userStory){
        this.userStory = userStory;
        this.idTXT.text = userStory.id.ToString();
        if (userStory.restriction == 0){
            this.preRestrictionTXT.enabled = false;
            this.restrictionTXT.enabled = false;
        } else {
            this.preRestrictionTXT.enabled = true;
            this.restrictionTXT.enabled = true;
            this.restrictionTXT.text = userStory.restriction.ToString();
        }
        this.descriptionTXT.text = userStory.description;
        this.starsTXT.text = userStory.stars.ToString();
        if (userStory.size == UserStory.Size.NOT_DEFINED){
            this.sizeTXT.text = "";
        } else {
            this.sizeTXT.text = userStory.size.ToString();
        }
    }
    public void Connect(Object manager){
        if (manager is PokerPlanningManager){
            this.pokerPlanningManager = (PokerPlanningManager) manager;
        }
        else if (manager is CustomPokerPlanningManager){
            this.customPokerPlanningManager = (CustomPokerPlanningManager) manager;
        }
    }

    public UserStory GetUserStory(){
        return this.userStory;
    }

    public void OnClick(){
        if (StateManager.gameState == StateManager.GameState.POKER_PLANNING){
            pokerPlanningManager.OnUserStoryClick(this.userStory.id);
        }
        else if (StateManager.gameState == StateManager.GameState.CUSTOM_POKER_PLANNING){
            customPokerPlanningManager.OnUserStoryClick(this.userStory.id);
        }
    }

    public void SetSize(UserStory.Size size){
        this.userStory.size = size;
        this.sizeTXT.text = size.ToString();
    }

    public void ChangeOutlineColor(UserStory.OutlineColor outlineColor){
        switch (outlineColor){
            case UserStory.OutlineColor.YELLOW :
                this.outline.color = UserStory.yellow;
                break;
            case UserStory.OutlineColor.RED :
                this.outline.color = UserStory.red;
                break;
            case UserStory.OutlineColor.ORANGE :
                this.outline.color = UserStory.orange;
                break;
            case UserStory.OutlineColor.GREEN :
                this.outline.color = UserStory.green;
                break;
            default :
                this.outline.color = UserStory.yellow;
                break;
        }
    }
    public void Deactivate(){
        Color32 temp = clickHandler.image.color;
        temp.a = 100;
        clickHandler.image.color = temp;
        canBeDrag = false;
    }
    public void Activate(){
        Color32 temp = clickHandler.image.color;
        temp.a = 0;
        clickHandler.image.color = temp;
        canBeDrag = true;
    }
}