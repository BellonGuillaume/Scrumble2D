using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDDDManager : MonoBehaviour
{
    [SerializeField] DropContent toDoDropContent;
    [SerializeField] DropContent doingDropContent;
    [SerializeField] GameObject littleUserStoryUSPrefab;


    void Start(){
        foreach (UserStory userStory in StateManager.userStories){
            GameObject userStoryUI = CreateUserStoryUI(userStory);
            toDoDropContent.AddUsUI(userStoryUI);
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
            GameManager.workingOn.Add(userStory);
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
