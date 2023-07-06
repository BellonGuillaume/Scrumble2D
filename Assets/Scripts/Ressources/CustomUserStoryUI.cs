using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomUserStoryUI : MonoBehaviour
{
    [SerializeField] TMP_Text idTXT;
    [SerializeField] TMP_Text restrictionTXT;
    [SerializeField] TMP_InputField restrictionIn;
    [SerializeField] TMP_Text asATXT;
    [SerializeField] TMP_InputField asAIn;
    [SerializeField] TMP_InputField iWantIn;
    [SerializeField] TMP_Dropdown starsIn;
    [SerializeField] TMP_Dropdown sizeIn;

    public int id;

    UserStory userStory;

    [SerializeField] CustomPokerPlanningManager customPokerPlanningManager;

    public void Fill(UserStory userStory){
        this.id = userStory.id;
        this.idTXT.text = userStory.id.ToString();
        if (userStory.restriction == 0){
            this.restrictionIn.text = "";
        } else {
            this.restrictionIn.text = userStory.restriction.ToString();
        }
        this.asAIn.text = userStory.asA;
        this.iWantIn.text = userStory.iWant;
        this.starsIn.value = userStory.stars;
        switch (userStory.size){
            case UserStory.Size.XS :
                this.sizeIn.value = 1;
                break;
            case UserStory.Size.S :
                this.sizeIn.value = 2;
                break;
            case UserStory.Size.M :
                this.sizeIn.value = 3;
                break;
            case UserStory.Size.L :
                this.sizeIn.value = 4;
                break;
            case UserStory.Size.XL :
                this.sizeIn.value = 5;
                break;
            default :
                this.sizeIn.value = 0;
                break;
        }

    }
    public void OnAsAChange(){
        this.customPokerPlanningManager.userStories[this.id-1].description = "En tant que " + this.asAIn.text + ",\nje veux " + this.iWantIn.text;
        this.customPokerPlanningManager.userStories[this.id-1].asA = this.asAIn.text;
        this.customPokerPlanningManager.UpdateUserStoryUI(this.id);
    }
    public void OnIWantChange(){
        this.customPokerPlanningManager.userStories[this.id-1].description = "En tant que " + this.asAIn.text + ",\nje veux " + this.iWantIn.text;
        this.customPokerPlanningManager.userStories[this.id-1].iWant = this.iWantIn.text;
        this.customPokerPlanningManager.UpdateUserStoryUI(this.id);
    }
    public void OnRestrictionChange(){
        int result = 0;
        if (this.restrictionIn.text != ""){
            result = int.Parse(this.restrictionIn.text);
        }
        this.customPokerPlanningManager.userStories[this.id-1].restriction = result;
        this.customPokerPlanningManager.UpdateUserStoryUI(this.id);
    }
    public void OnStarsChange(){
        this.customPokerPlanningManager.userStories[this.id-1].stars = this.starsIn.value;
        this.customPokerPlanningManager.UpdateUserStoryUI(this.id);
    }
    public void OnSizeChange(){
        UserStory.Size size = UserStory.Size.XL;
        switch (this.sizeIn.value){
            case 1 :
                size = UserStory.Size.XS;
                break;
            case 2 :
                size = UserStory.Size.S;
                break;
            case 3 :
                size = UserStory.Size.M;
                break;
            case 4 :
                size = UserStory.Size.L;
                break;
            case 5 :
                size = UserStory.Size.XL;
                break;
            default :
                size = UserStory.Size.NOT_DEFINED;
                break;
        }
        this.customPokerPlanningManager.userStories[this.id-1].size = size;
        this.customPokerPlanningManager.UpdateUserStoryUI(this.id);
    }
}
