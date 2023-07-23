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
    IEnumerator OnAwake(){
        yield return new WaitForSeconds(0f);
        foreach (UserStory userStory in StateManager.userStories){
            GameObject userStoryUI = CreateUserStoryUI(userStory);
            if (userStory.state == UserStory.State.TODO){
                toDoDropContent.AddUsUI(userStoryUI);
            } else if (userStory.state == UserStory.State.DOING){
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
        return go;
    }

    public void OnClick(){
        List<UserStory> doingUS = doingDropContent.GetNewUserStories();
        foreach (UserStory userStory in doingUS){
            StateManager.userStories[userStory.id-1].state = UserStory.State.DOING;
            StateManager.userStories[userStory.id-1].maxTask = StateManager.debtFactor * StateManager.players.Count * (int) userStory.size;
            GameManager.workingOn.Add(StateManager.userStories[userStory.id-1]);
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
