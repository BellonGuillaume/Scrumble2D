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
    UserStory userStory;
    PokerPlanningManager pokerPlanningManager;
    CustomPokerPlanningManager customPokerPlanningManager;
    public enum OutlineColor{
        GREEN, RED, ORANGE, YELLOW
    }
    Color yellow = new Color32(255, 208, 0, 255);
    Color red = new Color32(255, 31, 0, 255);
    Color orange = new Color32(255, 117, 0, 255);
    Color green = new Color32(124, 215, 70, 255);

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

    public void ChangeOutlineColor(OutlineColor outlineColor){
        switch (outlineColor){
            case OutlineColor.YELLOW :
                this.outline.color = this.yellow;
                break;
            case OutlineColor.RED :
                this.outline.color = this.red;
                break;
            case OutlineColor.ORANGE :
                this.outline.color = this.orange;
                break;
            case OutlineColor.GREEN :
                this.outline.color = this.green;
                break;
            default :
                this.outline.color = this.yellow;
                break;
        }
    }
}