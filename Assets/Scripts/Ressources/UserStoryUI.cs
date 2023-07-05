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
    UserStory userStory;
    PokerPlanningManager pokerPlanningManager;

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
        this.sizeTXT.text = userStory.size.ToString();
    }
    public void Connect(Object manager){
        if (manager is PokerPlanningManager){
            this.pokerPlanningManager = (PokerPlanningManager) manager;
        }
    }

    public UserStory GetUserStory(){
        return this.userStory;
    }

    public void OnClick(){
        if (StateManager.gameState == StateManager.GameState.POKER_PLANNING){
            pokerPlanningManager.OnUserStoryClick(this.userStory.id);
        }
    }

    public void SetSize(UserStory.Size size){
        this.userStory.size = size;
        this.sizeTXT.text = size.ToString();
    }
}

public class UserStory{
    public enum Size{
        XS, S, M, L, XL
    }
    public enum State{
        NEW, PROGRESS, FINISHED
    }

    public int id;
    public string category;
    public int restriction;
    public string description;
    public int defaultStars;
    public int stars;
    public Size defaultSize;
    public Size size;
    public State state;

    public UserStory(int id, string category, string description, int defaultStars, Size defaultSize, int restriction)
    {
        this.id = id;
        this.category = category;
        this.restriction = restriction;
        this.description = description;
        this.defaultStars = defaultStars;
        this.stars = defaultStars;
        this.defaultSize = defaultSize;
        this.size = defaultSize;
        this.state = State.NEW;
    }

    public override string ToString()
    {
        return  "id : " + this.id +
                ", category : " + this.category +
                ", default stars : " + this.defaultStars +
                ", stars : " + this.stars +
                ", default size : " + this.defaultSize +
                ", size : " + this.size +
                ", restriction : " + this.restriction +
                ", description : " + this.description;
    }
}
