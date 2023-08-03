using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDDDManager : MonoBehaviour
{
    [SerializeField] DropContent toDoDropContent;
    [SerializeField] DropContent doingDropContent;
    [SerializeField] Content doneContent;
    [SerializeField] GameObject littleUserStoryUSPrefab;

    void Awake(){
        StartCoroutine(OnAwake());
    }

    void Update(){
        foreach (DropCase dropCase in toDoDropContent.cases){
            if(dropCase.transform.childCount != 0){
                UserStoryUI usUI = dropCase.transform.GetChild(0).GetComponent<UserStoryUI>();
                if (usUI.userStory.restriction > 0){
                    if(StateManager.userStories[usUI.userStory.restriction-1].state == UserStory.State.DEPLOYED)
                        usUI.Activate();
                    else
                        usUI.Deactivate();
                }
            }
        }
        foreach (DropCase dropCase in doingDropContent.cases){
            if(dropCase.transform.childCount != 0){
                UserStoryUI usUI = dropCase.transform.GetChild(0).GetComponent<UserStoryUI>();
                if (usUI.userStory.state == UserStory.State.DEPLOYED){
                    this.doneContent.AddUsUI(usUI.gameObject);
                }
            }
        }
    }
    IEnumerator OnAwake(){
        yield return new WaitForSeconds(0f);
        foreach (UserStory userStory in StateManager.userStories){
            GameObject userStoryUI = CreateUserStoryUI(userStory);
            if (userStory.state == UserStory.State.PRODUCT_BACKLOG){
                toDoDropContent.AddUsUI(userStoryUI);
            } else if (userStory.state == UserStory.State.SPRINT_BACKLOG || userStory.state == UserStory.State.IN_PROGRESS || userStory.state == UserStory.State.DONE){
                doingDropContent.AddUsUI(userStoryUI);
            } else {
                doneContent.AddUsUI(userStoryUI);
            }
        }
        foreach (DropCase dropCase in toDoDropContent.cases){
            if (dropCase.transform.childCount == 0){
                continue;
            }
            dropCase.transform.GetChild(0).localScale = Vector3.one;
        }
        foreach (DropCase dropCase in doingDropContent.cases){
            if (dropCase.transform.childCount == 0){
                continue;
            }
            dropCase.transform.GetChild(0).localScale = Vector3.one;
        }
        foreach (DropCase dropCase in doneContent.cases){
            if (dropCase.transform.childCount == 0){
                continue;
            }
            dropCase.transform.GetChild(0).localScale = Vector3.one;
        }
    }

    public GameObject CreateUserStoryUI(UserStory userStory){
        GameObject go = Instantiate(this.littleUserStoryUSPrefab);
        go.GetComponent<UserStoryUI>().Fill(userStory);
        go.AddComponent<DraggableItem>();
        go.GetComponent<DraggableItem>().userStory = userStory;
        go.GetComponent<DraggableItem>().userStoryUI = go.GetComponent<UserStoryUI>();
        return go;
    }

    public void OnClick(){
        List<UserStory> doingUS = doingDropContent.GetNewUserStories();
        int maxLowered = 0;
        if (StateManager.maxUserStoryLowered)
            maxLowered = 3;
        foreach (UserStory userStory in doingUS){
            if(StateManager.userStories[userStory.id-1].state == UserStory.State.PRODUCT_BACKLOG){
                StateManager.userStories[userStory.id-1].state = UserStory.State.SPRINT_BACKLOG;
                StateManager.userStories[userStory.id-1].maxTask = (StateManager.debtFactor * StateManager.players.Count * (int) userStory.size) - maxLowered;
                GameManager.workingOn.Add(StateManager.userStories[userStory.id-1]);
            }
        }
    }

    public override string ToString()
    {
        string temp = "";
        List<UserStory> todoUS = toDoDropContent.GetUserStories();
        List<UserStory> doingUS = doingDropContent.GetUserStories();
        foreach (UserStory userStory in todoUS){
            temp += $"UserStory n°{userStory.id}'s state : {userStory.state.ToString()}\n";
        }
        foreach (UserStory userStory in doingUS){
            temp += $"UserStory n°{userStory.id}'s state : {userStory.state.ToString()}\n";
        }
        return temp;
    }
}
